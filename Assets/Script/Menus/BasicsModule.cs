using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;

public class BasicsModule : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI title;

    [SerializeField]
    private TextMeshProUGUI content;

    [SerializeField]
    protected ButtonA[] buttonsA;

    [SerializeField]
    LayoutGroup layoutGroup;

    public BasicsModule SetTexts(string _title, string _content)
    {
        title.text = _title;
        content.text = _content;
        return this;
    }

    public void SetGenericButtonA<T>(int index, SlotItem<T> item, string defaultName, UnityAction buttonAction) where T : ItemEquipable
    {
        //GameManager.RetardedOn((_bool) => layoutGroup.SetActive(_bool));
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
