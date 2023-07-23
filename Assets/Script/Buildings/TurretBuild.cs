using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurretBuild : Building
{
    public SpriteRenderer originalSprite;
    public Sprite newSprite;

    public StructureBase[] damagesUpgrades;
    public WeaponKataBase[] abilitiesUpgrades;

    public override string rewardNextLevel => throw new System.NotImplementedException();

    public override void EnterBuild()
    {
        base.EnterBuild();
        originalSprite.sprite = newSprite;

    }

    public override void UpgradeLevel()
    {
        base.UpgradeLevel();

    }
}

[System.Serializable]
public class TurretSubMenu : CreateSubMenu
{
    TurretBuild portalBuilding;
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
        myDetailsW = subMenu.AddComponent<DetailsWindow>().SetTexts("", "\nDependiendo del lugar al que quieras viajar el costo cambiará\n\n");

        subMenu.CreateTitle("Elige la ubicación");
    }

    void CreateButtons()
    {
        /*
        foreach (var item in portalBuilding.travelRequire)
        {
            subMenu.AddComponent<EventsCall>().Set(item.key.nameDisplay, () => { ButtonAct(item.key, item.value); }, "").rectTransform.sizeDelta = new Vector2(300, 75);
        }
        */
    }

    void ButtonAct(ShowDetails item, Recipes requirement)
    {
        DestroyLastButton();
        myDetailsW.SetTexts(item.nameDisplay, item.GetDetails().ToString() + "Costo del viaje: \n" + requirement.GetRequiresString(portalBuilding.character));
        myDetailsW.SetImage(item.image);
        lastButton = subMenu.AddComponent<EventsCall>().Set("Viajar", () => { Travel(item, requirement); }, "");
    }
    void DestroyLastButton()
    {
        if (lastButton != null)
            Object.Destroy(lastButton.gameObject);
    }

    void Travel(ShowDetails item, Recipes requirement)
    {
        if (requirement.CanCraft(portalBuilding.character))
        {
            requirement.Craft(portalBuilding.character);
            LoadSystem.instance.Load(item.nameDisplay, true);
        }
        else
            MenuManager.instance.modulesMenu.ObtainMenu<PopUp>(false).SetActiveGameObject(true).SetWindow("", "No tienes combustible suficiente").AddButton("Cerrar", () => MenuManager.instance.modulesMenu.ObtainMenu<PopUp>(false));

    }

    public TurretSubMenu(TurretBuild _portalBuilding)
    {
        portalBuilding = _portalBuilding;
    }
}