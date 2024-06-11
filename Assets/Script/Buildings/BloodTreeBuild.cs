using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BloodTreeBuild : CraftingBuild
{
    public Pictionarys<ItemCrafteable, GachaRarity> possibleRewards = new Pictionarys<ItemCrafteable, GachaRarity>();
    public int rewardsQuantity = 3;
    public override List<ItemCrafteable> currentRecipes => recipes;

    public Pictionarys<ItemCrafteable, int> gachaRewardsInt = new Pictionarys<ItemCrafteable, int>();
    List<ItemCrafteable> recipes = new List<ItemCrafteable>();

    List<Character> minions = new List<Character>();

    [SerializeField]
    SpriteRenderer sprite;
    protected override void Config()
    {
        base.Config();

        MyAwakes += MyAwake;
    }

    void MyAwake()
    {
        interactComp.OnInteract += SetRewards;
        health.noLife += Health_noLife;

        LoadSystem.AddPostLoadCorutine(SetMinios);
    }

    void SetMinios()
    {
        minions = hexagoneParent.gameObject.GetComponentsInChildren<Character>().ToList();
        if (minions.Contains(GameManager.instance.playerCharacter))
            minions.Remove(GameManager.instance.playerCharacter);

    }

    private void Health_noLife()
    {
        interactComp.ChangeInteract(true);
        team = Team.player;

        for (int i = 0; i < minions.Count; i++)
        {
            UI.Interfaz.instance.PopText(minions[i], "Apagado".RichText("size", "35").RichTextColor(Color.red), Vector2.up * 2);
            minions[i].IAOnOff(false);
        }

        UI.Interfaz.instance.PopText(this, "Conquistado".RichText("size", "35").RichTextColor(Color.green), Vector2.up * 2);

        sprite.color = Color.green;
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
