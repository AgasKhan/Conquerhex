using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Internal;

public abstract class MyScripts : MonoBehaviour
{
    protected Action MyAwakes;

    protected Action MyStarts;

    protected event Action MyUpdates;

    protected event Action MyFixedUpdates;
  

    protected event Action onPause
    {
        add
        {
            GameManager.onPause += value;
        }

        remove
        {
            GameManager.onPause -= value;
        }
    }

    protected event Action onPlay
    {
        add
        {
            GameManager.onPlay += value;
        }

        remove
        {
            GameManager.onPlay -= value;
        }
    }


    protected abstract void Config();

    internal void Awake()
    {
        GameManager.onPlay += ()=> gameObject.SetActive(true);
        GameManager.onPause += () => gameObject.SetActive(false);

        Config();

        MyAwakes?.Invoke();
    }

    internal void Start()
    {
        MyStarts?.Invoke();
    }

    internal void Update()
    {
        MyUpdates?.Invoke();
    }

    internal void FixedUpdate()
    {
        MyFixedUpdates?.Invoke();
    }

}


