using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSelectSubMenu : CreateSubMenu
{
    Character myCharacter;
    List<ButtonA> buttonsList = new List<ButtonA>();
    public override void Create(Character character)
    {
        myCharacter = character;
        subMenu = MenuManager.instance.modulesMenu.ObtainMenu<SubMenus>();

        subMenu.navbar.DestroyAll();
        subMenu.ClearBody();

        base.Create();
    }
    protected override void InternalCreate()
    {
        subMenu.CreateSection(0, 2);
        subMenu.CreateChildrenSection<ScrollRect>();

        var aux = myCharacter.GetInContainer<CasterEntityComponent>().abilities;

        for (int i = 0; i < aux.Count; i++)
        {
            //var item = (ItemBase)aux[i];
            //subMenu.AddComponent<EventsCall>().Set(item.key.Name, () => { DestroyLastButtons(); item.value.ShowMenu(myCharacter); }, "").rectTransform.sizeDelta = new Vector2(300, 75);
        }

        //foreach (var item in myCharacter.GetInContainer<CasterEntityComponent>().abilitiesCombo)
        //{

        //}

        subMenu.CreateSection(2, 6);
        subMenu.CreateChildrenSection<ScrollRect>();


        //detailsWindow = subMenu.AddComponent<DetailsWindow>().SetTexts("", interactComponent.container.flyweight.GetDetails()["Description"]).SetImage(interactComponent.container.flyweight.image);

        subMenu.CreateTitle("Equipe Items");
    }

    void ClearButtons()
    {
        foreach (var item in buttonsList)
        {
            Object.Destroy(item.gameObject);
        }
        buttonsList.Clear();
    }

    void ButtonAct(WeaponKata kata)
    {
        ClearButtons();

        for (int i = 0; i < myCharacter.inventory.inventory.Count; i++)
        {
            var item = myCharacter.inventory.inventory[i];

            if (item.GetItemBase() is MeleeWeaponBase)
            {
                ButtonA button = subMenu.AddComponent<ButtonA>();
                buttonsList.Add(button.SetButtonA(item.nameDisplay, item.image, "Uses: " + ((MeleeWeapon)item).current, ()=> { EquipWeapon(kata); }).SetType(item.itemType.ToString()));
            }

        }
    }

    void EquipWeapon(WeaponKata kata)
    {

    }

}
