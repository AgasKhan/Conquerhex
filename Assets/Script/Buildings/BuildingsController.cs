using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingsController : MonoBehaviour, IState<Building>
{
    protected virtual Building myBuilding { get; set; }
    
    public virtual void UpgradeLevel()
    {
        myBuilding.currentLevel++;
        if (myBuilding.currentLevel > myBuilding.maxLevel)
            myBuilding.currentLevel = myBuilding.maxLevel;

        SaveWithJSON.SaveInPictionary(myBuilding.flyweight.nameDisplay + "Level", myBuilding.currentLevel);
    }

    public void OnEnterState(Building param)
    {
        myBuilding = param;
    }

    public void OnStayState(Building param)
    {
        throw new System.NotImplementedException();
    }

    public void OnExitState(Building param)
    {
        throw new System.NotImplementedException();
    }
}
