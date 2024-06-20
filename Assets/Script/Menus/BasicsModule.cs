using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class BasicsModule : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI title;

    [SerializeField]
    private TextMeshProUGUI content;

    [SerializeField]
    private ButtonA weapon;

    [SerializeField]
    private ButtonA basicAbility;

    [SerializeField]
    private ButtonA alternativeAbility;

    [SerializeField]
    protected ButtonA[] buttonsA;

    public BasicsModule SetTexts(string _title, string _content)
    {
        title.text = _title;
        content.text = _content;
        return this;
    }
    #region Test
    public ButtonA SetButtonWeapon(string itemName, Sprite sprite, string textNum, UnityEngine.Events.UnityAction action)
    {
        weapon.SetButtonA(itemName, sprite, textNum, action);
        return weapon;
    }

    public ButtonA SetButtonAbilityOne(string itemName, Sprite sprite, string textNum, UnityEngine.Events.UnityAction action)
    {
        basicAbility.SetButtonA(itemName, sprite, textNum, action);
        return basicAbility;
    }

    public ButtonA SetButtonAbilityTwo(string itemName, Sprite sprite, string textNum, UnityEngine.Events.UnityAction action)
    {
        alternativeAbility.SetButtonA(itemName, sprite, textNum, action);
        return alternativeAbility;
    }
    #endregion

    public void SetGenericButtonA<T>(int index, SlotItem<T> item, string defaultName, UnityAction buttonAction) where T : ItemEquipable
    {
        var info = new SlotInfo(buttonsA[index].defaultText, buttonsA[index].defaultImage, "", typeof(T));
        
        if (item.equiped != null)
        {
            info.name = item.equiped.nameDisplay;
            info.sprite = item.equiped.image;

            if (item.equiped is MeleeWeapon)
                info.str = "Usos: " + (item.equiped as MeleeWeapon).current;
        }

        buttonsA[index].SetButtonA(info.name, info.sprite, info.str, buttonAction);

        if (!item.isModifiable)
            buttonsA[index].button.interactable = false;
    }

    public ButtonA ClearButtonA(int index)
    {
        return buttonsA[index].ClearButton();
    }

}
