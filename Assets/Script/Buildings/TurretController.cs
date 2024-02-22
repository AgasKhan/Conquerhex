using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurretController : BuildingsController
{
    [SerializeReference]
    AutomaticAttack prin;

    [SerializeReference]
    AutomaticAttack sec;

    [SerializeReference]
    AutomaticAttack ter;

    [SerializeField]
    public TurretSubMenu subMenu;

    public TurretBuild turret;

    protected override void Config()
    {
        base.Config();

        MyAwakes += MyAwake;
        MyStarts += MyStart;
    }

    private void MyAwake()
    {
        SetAttack(prin = new AutomaticAttack(turret.attack, 0));

        SetAttack(sec = new AutomaticAttack(turret.attack, 1));

        SetAttack(ter = new AutomaticAttack(turret.attack, 2));

        turret.health.noLife += DestroyTurret;
    }
    private void MyStart()
    {
        subMenu.Init(turret);
    }
    void SetAttack(AutomaticAttack automatic)
    {
        automatic.onAttack += () =>
        {
            if (automatic.weaponKata != null)
                automatic.timerToAttack.Set(automatic.weaponKata.finalVelocity + 0.1f);
        };

        automatic.timerToAttack.Start();
    }

    protected virtual void DestroyTurret()
    {
        prin.StopTimers();
        sec.StopTimers();
        ter.StopTimers();

        turret.DestroyConstruction();
    }

    public override void EnterBuild()
    {
        if (turret.currentLevel < turret.maxLevel)
            subMenu.Create();
        else
            MenuManager.instance.modulesMenu.ObtainMenu<PopUp>(false).SetActiveGameObject(true).SetWindow("", "La torreta alcanzó el nivel máximo").AddButton("Cerrar", () => MenuManager.instance.modulesMenu.ObtainMenu<PopUp>(false));
    }

    public override void UpgradeLevel()
    {
        turret.currentLevel++;
        if (turret.currentLevel > turret.maxLevel)
            turret.currentLevel = turret.maxLevel;
    }

    private void OnDestroy()
    {
        turret.DestroyConstruction();
    }

}

[System.Serializable]
public class TurretSubMenu : CreateSubMenu
{
    TurretBuild turretBuilding;
    EventsCall lastButton;
    DetailsWindow myDetailsW;

    public void Init(TurretBuild turret)
    {
        base.Init();

        turretBuilding = turret;
    }

    public override void Create()
    {
        subMenu = MenuManager.instance.modulesMenu.ObtainMenu<SubMenus>();

        subMenu.ClearBody();
        subMenu.navbar.DestroyAll();
        base.Create();
    }
    protected override void InternalCreate()
    {
        DestroyLastButton();

        subMenu.CreateSection(0, 2);
        subMenu.CreateChildrenSection<ScrollRect>();
        CreateButtons();

        subMenu.CreateSection(2, 6);
        subMenu.CreateChildrenSection<ScrollRect>();
        myDetailsW = subMenu.AddComponent<DetailsWindow>().SetTexts("Elige una habilidad", "\nSelecciona una habilidad de la izquierda para ver más detalles de la misma, luego elige una, pero hazlo con cuidado no olvides que " + "sólo puedes escoger una mejora y será permanente\n\n".RichText("color", "#ff0000ff") + "Costo:\n".RichText("color", "#ffa500ff") + turretBuilding.upgradesRequirements[turretBuilding.currentLevel].GetRequiresString(turretBuilding.invent) + "\n");

        subMenu.CreateTitle("Torreta");
    }

    void CreateButtons()
    {
        for (int i = 0; i < turretBuilding.attack.flyweight.kataCombos.Length; i++)
        {
            var item = turretBuilding.attack.flyweight.kataCombos[i];
            var index = i;
            UnityEngine.Events.UnityAction abilityAction;

            if (item.kata.nameDisplay == turretBuilding.originalAbility)
                continue;

            if (turretBuilding.currentLevel == 0)
                abilityAction = () => { turretBuilding.originalAbility = item.kata.nameDisplay; turretBuilding.ChangeSprite(turretBuilding.myStructure.possibleAbilities[turretBuilding.originalAbility][0]); };
            else
                abilityAction = () => TurretMaxLevel();

            subMenu.AddComponent<EventsCall>().Set(item.kata.nameDisplay, () => { ButtonAction(item.kata, () => { if(AddAbility(index, turretBuilding.upgradesRequirements[turretBuilding.currentLevel])) abilityAction.Invoke(); }); }, "").rectTransform.sizeDelta = new Vector2(300, 75);
        }

        if (turretBuilding.currentLevel > 0)
        {
            foreach (var item in turretBuilding.myStructure.damagesUpgrades)
            {
                subMenu.AddComponent<EventsCall>().Set(item.nameDisplay, () => { ButtonAction(item, () => { ImproveDamage(item, turretBuilding.upgradesRequirements[turretBuilding.currentLevel]); TurretMaxLevel(); }); }, "").rectTransform.sizeDelta = new Vector2(300, 75);
            }
        }
    }

    void TurretMaxLevel()
    {
        /*
        foreach (var item in turretBuilding.interact)
        {
            if (item.key == "Mejorar")
                item.key = "Nivel Máximo";
        }
        */
        turretBuilding.ChangeSprite(turretBuilding.myStructure.possibleAbilities[turretBuilding.originalAbility][1]);
    }



    void ButtonAction(ItemBase item, UnityEngine.Events.UnityAction action)
    {
        DestroyLastButton();
        myDetailsW.SetTexts(item.nameDisplay, "\n" + item.GetDetails().ToString() + "Solo puedes elegir una habilidad y sera permanente".RichText("color", "#ff0000ff") + "\nCosto: \n".RichText("color", "#ffa500ff") + turretBuilding.upgradesRequirements[turretBuilding.currentLevel].GetRequiresString(turretBuilding.character.inventory) + "\n");

        lastButton = subMenu.AddComponent<EventsCall>().Set("Elegir", action, "");
    }

    void DestroyLastButton()
    {
        if (lastButton != null)
            Object.Destroy(lastButton.gameObject);
    }

    void ImproveDamage(EntityBase item, Recipes requirement)
    {
        if (requirement.CanCraft(turretBuilding.character.inventory))
        {
            requirement.Craft(turretBuilding.character.inventory);
            turretBuilding.ChangeStructure(item);
            turretBuilding.UpgradeLevel();
            subMenu.SetActiveGameObject(false);
        }
        else
            MenuManager.instance.modulesMenu.ObtainMenu<PopUp>(false).SetActiveGameObject(true).SetWindow("", "No tienes los recursos necesarios").AddButton("Cerrar", () => MenuManager.instance.modulesMenu.ObtainMenu<PopUp>(false));
    }

    bool AddAbility(int index, Recipes requirement)
    {
        if (requirement.CanCraft(turretBuilding.character.inventory))
        {
            requirement.Craft(turretBuilding.character.inventory);

            turretBuilding.SetKataCombo(index);
            /*
            foreach (var item in turretBuilding.interact)
            {
                if (item.key == "Construir")
                    item.key = "Mejorar";
            }
            */
            turretBuilding.UpgradeLevel();
            subMenu.SetActiveGameObject(false);
            
            return true;
        }
        else
        {
            MenuManager.instance.modulesMenu.ObtainMenu<PopUp>(false).SetActiveGameObject(true).SetWindow("", "No tienes los recursos necesarios").AddButton("Cerrar", () => MenuManager.instance.modulesMenu.ObtainMenu<PopUp>(false));
            return false;
        }
    }
}
