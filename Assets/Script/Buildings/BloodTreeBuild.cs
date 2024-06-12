using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BloodTreeBuild : CraftingBuild
{
    public Pictionarys<ItemCrafteable, GachaRarity> possibleRewards = new Pictionarys<ItemCrafteable, GachaRarity>();
    public int rewardsQuantity = 3;
    public override List<ItemCrafteable> currentRecipes => recipes;

    Pictionarys<ItemCrafteable, int> gachaRewardsInt = new Pictionarys<ItemCrafteable, int>();
    List<ItemCrafteable> recipes = new List<ItemCrafteable>();

    Character[] minions;

    [SerializeField]
    SpriteRenderer sprite;

    Hexagone[] originalTp = new Hexagone[6];

    Hexagone[] encerradoTp = new Hexagone[6];

    bool encerrado = false;

    protected override void Config()
    {
        base.Config();

        MyAwakes += MyAwake;
    }

    void MyAwake()
    {
        //interactComp.OnInteract += SetRewards;
        health.noLife += Health_noLife;
        health.death += Health_noLife;

        LoadSystem.AddPostLoadCorutine(PostAwake);
    }

    void PostAwake()
    {
        for (int i = 0; i < encerradoTp.Length; i++)
        {
            encerradoTp[i] = hexagoneParent;
        }

        minions = hexagoneParent.gameObject.GetComponentsInChildren<Character>().Where((m)=>m.team!=Team.player).ToArray();

        for (int i = 0; i < minions.Length; i++)
        {
            ((IAFather)minions[i].CurrentState).detect += Encerrar;
        }

        hexagoneParent.ladosArray.CopyTo(originalTp, 0);
    }

    [ContextMenu("Encerrar")]
    public void Encerrar()
    {
        if (encerrado || GameManager.instance.playerCharacter.hexagoneParent != hexagoneParent)
            return;

        for (int i = 0; i < minions.Length; i++)
        {
            UI.Interfaz.instance.PopText(minions[i], "!".RichTextColor(Color.red).RichText("size", 50.ToString()), Vector2.up);
        }

        UI.Interfaz.instance["Titulo"].ShowMsg("Encerrado".RichTextColor(Color.red));
        encerradoTp.CopyTo(hexagoneParent.ladosArray, 0);

        GameManager.instance.playerCharacter.move.Teleport(hexagoneParent, 0);

        encerrado = true;
    }

    [ContextMenu("Liberar")]
    public void Liberar()
    {
        originalTp.CopyTo(hexagoneParent.ladosArray, 0);

        GameManager.instance.playerCharacter.move.Teleport(hexagoneParent, 0);

        UI.Interfaz.instance["Titulo"].ShowMsg("Liberado".RichTextColor(Color.cyan));
    }

    private void Health_noLife()
    {
        interactComp.ChangeInteract(true);
        team = Team.player;

        for (int i = 0; i < minions.Length; i++)
        {
            //UI.Interfaz.instance.PopText(minions[i], "Apagado".RichText("size", "35").RichTextColor(Color.red), Vector2.up * 2);
            
            minions[i].StopIA();

            //minions[i].CurrentState = null;
        }

        UI.Interfaz.instance.PopText(this, "Conquistado".RichText("size", "35").RichTextColor(Color.green), Vector2.up * 2);

        sprite.color = Color.green;

        Liberar();
    }

    

    public void SetRewards()
    {
        gachaRewardsInt.Clear();

        foreach (var item in possibleRewards)
        {
            gachaRewardsInt.Add(item.key, (int)item.value);
        }

        foreach (var item in interactComp.lastCharInteract.inventory)
        {
            if(item.GetItemBase() is ItemCrafteable && gachaRewardsInt.ContainsKey(item.GetItemBase() as ItemCrafteable))
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
        //-----------------------------------------------
        interactComp.lastCharInteract.inventory.AddItem(recipes[0], 1);
        MenuManager.instance.modulesMenu.ObtainMenu<PopUp>(false).SetActiveGameObject(true)
                .SetWindow("Felicidades", "Has obtenido: \n" + recipes[0].nameDisplay)
                .AddButton("Aceptar", () => { GameManager.instance.Menu(false); MenuManager.instance.modulesMenu.ObtainMenu<PopUp>(false).SetActiveGameObject(false); });

        interactComp.ChangeInteract(false);
        //interactComp.OnInteract -= SetRewards;
    }
}
