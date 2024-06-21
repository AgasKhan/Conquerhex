using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BaseData/RewardsPool", fileName = "New Rewards Pool")]
public class PoolRewardsBase : FlyWeight<EntityBase>
{
    public Pictionarys<ItemCrafteable, GachaRarity> possibleRewards = new Pictionarys<ItemCrafteable, GachaRarity>();
}
