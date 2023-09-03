using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GachaBuild : Building
{
    public Pictionarys<ItemBase, GachaRarity> gachaRewards = new Pictionarys<ItemBase, GachaRarity>();
    public int maxTriesSS = 50;
    public Recipes gachaCost;

    Pictionarys<ItemBase, int> gachaRewardsInt = new Pictionarys<ItemBase, int>();
    string textRewards = "";
    ItemBase lastReward;
    int pitySystem;

    public override void EnterBuild()
    {
        RefreshRewards();
    }

    void RefreshRewards()
    {
        myBuildSubMenu.detailsWindow.SetTexts("Posibles Recompensas", textRewards).SetImage(null);
        
        if (gachaCost.CanCraft(character))
        {
            myBuildSubMenu.CreateButton(gachaCost.GetRequiresString(character), () =>
            {
                myBuildSubMenu.DestroyCraftButtons();
                gachaCost.Craft(character);
                StartCoroutine(ShowRewards());
            });
        }
    }


    protected override void Config()
    {
        base.Config();

        MyAwakes += MyAwake;
    }


    void MyAwake()
    {
        textRewards = "";

        //-----------------------------
        character.AddOrSubstractItems("Coin", 50);
        //-----------------------------

        foreach (var item in gachaRewards)
        {
            switch(item.value)
            {
                case GachaRarity.D :
                    textRewards += item.key.nameDisplay.RichText("color", "#c0c0c0ff") + "\n";
                    break;
                case GachaRarity.C:
                    textRewards += item.key.nameDisplay.RichText("color", "#808000ff") + "\n";
                    break;
                case GachaRarity.B:
                    textRewards += item.key.nameDisplay.RichText("color", "#add8e6ff") + "\n";
                    break;
                case GachaRarity.A:
                    textRewards += item.key.nameDisplay.RichText("color", "#00ff00ff") + "\n";
                    break;
                case GachaRarity.S:
                    textRewards += item.key.nameDisplay.RichText("color", "#00ffffff") + "\n";
                    break;
                case GachaRarity.SS:
                    textRewards += item.key.nameDisplay.RichText("color", "#ff00ffff") + "\n";
                    break;
                default:
                    break;
            }
        }

        GetRewardInt();
    }
    
    public void GetRewardInt()
    {
        gachaRewardsInt.Clear();
        foreach (var item in gachaRewards)
        {
            gachaRewardsInt.Add(item.key, (int)item.value);
        }
    }

    void SetLastReward()
    {
        lastReward = gachaRewardsInt.RandomPic();
        myBuildSubMenu.detailsWindow.SetTexts(lastReward.nameDisplay, "").SetImage(lastReward.image);
    }

    IEnumerator ShowRewards()
    {
        for (int i = 0; i < 5; i++)
        {
            myBuildSubMenu.detailsWindow.SetActive(false);
            yield return null;
            SetLastReward();
            yield return new WaitForSeconds(0.5f);
        }

        if(gachaRewards[lastReward] != GachaRarity.SS)
        {
            pitySystem++;
            if(pitySystem >= maxTriesSS)
            {
                foreach (var item in gachaRewardsInt)
                {
                    if (item.value != (int)GachaRarity.SS)
                        item.value = 0;
                }
                SetLastReward();
                yield return new WaitForSeconds(0.5f);

                pitySystem = 0;
                GetRewardInt();
            }
        }
        else
            pitySystem = 0;

        character.AddOrSubstractItems(lastReward.nameDisplay, 1);

        MenuManager.instance.modulesMenu.ObtainMenu<PopUp>(true).SetWindow("¡Felicidades!", "Has obtenido: " + lastReward.nameDisplay.RichText("color", "#ffff00ff") + ". Este item tiene una probalidad de salir del " + (float)gachaRewards[lastReward]/10f + " %")
            .AddButton("Aceptar", () => { MenuManager.instance.modulesMenu.ObtainMenu<PopUp>(false); RefreshRewards(); });
    }

}

public enum GachaRarity
{
    D = 750,
    C = 500,
    B = 350,
    A = 150,
    S = 70,
    SS = 5
}