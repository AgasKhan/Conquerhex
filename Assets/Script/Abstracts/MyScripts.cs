using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class MyScripts : MonoBehaviour
{
    protected Action MyAwakes;

    protected Action MyStarts;

    protected event Action MyUpdates
    {
        add
        {
            GameManager.update +=value;
            _updates += value;
        }

        remove
        {
            GameManager.update -= value;
            _updates -= value;
        }
    }

    protected event Action MyFixedUpdates
    {
        add
        {
            GameManager.fixedUpdate += value;
            _fixedUpdates += value;
        }

        remove
        {
            GameManager.fixedUpdate -= value;
            _fixedUpdates -= value;
        }
    }

    Action _updates;
    Action _fixedUpdates;

    protected abstract void Config();

    internal void Awake()
    {
        Config();

        MyAwakes?.Invoke();
    }

    internal void Start()
    {
        MyStarts?.Invoke();
    }

    private void OnDestroy()
    {
        GameManager.fixedUpdate -= _fixedUpdates;
        GameManager.update -= _updates;
    }
}

