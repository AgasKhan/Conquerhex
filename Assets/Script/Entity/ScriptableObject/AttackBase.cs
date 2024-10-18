using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BaseData/AttackBase", fileName = "new AttackBase")]
public class AttackBase : FlyWeight<EntityBase>
{
    [System.Serializable]
    public abstract class ToEquip
    {
        public int indexToEquip = -1;

        public bool isModifiable = true;

        public bool isDefault = false;

        public bool isBlocked = false;
    }

    [System.Serializable]
    public class WeaponToEquip : ToEquip
    {
        public MeleeWeaponBase weapon;
    }

    [System.Serializable]
    public class AbilityCombo : WeaponToEquip
    {
        public AbilityBase ability;
    }

    [System.Serializable]
    public class WeaponKataCombo : WeaponToEquip
    {
        public WeaponKataBase kata;
    }

    [System.Serializable]
    public class AbilityToEquip : ToEquip
    {
        public AbilityExtCastBase ability;
    }

    [System.Serializable]
    public class ItemToEquip : ToEquip
    {
        public MeleeWeaponBase weaponToEquip;
        public AbilityBase itemToEquip;
    }

    [Header("Energia")]
    public int energy = 100;

    [Tooltip("Cuanta energia por segundo es la que gana/pierde")]
    public float chargeEnergy = 1;

    [Tooltip("en caso de verdadero, no tendera a ningun numero la barra")]
    public bool energyStatic;

    [Range(0,1), Tooltip("A que valor tendera por defecto\n 0.5 para la mitad de la barra")]
    public float energyDefault;

    [Header("Arma por defecto")]
    public WeaponToEquip weaponToEquip;

    [Header("KataCombos")]

    public WeaponKataCombo[] kataCombos;

    [Header("Habilidades")]

    public AbilityToEquip[] abilities;

    [Header("Combos de ataque")]
    public AbilityCombo[] combos;

    [Header("Test de habilidades")]
    public ItemToEquip[] testAbilities;

    [Header("Especialization")]

    public Damage[] additiveDamage;
}

