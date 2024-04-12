using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SavingAction : InteractAction<(Character character, string slotName)>
{
    public BaseData baseData;
    public override void Activate((Character character, string slotName) specificParam)
    {
        var customer = specificParam.character;

        baseData.SaveGame(specificParam.slotName);
    }

    public override void InteractInit(InteractEntityComponent _interactComp)
    {
        base.InteractInit(_interactComp);
        subMenu = new GenericSubMenu(_interactComp);
        GenericSubMenu menu = subMenu as GenericSubMenu;

        System.Action<SubMenus> menuAction =
        (internalSubMenu) =>
        {
            internalSubMenu.CreateSection(0, 3);
            internalSubMenu.CreateChildrenSection<ScrollRect>();

            foreach (var item in baseData.savedGames)
            {
                internalSubMenu.AddComponent<EventsCall>().Set("Save game: " + item.key, 
                    () => 
                    { 
                        menu.DestroyLastButtons();
                        menu.detailsWindow.SetTexts("Game name: " + item.key.nameDisplay, "Last modification date: ").SetImage(null);
                        menu.CreateButton("Save Game", () => Activate((menu.myCharacter, item.key.nameDisplay)));
                    }, "").rectTransform.sizeDelta = new Vector2(350, 75);
            }

            internalSubMenu.CreateSection(3, 6);
            internalSubMenu.CreateChildrenSection<ScrollRect>();
            menu.detailsWindow = internalSubMenu.AddComponent<DetailsWindow>().SetTexts("", "").SetImage(null);

            internalSubMenu.CreateTitle("Save Building");
        };
    }
}