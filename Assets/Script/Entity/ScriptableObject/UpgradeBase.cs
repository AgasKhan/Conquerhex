using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BaseData/Upgrades", fileName = "New Upgrade")]
public class UpgradeBase : FlyWeight<EntityBase>
{
    public ItemCrafteable[] upgradesRequirements;

}
