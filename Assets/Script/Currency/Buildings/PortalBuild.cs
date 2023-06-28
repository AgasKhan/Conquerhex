using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PortalBuild : Building
{
    public Pictionarys<ShowDetails, Recipes> travelRequire = new Pictionarys<ShowDetails, Recipes>();
    public  override string rewardNextLevel => throw new System.NotImplementedException();
    PortalSubMenu myPortalSubMenu;
    protected override void Config()
    {
        base.Config();

        MyAwakes += MyAwake;
    }
    void MyAwake()
    {
        myPortalSubMenu = new PortalSubMenu(this);
    }

    public override void EnterBuild()
    {
        myPortalSubMenu.Create();
    }
}

[System.Serializable]
public class PortalSubMenu : CreateSubMenu
{
    PortalBuild portalBuilding;
    EventsCall lastButton;
    DetailsWindow myDetailsW;
    public override void Create()
    {
        subMenu = MenuManager.instance.modulesMenu.ObtainMenu<SubMenus>();
        base.Create();
    }
    protected override void InternalCreate()
    {
        subMenu.ClearBody();
        subMenu.navbar.DestroyAll(); 
        DestroyLastButton();

        subMenu.CreateSection(1, 2);
        subMenu.CreateChildrenSection<ScrollRect>();
        CreateButtons();

        subMenu.CreateSection(2, 6);
        myDetailsW = subMenu.AddComponent<DetailsWindow>().SetTexts(portalBuilding.structureBase.nameDisplay, portalBuilding.structureBase.GetDetails().ToString());

    }

    void CreateButtons()
    {
        foreach (var item in portalBuilding.travelRequire)
        {
            subMenu.AddComponent<EventsCall>().Set(item.key.nameDisplay, () => { ButtonAct(item.key, item.value); }, "");
        }
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
        if(requirement.CanCraft(portalBuilding.character))
        {
            requirement.Craft(portalBuilding.character);
            LoadSystem.instance.Load(item.nameDisplay, true);
        }
        else
            MenuManager.instance.modulesMenu.ObtainMenu<PopUp>(false).SetActiveGameObject(true).SetWindow("", "No tienes combustible suficiente").AddButton("Cerrar", () => MenuManager.instance.modulesMenu.ObtainMenu<PopUp>(false));

    }

    public PortalSubMenu (PortalBuild _portalBuilding)
    {
        portalBuilding = _portalBuilding;
    }
}
