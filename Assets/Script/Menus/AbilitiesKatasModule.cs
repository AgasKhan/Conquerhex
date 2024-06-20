using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class AbilitiesKatasModule : BasicsModule
{
    [Space]
    [Header ("Abilities")]

    [SerializeField]
    TextMeshProUGUI abilitiesTitle;
    [SerializeField]
    TextMeshProUGUI abilitiesContent;

    [Space]
    [Header("Katas")]

    [SerializeField]
    TextMeshProUGUI katasTitle;
    [SerializeField]
    TextMeshProUGUI katasContent;
    [SerializeField]
    DoubleButtonA[] katasButtons;

    public void SetKataComboButton(int index, SlotItem<WeaponKata> kata, UnityAction actionKata, UnityAction actionWeapon)
    {
        var infoKata = new SlotInfo("Equipar Kata", katasButtons[index].left.defaultImage, "", typeof(WeaponKata));
        var infoWeapon = new SlotInfo("Equipar Arma", katasButtons[index].right.defaultImage, "", typeof(MeleeWeapon));

        bool interactiveWeap = false;

        if (kata.equiped != null)
        {
            infoKata.name = kata.equiped.nameDisplay;
            infoKata.sprite = kata.equiped.image;
            interactiveWeap = true;

            if (kata.equiped.WeaponEnabled != null)
            {
                infoWeapon.name = kata.equiped.Weapon.nameDisplay;
                infoWeapon.sprite = kata.equiped.Weapon.image;
                infoWeapon.str = "Usos: " + kata.equiped.Weapon.current;
            }
        }

        katasButtons[index].left.SetButtonA(infoKata.name, infoKata.sprite, infoKata.str, actionKata);
        katasButtons[index].right.SetButtonA(infoWeapon.name, infoWeapon.sprite, infoWeapon.str, actionWeapon).button.interactable = interactiveWeap;

        if (!kata.isModifiable)
        {
            katasButtons[index].left.button.interactable = false;
            katasButtons[index].right.button.interactable = false;
        }
    }


    public DoubleButtonA ClearDoubleA(int index)
    {
        return katasButtons[index].ClearDoubleA();
    }
}
