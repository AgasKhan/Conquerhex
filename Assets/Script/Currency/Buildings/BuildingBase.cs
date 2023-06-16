using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BuildingBase : StaticEntity
{
    public Character character;
    public StructureBase structureBase;
    public Recipes[] upgradesRequirements;
    public ItemType[] NavBarButtons;
    public int currentLevel = 0;
    public BuildingsSubMenu myBuildSubMenu;
    public abstract string rewardNextLevel
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
    

    protected override Damage[] vulnerabilities => structureBase.vulnerabilities;
    
    protected override void Config()
    {
        base.Config();

        MyAwakes += MyAwake;
    }

    void MyAwake()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        character = other.GetComponent<Character>();
    }

    public virtual void UpgradeLevel()
    {
        currentLevel++;
        if (currentLevel > maxLevel)
            currentLevel = maxLevel;

    }

    public virtual void EnterBuild()
    {
        
    }
    public virtual void PopUpAction(UnityEngine.Events.UnityAction action)
    {
        var aux = MenuManager.instance.modulesMenu;
        aux.ObtainMenu<PopUp>(false).SetActiveGameObject(true).SetWindow("", "¿Estas seguro de esta acción?").AddButton("No", aux.CloseMenus).AddButton("Si", action);
    }
}
