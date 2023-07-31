using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BaseData/turret", fileName = "new Turret")]
public class TurretStructure : StructureBase
{
    public StructureBase[] damagesUpgrades;

    public Pictionarys<string, Sprite[]> possibleAbilities = new Pictionarys<string, Sprite[]>();
}
