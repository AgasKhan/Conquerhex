using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodTreeBuild : CraftingBuild
{
    public Pictionarys<ItemCrafteable, GachaRarity> possibleRewards = new Pictionarys<ItemCrafteable, GachaRarity>();
    public int rewardsQuantity = 3;
    public override List<ItemCrafteable> currentRecipes => recipes;

    public Pictionarys<ItemCrafteable, int> gachaRewardsInt = new Pictionarys<ItemCrafteable, int>();
    List<ItemCrafteable> recipes = new List<ItemCrafteable>();

    protected override void Config()
    {
        base.Config();

        MyAwakes += MyAwake;
    }

    void MyAwake()
    {
        interactComp.OnInteract += SetRewards;
        health.noLife += Health_noLife;
    }

    private void Health_noLife()
    {
        interactComp.ChangeInteract(true);
        team = Team.player;
    }

    void SetRewards()
    {
        gachaRewardsInt.Clear();

        foreach (var item in possibleRewards)
        {
            gachaRewardsInt.Add(item.key, (int)item.value);
        }

        foreach (var item in interactComp.lastCharInteract.inventory)
        {
            if(gachaRewardsInt.ContainsKey((ItemCrafteable)item.GetItemBase()))
            {
                gachaRewardsInt[(ItemCrafteable)item.GetItemBase()] = (int)GachaRarity.S;
            }
        }
        
        for (int i = 0; i < rewardsQuantity; i++)
        {
            if (gachaRewardsInt.Count <= 0)
                break;
            
            var aux = gachaRewardsInt.RandomPic();
            recipes.Add(aux);
            gachaRewardsInt.Remove(aux);
        }
        
        interactComp.OnInteract -= SetRewards;
    }
}
