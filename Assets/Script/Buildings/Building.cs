using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : Entity, Interactuable
{
    public Recipes[] upgradesRequirements;
    public ResourceType[] NavBarButtons;
    public BuildingsController controller;

    [HideInInspector]
    public int currentLevel = 0;

    [HideInInspector]
    public BuildingsSubMenu myBuildSubMenu;

    [HideInInspector]
    public Character character;

    public virtual string rewardNextLevel
    {
        get;
    }
    public int maxLevel
    {
        get
        {
            return upgradesRequirements.Length;
        }
    }

    public virtual Sprite Image => flyweight.image;

    public virtual bool interactuable { get=>enabled; set=>enabled = value; }

    protected override void Config()
    {
        base.Config();

        MyAwakes += MyAwake;
    }

    void MyAwake()
    {
        myBuildSubMenu = new BuildingsSubMenu(this);

        controller = GetComponent<BuildingsController>();

        if (SaveWithJSON.BD.ContainsKey(flyweight.nameDisplay + "Level"))
            currentLevel = SaveWithJSON.LoadFromPictionary<int>(flyweight.nameDisplay + "Level");
    }

    public virtual void UpgradeLevel()
    {
        if(controller != null)
        {
            controller.UpgradeLevel();
        }
        else
        {
            currentLevel++;
            if (currentLevel > maxLevel)
                currentLevel = maxLevel;

            SaveWithJSON.SaveInPictionary(flyweight.nameDisplay + "Level", currentLevel);
        }
    }

    public virtual void EnterBuild()
    {
        if (controller != null)
            controller.EnterBuild();
    }

    public virtual void PopUpAction(UnityEngine.Events.UnityAction action)
    {
        var aux = MenuManager.instance.modulesMenu;
        aux.ObtainMenu<PopUp>(false).SetActiveGameObject(true).SetWindow("", "¿Estas seguro de esta acción?").AddButton("No", aux.CloseMenus).AddButton("Si", action);
    }

    public virtual void Interact(Character character)
    {
        this.character = character;

        myBuildSubMenu.Create();
    }
}

public interface Interactuable
{
    void Interact(Character character); 

    Sprite Image { get; }

    bool interactuable { get; set; } 
}