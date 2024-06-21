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

    BasicsModule myBasicsModule;
    AbilitiesKatasModule myAbilityKataModule;
    StatisticsModule myStatisticsModule;
    public override void Create(Character _character)
    {
        character = _character;
        base.Create(_character);
    }
    
    protected override void InternalCreate()
    {
        subMenu.navbar.DestroyAll();

        subMenu.AddNavBarButton("Equipamiento", ()=> { CreateStatsBody() ; inventorySubMenu.slotItem = null; }).AddNavBarButton("Inventario", ()=>CreateInventory(character));

        CreateStatsBody();

        //myStatisticsModule.SetText(character.flyweight.nameDisplay, character.flyweight.GetDetails().ToString());

        /*
        subMenu.CreateSection(0, 2);
        subMenu.CreateChildrenSection<ScrollRect>();
        subMenu.AddComponent<DetailsWindow>().SetTexts("Básicos", "Ataque básico: click izq \nHabilidad basica: Click der\nHabilidad Alternativa: shift izquierdo\nAlgunas habilidades apuntaran en direccion del mouse y otras dependeran del movimiento");
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
        */
    }

    void CreateStatsBody()
    {
        subMenu.ClearBody();

        subMenu.CreateSection(0, 4);
        subMenu.CreateChildrenSection<VerticalLayoutGroup>();
        myBasicsModule = subMenu.AddComponent<BasicsModule>();
        CreateBasicEquipament(character);

        myAbilityKataModule = subMenu.AddComponent<AbilitiesKatasModule>();
        CreateEquipamentAbilities(character);
        CreateEquipamentKatas(character);

        subMenu.CreateSection(4, 6);
        myStatisticsModule = subMenu.AddComponent<StatisticsModule>();
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
        subMenu.OnClose -= subMenu.ExitSubmenu;
        subMenu.OnClose += SubMenuOnClose;
    }

    private void SubMenuOnClose()
    {
        subMenu.OnClose -= SubMenuOnClose;
        Create(character);
    }

    UnityAction WeaponAction(SlotItem<MeleeWeapon> item)
    {
        Action<SlotItem, int> equipAction = (_slotItem, _index) =>
        {
            _slotItem.indexEquipedItem = _index;
            subMenu.TriggerOnClose();
        };

        return () =>
        {
            inventorySubMenu.SetEquipMenu<MeleeWeapon>(item, typeof(MeleeWeapon), equipAction);
            CreateEquipInventory(character);
        };
    }

    UnityAction AbilityAction<T>(SlotItem<T> item) where T : ItemEquipable
    {
        Action<SlotItem, int> equipAction = (_slotItem, _index) =>
        {
            var abilityCopy = ((AbilityExtCast)_slotItem.inventoryComponent[_index]).CreateCopy(out int indexCopy);
            _slotItem.indexEquipedItem = indexCopy;
            subMenu.TriggerOnClose();
        };

        return () =>
        {
            inventorySubMenu.SetEquipMenu<MeleeWeapon>(item, typeof(T), equipAction);
            CreateEquipInventory(character);
        };
    }

    void CreateBasicEquipament(Character charac)
    {
        myBasicsModule.SetGenericButtonA(0, charac.caster.weapons[0], "Arma",
        WeaponAction(charac.caster.weapons[0]));

        myBasicsModule.SetGenericButtonA(1, charac.caster.abilities[0], "Habilidad",
        AbilityAction(charac.caster.abilities[0]));

        myBasicsModule.SetGenericButtonA(2, charac.caster.abilities[1], "Habilidad",
        AbilityAction(charac.caster.abilities[1]));

        /*
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
        */
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
            myAbilityKataModule.SetKataComboButton(i, charac.caster.katasCombo[i],
                KataAction(charac.caster.katasCombo[i]), WeaponOfKataAction(charac.caster.katasCombo[i]));
        }
        /*
        for (int i = 0; i < charac.caster.katasCombo.Count; i++)
        {
            CreateKataCombosButtons(charac.caster.katasCombo[i]);
        }
        */
    }

    void CreateEquipamentAbilities(Character charac)
    {
        for (int i = 2; i < charac.caster.abilities.Count; i++)
        {
            myAbilityKataModule.SetGenericButtonA(i - 2, charac.caster.abilities[i], "Equipar Habilidad",
            AbilityAction(charac.caster.abilities[i]));
        }

        /*
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
        */
    }

    void CreateEquipmentButtons(Character charac)
    {
        CreateEquipmentWeapons(charac);
        CreateEquipamentKatas(charac);
        CreateEquipamentAbilities(charac);
    }

    UnityAction WeaponOfKataAction(SlotItem<WeaponKata> item)
    {
        Action<SlotItem, int> equipAction = (_slotItem, _index) =>
        {
            (_slotItem as SlotItem<WeaponKata>).equiped.ChangeWeapon(_slotItem.inventoryComponent[_index]);
            subMenu.TriggerOnClose();
        };

        return () =>
        {
            inventorySubMenu.SetEquipMenu<MeleeWeapon>(item, typeof(MeleeWeapon), equipAction);
            CreateEquipInventory(character);
        };
    }

    UnityAction KataAction(SlotItem<WeaponKata> item)
    {
        Action<SlotItem, int> equipAction = (_slotItem, _index) =>
        {
            ((WeaponKata)_slotItem.inventoryComponent[_index]).CreateCopy(out int indexCopy);
            _slotItem.indexEquipedItem = indexCopy;
            subMenu.TriggerOnClose();
        };

        return () =>
        {
            inventorySubMenu.SetEquipMenu<MeleeWeapon>(item, typeof(WeaponKata), equipAction);
            CreateEquipInventory(character);
        };
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

    void CreateGenericButton<T>(SlotItem<T> item, string defaultName, System.Action<SlotItem, int> equipAction) where T : ItemEquipable
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

        var button = subMenu.AddComponent<ButtonA>().SetButtonA(info.name, info.sprite, info.str, action);

        if (!item.isModifiable)
            button.button.interactable = false;
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

        if (!kata.isModifiable)
        {
            doubleButton.left.button.interactable = false;
            doubleButton.right.button.interactable = false;
        }
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