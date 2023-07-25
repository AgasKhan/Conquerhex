using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurretBuild : Building
{
    public SpriteRenderer originalSprite;
    public Sprite newSprite;

    public StructureBase[] damagesUpgrades;
    public Pictionarys<string, Sprite[]> possibleAbilities = new Pictionarys<string, Sprite[]>();

    TurretSubMenu myTurretSubmenu;
    [HideInInspector]
    public string originalAbility;

    public override string rewardNextLevel => throw new System.NotImplementedException();

    protected override void Config()
    {
        base.Config();

        MyAwakes += MyAwake;
    }
    void MyAwake()
    {
        myTurretSubmenu = new TurretSubMenu(this);
    }

    public override void EnterBuild()
    {
        base.EnterBuild();
        //originalSprite.sprite = newSprite;

        if(currentLevel < maxLevel)
            myTurretSubmenu.Create();
        else
            MenuManager.instance.modulesMenu.ObtainMenu<PopUp>(false).SetActiveGameObject(true).SetWindow("", "La torreta alcanzó el nivel máximo").AddButton("Cerrar", () => MenuManager.instance.modulesMenu.ObtainMenu<PopUp>(false));
    }

    public override void UpgradeLevel()
    {
        
        
        base.UpgradeLevel();
    }

    public void ChangeStructure(StructureBase newStructure)
    {
        flyweight = newStructure;
    }
}

[System.Serializable]
public class TurretSubMenu : CreateSubMenu
{
    TurretBuild turretBuilding;
    EventsCall lastButton;
    DetailsWindow myDetailsW;
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
        myDetailsW = subMenu.AddComponent<DetailsWindow>().SetTexts("Elige una habilidad", "\nSelecciona una habilidad de la izquierda para ver más detalles de la misma, luego elige una, pero hazlo con cuidado no olvides que "+ "sólo puedes escoger una mejora y será permanente\n\n".RichText("color", "#ff0000ff") + "Costo:\n".RichText("color", "#ffa500ff") + turretBuilding.upgradesRequirements[turretBuilding.currentLevel].GetRequiresString(turretBuilding.character) + "\n");

        subMenu.CreateTitle("Torreta");
    }

    void CreateButtons()
    {
        if(turretBuilding.currentLevel == 0)
        {
            foreach (var item in turretBuilding.flyweight.kataCombos)
            {
                subMenu.AddComponent<EventsCall>().Set(item.kata.nameDisplay, () => { ButtonAbility(item.kata); turretBuilding.originalAbility = item.kata.nameDisplay; }, "").rectTransform.sizeDelta = new Vector2(300, 75);
            }
        }
        else
        {
            foreach (var item in turretBuilding.flyweight.kataCombos)
            {
                /*
                if (item.kata == turretBuilding.ActualKata)
                    continue;
                */
                //subMenu.AddComponent<EventsCall>().Set(item.nameDisplay, () => { }, "").rectTransform.sizeDelta = new Vector2(300, 75);
                subMenu.AddComponent<EventsCall>().Set(item.kata.nameDisplay, () => { ButtonAbility(item.kata); TurretMaxLevel(); }, "").rectTransform.sizeDelta = new Vector2(300, 75);
            }

            foreach (var item in turretBuilding.damagesUpgrades)
            {
                subMenu.AddComponent<EventsCall>().Set(item.nameDisplay, ()=> { ButtonDamage(item); TurretMaxLevel();  }, "").rectTransform.sizeDelta = new Vector2(300, 75);
            }
        }
    }

    void TurretMaxLevel()
    {
        foreach (var item in turretBuilding.interact)
        {
            if (item.key == "Mejorar")
                item.key = "Nivel Máximo";
        }

        //Camiar Sprite a nivel maximo con "originalAbility"
    }
    void ButtonAbility(WeaponKataBase item)
    {
        DestroyLastButton();
        myDetailsW.SetTexts(item.nameDisplay, "\n" + item.GetDetails().ToString() + "Solo puedes elegir una habilidad y sera permanente".RichText("color", "#ff0000ff") + "\nCosto: \n".RichText("color", "#ffa500ff") + turretBuilding.upgradesRequirements[turretBuilding.currentLevel].GetRequiresString(turretBuilding.character) + "\n");
        //myDetailsW.SetImage(item.image);
        lastButton = subMenu.AddComponent<EventsCall>().Set("Elegir Habilidad", () => AddAbility(item, turretBuilding.upgradesRequirements[turretBuilding.currentLevel]), "");
    }

    void ButtonDamage(StructureBase item)
    {
        DestroyLastButton();
        myDetailsW.SetTexts(item.nameDisplay, "\n" + item.GetDetails().ToString() + "Solo puedes elegir una mejora y sera permanente".RichText("color", "#ff0000ff") + "\nCosto: \n".RichText("color", "#ffa500ff") + turretBuilding.upgradesRequirements[turretBuilding.currentLevel].GetRequiresString(turretBuilding.character) + "\n");
        //myDetailsW.SetImage(item.image);
        lastButton = subMenu.AddComponent<EventsCall>().Set("Elegir Mejora", () => { ImproveDamage(item, turretBuilding.upgradesRequirements[turretBuilding.currentLevel]); }, "");
    }
    
    void DestroyLastButton()
    {
        if (lastButton != null)
            Object.Destroy(lastButton.gameObject);
    }

    void ImproveDamage(StructureBase item, Recipes requirement)
    {
        if (requirement.CanCraft(turretBuilding.character))
        {
            requirement.Craft(turretBuilding.character);
            turretBuilding.ChangeStructure(item);
            turretBuilding.UpgradeLevel();
            subMenu.SetActiveGameObject(false);
        }
        else
            MenuManager.instance.modulesMenu.ObtainMenu<PopUp>(false).SetActiveGameObject(true).SetWindow("", "No tienes los recursos necesarios").AddButton("Cerrar", () => MenuManager.instance.modulesMenu.ObtainMenu<PopUp>(false));
    }

    void AddAbility(WeaponKataBase ability, Recipes requirement)
    {
        if (requirement.CanCraft(turretBuilding.character))
        {
            requirement.Craft(turretBuilding.character);

            foreach (var item in turretBuilding.interact)
            {
                if (item.key == "Construir")
                    item.key = "Mejorar";
            }

            //Cambiar Sprite
            turretBuilding.UpgradeLevel();
            subMenu.SetActiveGameObject(false);
        }
        else
            MenuManager.instance.modulesMenu.ObtainMenu<PopUp>(false).SetActiveGameObject(true).SetWindow("", "No tienes los recursos necesarios").AddButton("Cerrar", () => MenuManager.instance.modulesMenu.ObtainMenu<PopUp>(false));
    }

    public TurretSubMenu(TurretBuild _portalBuilding)
    {
        turretBuilding = _portalBuilding;
    }
}