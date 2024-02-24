using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BaseData/AttackBase", fileName = "new AttackBase")]
public class AttackBase : FlyWeight<EntityBase>
{
    [Header("Habilidades")]

    public WeaponKataCombo[] kataCombos;

    [Header("Especialization")]

    public Damage[] additiveDamage;
}

[System.Serializable]
public class WeaponKataCombo
{
    public WeaponKataBase kata;

    public MeleeWeaponBase weapon;
}