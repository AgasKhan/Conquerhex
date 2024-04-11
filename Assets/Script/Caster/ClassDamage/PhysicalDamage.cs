using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DamageTypes
{
    [CreateAssetMenu(menuName = "Weapons/PhysicalDamage", fileName = "PhysicalDamage")]
    /// <summary>
    /// Fisico - padre de los danios fisicos - en su forma pura hace danio solo a la vida
    /// </summary>
    public class PhysicalDamage : PureDamage
    {
    }
}
