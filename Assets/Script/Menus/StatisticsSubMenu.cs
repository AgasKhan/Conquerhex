using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable]
public class StatisticsSubMenu : CreateSubMenu
{
    Character character;

    [SerializeField]
    InventorySubMenu inventorySubMenu;
    public override void Create(Character _character)
    {
        character = _character;
        base.Create(_character);
    }

    protected override void InternalCreate()
    {
        subMenu.navbar.DestroyAll();

        subMenu.AddNavBarButton("Equipamiento", ()=> { Create(character); inventorySubMenu.slotItem = null; }).AddNavBarButton("Inventario", ()=>CreateInventory(character));

        subMenu.ClearBody();
        /*

        subMenu.CreateSection(0, 2);
        subMenu.CreateChildrenSection<ScrollRect>();
        subMenu.AddComponent<DetailsWindow>().SetImage(character.flyweight.image).SetTexts(character.flyweight.nameDisplay, character.flyweight.GetDetails().ToString("\n"));

        subMenu.CreateSection(2, 6);
        subMenu.CreateChildrenSection<ScrollRect>();

        CreateEquipmentButtons(character);
        */

        subMenu.CreateSection(0, 2);
        subMenu.CreateChildrenSection<ScrollRect>();
        subMenu.AddComponent<DetailsWindow>().SetTexts("Básicos", "Ataque básico: click izq \nHabilidad basica: Click der\nHabilidad Alternativa: shift izquierdo\nAlgunas habilidades apuntaran en direccion del mouse y otras dependeran del movimiento");
        //CreateEquipmentWeapons(character);
        CreateBasicEquipament(character);

        subMenu.CreateSection(3, 5);
        subMenu.CreateChildrenSection<ScrollRect>();
        subMenu.AddComponent<DetailsWindow>().SetTexts("¿Combinación de teclas?", "Para ejecutar cualquier habilidad se debera presionar unas teclas de movimiento +  Click..\n" +
            "Primera:   ↑↑ + Click\n" +
            "Segunda:   →→ + Click\n" +
            "Tercera:   ←← + Click\n" +
            "Cuarta:    ↓↓ + Click\n");

        subMenu.CreateSection(5, 8);
        subMenu.CreateChildrenSection<ScrollRect>();
        subMenu.AddComponent<DetailsWindow>().SetTexts("Habilidades", "Combinación de teclas (movimiento) +  Click der");
        CreateEquipamentAbilities(character);

        subMenu.CreateSection(8, 12);
        subMenu.CreateChildrenSection<ScrollRect>();
        subMenu.AddComponent<DetailsWindow>().SetTexts("(Katas) Movimientos ofensivos", "Combinacion de teclas (movimiento) +  Click izq");
        CreateEquipamentKatas(character);

        subMenu.OnClose += Exit;
    }

    public void Exit()
    {
        subMenu.ExitSubmenu();
    }

    public void TriggerMyOnClose()
    {
        subMenu.TriggerOnClose();
    }

    void CreateInventory(Character _character)
    {
        inventorySubMenu.Create(_character);
        //subMenu.OnClose -= Exit;
        //subMenu.OnClose += SubMenuOnClose;
    }

    void CreateEquipInventory(Character _character)
    {
        inventorySubMenu.Create(_character);
        subMenu.OnClose -= Exit;
        subMenu.OnClose += SubMenuOnClose;
    }

    private void SubMenuOnClose()
    {
        subMenu.OnClose -= SubMenuOnClose;
        Create(character);
    }

    void CreateBasicEquipament(Character charac)
    {
        CreateGenericButton(charac.caster.weapons[0], "Arma",
         (_slotItem, _index) =>
         {
             _slotItem.indexEquipedItem = _index;
             subMenu.TriggerOnClose();
         });

        CreateGenericButton(charac.caster.abilities[0], "Habilidad",
            (_slotItem, _index) =>
            {
                var abilityCopy = ((AbilityExtCast)_slotItem.inventoryComponent[_index]).CreateCopy(out int indexCopy);
                _slotItem.indexEquipedItem = indexCopy;
                subMenu.TriggerOnClose();
            });

        CreateGenericButton(charac.caster.abilities[1], "Habilidad alternativa",
        (_slotItem, _index) =>
        {
            var abilityCopy = ((AbilityExtCast)_slotItem.inventoryComponent[_index]).CreateCopy(out int indexCopy);
            _slotItem.indexEquipedItem = indexCopy;
            subMenu.TriggerOnClose();
        });
    }

    void CreateEquipmentWeapons(Character charac)
    {
        for (int i = 0; i < charac.caster.weapons.Count; i++)
        {
            CreateGenericButton(charac.caster.weapons[i], "Equipar Arma",
            (_slotItem, _index) =>
            {
                _slotItem.indexEquipedItem = _index;
                subMenu.TriggerOnClose();
            });
        }
    }

    void CreateEquipamentKatas(Character charac)
    {
        for (int i = 0; i < charac.caster.katasCombo.Count; i++)
        {
            CreateKataCombosButtons(charac.caster.katasCombo[i]);
        }
    }

    void CreateEquipamentAbilities(Character charac)
    {
        //for (int i = 0; i < charac.caster.abilities.Count; i++)
        for (int i = 2; i < charac.caster.abilities.Count; i++)
        {
            CreateGenericButton(charac.caster.abilities[i], "Equipar Habilidad",
            (_slotItem, _index) =>
            {
                var abilityCopy = ((AbilityExtCast)_slotItem.inventoryComponent[_index]).CreateCopy(out int indexCopy);
                _slotItem.indexEquipedItem = indexCopy;
                subMenu.TriggerOnClose();
            });
        }
    }

    void CreateEquipmentButtons(Character charac)
    {
        CreateEquipmentWeapons(charac);
        CreateEquipamentKatas(charac);
        CreateEquipamentAbilities(charac);
    }

    void EquipWeaponAction(SlotItem _slotItem, int _index)
    {
        (_slotItem as SlotItem<WeaponKata>).equiped.ChangeWeapon(_slotItem.inventoryComponent[_index]);
        subMenu.TriggerOnClose();
    }

    void EquipKataAction(SlotItem _slotItem, int _index)
    {
        ((WeaponKata)_slotItem.inventoryComponent[_index]).CreateCopy(out int indexCopy);
        _slotItem.indexEquipedItem = indexCopy;
        subMenu.TriggerOnClose();
    }

    void CreateGenericButton<T>(SlotItem<T> item, string defaultName, System.Action<SlotItem, int> equipAction) where T : Item
    {
        var info = new SlotInfo(defaultName, null, "", typeof(T));

        UnityAction action = () =>
        {
            inventorySubMenu.SetEquipMenu<MeleeWeapon>(item, info.filter, equipAction);
            CreateEquipInventory(character);
        };

        if (item.equiped != null)
        {
            info.name = item.equiped.nameDisplay;
            info.sprite = item.equiped.image;

            if(item.equiped is MeleeWeapon)
                info.str = "Usos: " + (item.equiped as MeleeWeapon).current;
        }

        subMenu.AddComponent<ButtonA>().SetButtonA(info.name, info.sprite, info.str, action);
    }

    void CreateKataCombosButtons(SlotItem<WeaponKata> kata)
    {
        var infoKata = new SlotInfo("Equipar Kata", null, "", typeof(WeaponKata));

        UnityAction actionKata;

        var infoWeapon = new SlotInfo("Equipar Arma", null, "", typeof(MeleeWeapon));

        UnityAction actionWeapon;

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

        actionKata = () =>
        {
            inventorySubMenu.SetEquipMenu<WeaponKata>(kata, infoKata.filter, EquipKataAction);
            CreateEquipInventory(character);
        };

        actionWeapon = () =>
        {
            inventorySubMenu.SetEquipMenu<WeaponKata>(kata, infoWeapon.filter, EquipWeaponAction);
            CreateEquipInventory(character);
        };


        var doubleButton = subMenu.AddComponent<DoubleButtonA>();
        doubleButton.left.SetButtonA(infoKata.name, infoKata.sprite, infoKata.str, actionKata);
        doubleButton.right.SetButtonA(infoWeapon.name, infoWeapon.sprite, infoWeapon.str, actionWeapon).button.interactable = interactiveWeap;
    }
}

public struct SlotInfo
{
    public string name;
    public Sprite sprite;
    public string str;
    public Type filter;

    public SlotInfo(string name, Sprite sprite, string str, Type filter)
    {
        this.name = name;
        this.sprite = sprite;
        this.str = str;
        this.filter = filter;
    }
}