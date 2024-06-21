using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BloodTreeBuild : Building
{
    //public Pictionarys<ItemCrafteable, GachaRarity> possibleRewards = new Pictionarys<ItemCrafteable, GachaRarity>();
    public int rewardsQuantity = 3;
    //public override List<ItemCrafteable> currentRecipes => recipes;

    Pictionarys<ItemCrafteable, GachaRarity> possibleRewards;
    Pictionarys<ItemCrafteable, int> gachaRewardsInt = new Pictionarys<ItemCrafteable, int>();
    //List<ItemCrafteable> recipes = new List<ItemCrafteable>();

    [SerializeField]
    Character[] minions;

    [SerializeField]
    SpriteRenderer sprite;

    Hexagone[] originalTp = new Hexagone[6];

    Hexagone[] encerradoTp = new Hexagone[6];

    bool encerrado = false;

    CraftingBuild craftReference;

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
        possibleRewards = flyweight.GetFlyWeight<PoolRewardsBase>().possibleRewards;

        LoadSystem.AddPostLoadCorutine(PostAwake, 1000);
    }

    void PostAwake()
    {
        for (int i = 0; i < encerradoTp.Length; i++)
        {
            encerradoTp[i] = hexagoneParent;
        }

        hexagoneParent.ladosArray.CopyTo(originalTp, 0);

        minions = hexagoneParent.gameObject.GetComponentsInChildren<Character>(true).Where((m) => m.team != Team.player).ToArray();

        for (int i = 0; i < minions.Length; i++)
        {
            ((IAFather)minions[i].CurrentState).detect += Encerrar;
        }

        craftReference = CraftingBuild.instance; 
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


    public void SetRewards()
    {
        gachaRewardsInt.Clear();

        foreach (var item in possibleRewards)
        {
            gachaRewardsInt.Add(item.key, (int)item.value);
        }

        foreach (var item in craftReference.currentRecipes)
        {
            if(gachaRewardsInt.ContainsKey(item))
            {
                //gachaRewardsInt[(ItemCrafteable)item.GetItemBase()] = (int)GachaRarity.S;
                gachaRewardsInt.Remove(item);
            }
        }

        if (gachaRewardsInt.Count > 0)
        {
            List<ItemCrafteable> newRecipes = new List<ItemCrafteable>(); 
            for (int i = 0; i < rewardsQuantity; i++)
            {
                if (gachaRewardsInt.Count <= 0)
                    break;

                var aux = gachaRewardsInt.RandomPic();
                craftReference.AddRecipe(aux);
                newRecipes.Add(aux);
                gachaRewardsInt.Remove(aux);
            }

            string recipesStr = "";
            foreach (var item in newRecipes)
            {
                recipesStr += item.nameDisplay + " ";
            }

            MenuManager.instance.modulesMenu.ObtainMenu<PopUp>(false).SetActiveGameObject(true)
                    .SetWindow((newRecipes.Count > 1? "¡Nuevas recetas desbloqueadas!" : "¡Nueva receta desbloqueada!"),
                    recipesStr.RichTextColor(Color.cyan) 
                    + (newRecipes.Count > 1 ? "\nVuelve a la mesa de crafteo para ver tus nuevas recetas" : "\nVuelve a la mesa de crafteo para ver tu nueva receta"))
                    .AddButton("Aceptar", () => { GameManager.instance.Menu(false); MenuManager.instance.modulesMenu.ObtainMenu<PopUp>(false).SetActiveGameObject(false); });
        }
        else
        {
            var aux = possibleRewards.keys[Random.Range(0, possibleRewards.Count)].ingredients;
            var randomItem = aux[Random.Range(0, aux.Count)].Item;
            int randomAmount = Random.Range(5, 15);
            interactComp.lastCharInteract.inventory.AddItem(randomItem, randomAmount);
            MenuManager.instance.modulesMenu.ObtainMenu<PopUp>(false).SetActiveGameObject(true)
                    .SetWindow("Felicidades", "Ya obtuviste todas las recetas\nHas obtenido: " + randomItem.nameDisplay.RichTextColor(Color.cyan) + " x ".RichTextColor(Color.cyan) + (randomAmount.ToString().RichTextColor(Color.cyan)) + " como compensación")
                    .AddButton("Aceptar", () => { GameManager.instance.Menu(false); MenuManager.instance.modulesMenu.ObtainMenu<PopUp>(false).SetActiveGameObject(false); });
        }

        interactComp.ChangeInteract(false);
        /*
        interactComp.lastCharInteract.inventory.AddItem(recipes[0], 1);
        MenuManager.instance.modulesMenu.ObtainMenu<PopUp>(false).SetActiveGameObject(true)
                .SetWindow("Felicidades", "Has obtenido: \n" + recipes[0].nameDisplay)
                .AddButton("Aceptar", () => { GameManager.instance.Menu(false); MenuManager.instance.modulesMenu.ObtainMenu<PopUp>(false).SetActiveGameObject(false); });

        interactComp.ChangeInteract(false);
        */

        interactComp.OnInteract -= SetRewards;
    }

    private void Health_noLife()
    {
        interactComp.ChangeInteract(true);
        team = Team.noTeam;

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
}
