using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : Entity
{
    // Mover a scrreable
    public ItemCrafteable[] upgradesRequirements;
    public ResourceType[] NavBarButtons;
    public BuildingsController controller;
    public InteractEntityComponent interactComp;
    //public Pictionarys<string, LogicActive> quickActs;
    public LevelComponent levelComp;

    public int currentLevel
    {
        get => levelComp.CurrentLevel;
        set
        {
            levelComp.CurrentLevel = value;
        }
    }

    [HideInInspector]
    public CreateSubMenu myBuildSubMenu;

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
        //myBuildSubMenu = new BuildingsSubMenu(this);

        interactComp = GetInContainer<InteractEntityComponent>();
        
        levelComp= GetInContainer<LevelComponent>();

        //levelComp.MaxLevel = () => { return upgradesRequirements.Length; };

        controller = GetComponent<BuildingsController>();

        controller?.OnEnterState(this);

        hexagoneParent = transform.root.GetComponent<Hexagone>();

        var aux = flyweight?.GetFlyWeight<UpgradeBase>();
        if (aux != null)
            upgradesRequirements = aux.upgradesRequirements;
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
        }
    }

    public virtual void PopUpAction(UnityEngine.Events.UnityAction action)
    {
        var aux = MenuManager.instance.modulesMenu;
        aux.ObtainMenu<PopUp>(false).SetActiveGameObject(true).SetWindow("", "¿Estas seguro de esta acción?").AddButton("No", aux.CloseMenus).AddButton("Si", action);
    }

    public virtual void InternalInteract(Character character)
    {

    }

}

public interface Interactuable
{
    void Interact(Character character); 

    Sprite Image { get; }

    bool interactuable { get; set; } 
}