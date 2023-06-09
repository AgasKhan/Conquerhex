using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonsFunc_Hexagonos : ButtonsFunctions
{
    [SerializeField]
    ShowSubMenus controlSubMenus;

    [SerializeField]
    ShowSubMenus lootSubMenus;

    [SerializeField]
    ShowSubMenus objectivesSubMenus;

    protected override void LoadButtons()
    {
        base.LoadButtons();

        MenuManager.instance.eventListVoid.AddRange(new Pictionarys<string, System.Action<GameObject>>()
        {
            //Menu in game
            {"MenuInGame", PauseMenu},

            {"Resume", Resume},
            {"Restart", Restart},
            {"ShowControls", ShowControls},
            {"BackMenu", BackMenu},

            {"MenuDetails", PauseMenu},
            {"CraftingMenu", Crafting},
            //{"InventoryMenu", DisplayWindow},



            //Store
            {"BuySingleItem", BuySingleItem},
            {"EquipItem", EquipItem},

        });
    }

    void PauseMenu(GameObject g)
    {
        /*
        DisplayWindow(g);
        GameManager.instance.TogglePause();
        */
        refMenu.modulesMenu.ObtainMenu<MenuList>(false).SetActiveGameObject(true).CreateDefault();
        GameManager.instance.Pause(true);

        //Despausar
    }

    void Resume(GameObject g)
    {
        refMenu.modulesMenu.ObtainMenu<MenuList>(false);
        GameManager.instance.Pause(false);
    }

    void BackMenu(GameObject g)
    {
        LoadSystem.instance.Load("MainMenu");
        SaveWithJSON.SaveGame();
    }

    void Crafting(GameObject g)
    {
        StoreInteract.instance.ShowCraftingW();
    }

    void BuySingleItem(GameObject g)
    {
        if (StoreInteract.instance.BuyAnItem(g.transform.parent.name))
        {
            var aux = g.GetComponent<UnityEngine.UI.Button>();
            aux.interactable = false;

            BaseData.playerInventory.Add(g.transform.parent.name);

            //DetailsWindow.EnableButton();
            //Building.instance.RefreshPlayerCoins();
        }
    }

    void EquipItem(GameObject g)
    {
        //var character = GameManager.instance.player.GetComponent<Character>();

        Debug.Log("El jugador se equipó: " + g.transform.parent.name);

        BaseData.currentWeapon = g.transform.parent.name;

        Debug.Log("Current weapon = " + BaseData.currentWeapon);

        var aux = g.GetComponent<UnityEngine.UI.Button>();
        aux.interactable = false;


        //------------------------------------------------------------------------
        /*
        var body = Manager<ItemBase>.pic["Pj"] as BodyBase;
        var weapon = Manager<ItemBase>.pic[g.transform.parent.name] as WeaponBase;

        body.principal.weapon = weapon;
        */
        //------------------------------------------------------------------------
    }


    void ShowControls(GameObject g)
    {
        CreateSubMenu.CreateNavBar
        (
            (submenu) =>
            {
                submenu.AddNavBarButton("Controls", Controls).AddNavBarButton("Loot", Loot).AddNavBarButton("Objective", Objective);
            }
        );

        controlSubMenus.Init();

        objectivesSubMenus.Init();

        lootSubMenus.Init();

        Controls();
    }

    void Controls()
    {
        controlSubMenus.Create();
    }

    void Objective()
    {
        objectivesSubMenus.Create();
    }

    void Loot()
    {
        lootSubMenus.Create();
    }

    void ShowMenuStatic(GameObject g)
    {
        CreateSubMenu.CreateBody(BodyCreate);

        CreateSubMenu.CreateNavBar
        (
            (submenu) =>
            {
                submenu.AddNavBarButton("Menu", "Menu").AddNavBarButton("opciones", "Options").AddNavBarButton("creacion", () => { });
            }
        );
    }

    void ShowControls2(GameObject g)
    {
        CreateSubMenu.CreateNavBar((submenu) => { submenu.AddNavBarButton("Controls", "Controls").AddNavBarButton("Loot", "Loot").AddNavBarButton("Objetive", "Objetive"); });
        CreateSubMenu.CreateBody(BodyCreate);
    }

    void BodyCreate(SubMenus submenu)
    {
        submenu.CreateSection(0, 4);
            submenu.AddComponent<DetailsWindow>().SetTexts("Title", "Description");

        submenu.CreateSection(4, 8);
            submenu.AddComponent<DetailsWindow>().SetImage();
    }
}
