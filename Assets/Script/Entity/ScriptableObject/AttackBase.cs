using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BaseData/AttackBase", fileName = "new AttackBase")]
public class AttackBase : FlyWeight<EntityBase>
{
    [Header("Energia")]
    public int energy = 100;

    [Tooltip("Cuanta energia por segundo es la que gana/pierde")]
    public float chargeEnergy = 1;

    [Tooltip("en caso de verdadero, no tendera a ningun numero la barra")]
    public bool energyStatic;

    [Range(0,1), Tooltip("A que valor tendera por defecto\n 0.5 para la mitad de la barra")]
    public float energyDefault;

    [Header("KataCombos")]

    public WeaponKataCombo[] kataCombos;

    [Header("Habilidades")]

    public AbilityToEquip[] abilities;

    [Header("Especialization")]

    public Damage[] additiveDamage;
}

[System.Serializable]
public class WeaponKataCombo
{
    public int indexToEquip = -1;

    public bool isModifiable = true;

    public bool isDefault = false;

    public WeaponKataBase kata;

    public MeleeWeaponBase weapon;
}

[System.Serializable]
public class AbilityToEquip
{
    public int indexToEquip = -1;

    public bool isModifiable = true;

    public bool isDefault = false;

    public AbilityExtCastBase ability;
}