using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StaticEntity : Entity
{
    public LogicActive interact;

    public WorkEntiy work;

    public List<Item> inventory;

    private void Start()
    {

    }
}

public abstract class WorkEntiy : MonoBehaviour 
{
    public abstract void WorkUpdate(StaticEntity entity);
}


public class Item : ScriptableObject
{
    public string nameDisplay;
    public Sprite image;

    [Space]
    [TextArea(3, 6)]
    public string description;

    [Range(1,1000)]
    public int maxAmount=1;
}



