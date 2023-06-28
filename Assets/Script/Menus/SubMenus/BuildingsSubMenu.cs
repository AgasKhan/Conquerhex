using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class BuildingsSubMenu : CreateSubMenu
{
    Building buildingBase;

    [HideInInspector]
    public DetailsWindow detailsWindow;

    EventsCall lastButton = null;

    public override void Create()
    {
        subMenu = MenuManager.instance.modulesMenu.ObtainMenu<SubMenus>();

        subMenu.navbar.DestroyAll();
        subMenu.ClearBody();
        DestroyCraftButtons();
        base.Create();
    }
    protected override void InternalCreate()
    {
        subMenu.CreateSection(0, 2);
        subMenu.CreateChildrenSection<ScrollRect>();

        foreach (var item in buildingBase.interact)
        {
            subMenu.AddComponent<EventsCall>().Set(item.key, () => { item.value.Activate(buildingBase); }, "").rectTransform.sizeDelta = new Vector2(300, 75);
        }

        subMenu.CreateSection(2, 6);
        subMenu.CreateChildrenSection<ScrollRect>();
        detailsWindow = subMenu.AddComponent<DetailsWindow>().SetTexts("", buildingBase.structureBase.GetDetails()["Description"]).SetImage(buildingBase.structureBase.image);

        subMenu.CreateTitle(buildingBase.name);
    }

    public EventsCall CreateButton(string text, UnityEngine.Events.UnityAction action)
    {
        DestroyCraftButtons();
        lastButton = subMenu.AddComponent<EventsCall>().Set(text, action, "");
        return lastButton;
    }

    public void DestroyCraftButtons()
    {
        if (lastButton != null)
            Object.Destroy(lastButton.gameObject);
    }

    public BuildingsSubMenu(Building _buildingBase)
    {
        buildingBase = _buildingBase;
    }
}
