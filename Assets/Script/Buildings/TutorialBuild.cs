using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialBuild : Building
{
    public List<ShowDetails> allTutorials = new List<ShowDetails>();
    public override string rewardNextLevel => throw new System.NotImplementedException();
}

[System.Serializable]
public class TutorialSubMenu : CreateSubMenu
{
    TutorialBuild tutorialBuilding;
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
        myDetailsW = subMenu.AddComponent<DetailsWindow>().SetTexts("", "\nEscoge la simulación que desees vivir\n\n");

        subMenu.CreateTitle("Elige la simulación");
    }

    void CreateButtons()
    {
        foreach (var item in tutorialBuilding.allTutorials)
        {
            subMenu.AddComponent<EventsCall>().Set(item.nameDisplay, () => { ButtonAct(item); }, "").rectTransform.sizeDelta = new Vector2(300, 75);
        }
    }

    void ButtonAct(ShowDetails item)
    {
        DestroyLastButton();
        myDetailsW.SetTexts(item.nameDisplay, item.GetDetails().ToString());
        myDetailsW.SetImage(item.image);
        lastButton = subMenu.AddComponent<EventsCall>().Set("Iniciar", () => { LoadSystem.instance.Load(item.nameDisplay, true); }, "");
    }
    void DestroyLastButton()
    {
        if (lastButton != null)
            Object.Destroy(lastButton.gameObject);
    }

    public TutorialSubMenu(TutorialBuild _portalBuilding)
    {
        tutorialBuilding = _portalBuilding;
    }
}
