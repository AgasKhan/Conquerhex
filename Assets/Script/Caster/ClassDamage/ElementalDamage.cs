using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DamageTypes
{
    [CreateAssetMenu(menuName = "Weapons/ElementalDamage", fileName = "ElementalDamage")]
    /// <summary>
    /// Elemental - padre de los danios elementales - en su forma pura hace danio solo a la regeneracion
    /// </summary>
    public class ElementalDamage : PureDamage
    {
    }
}