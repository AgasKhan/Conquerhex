using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PortalBuild : Building
{
    public Pictionarys<ShowDetails, Recipes> travelRequire = new Pictionarys<ShowDetails, Recipes>();
    public  override string rewardNextLevel => throw new System.NotImplementedException();
    PortalSubMenu myPortalSubMenu;

    /*
    protected override void Config()
    {
        base.Config();

        MyAwakes += MyAwake;
    }
    
    void MyAwake()
    {
        myPortalSubMenu = new PortalSubMenu(this);
    }
    */
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
        foreach (var item in portalBuilding.travelRequire)
        {
            subMenu.AddComponent<EventsCall>().Set(item.key.nameDisplay, () => { ButtonAct(item.key, item.value); }, "").rectTransform.sizeDelta = new Vector2(300, 75);
        }
    }

    void ButtonAct(ShowDetails item, Recipes requirement)
    {
        DestroyLastButton();
        //myDetailsW.SetTexts(item.nameDisplay, item.GetDetails().ToString() + "Costo del viaje: \n" + requirement.GetRequiresString(portalBuilding.character.inventory));
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
        //if(requirement.CanCraft(portalBuilding.character.inventory))
        //{
        //    requirement.Craft(portalBuilding.character.inventory);
        //    LoadSystem.instance.LoadAndSavePlayer(item.nameDisplay, true);
        //}
        //else
        //    MenuManager.instance.modulesMenu.ObtainMenu<PopUp>(false).SetActiveGameObject(true).SetWindow("", "No tienes combustible suficiente").AddButton("Cerrar", () => MenuManager.instance.modulesMenu.ObtainMenu<PopUp>(false));

    }

    public PortalSubMenu (PortalBuild _portalBuilding)
    {
        portalBuilding = _portalBuilding;
    }
}
