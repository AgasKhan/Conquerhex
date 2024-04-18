using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ComponentsAndContainers;
using System;

public class LevelComponent : ComponentOfContainer<Entity>, ISaveObject
{
    [SerializeField]
    int _currentLevel = 0;


    public int CurrentLevel
    {
        get => _currentLevel;
        set
        {
            if (MaxLevel == null || value <= MaxLevel())
                _currentLevel = value;
            else
                _currentLevel = MaxLevel();

            _onChange?.Invoke(value);
        }
    }


    public event Action<int> OnChange
    {
        add
        {
            _onChange += value;
            value(_currentLevel);
        }
        remove
        {
            _onChange -= value;
        }
    }
    private event Action<int> _onChange;

    public Func<int> MaxLevel 
    { 
        get => _maxLevel;
        set
        {
            _maxLevel = value;
        }
    }

    private Func<int> _maxLevel;

    public string Save()
    {
        return JsonUtility.ToJson(this);
    }

    public void Load(string str)
    {
        JsonUtility.FromJsonOverwrite(str, this);
        _onChange?.Invoke(_currentLevel);
    }

    public override void OnEnterState(Entity param)
    {

    }

    public override void OnStayState(Entity param)
    {

    }

    public override void OnExitState(Entity param)
    {

    }
}
