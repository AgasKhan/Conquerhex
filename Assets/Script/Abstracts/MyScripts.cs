using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Internal;

public abstract class MyScripts : MonoBehaviour
{
    protected Action MyAwakes;

    protected Action MyStarts;

    protected event Action MyUpdates
    {
        add
        {
            GameManager.update += value;
            update += value;
        }

        remove
        {
            GameManager.update -= value;
            update -= value;
        }
    }

    protected event Action MyFixedUpdates
    {
        add
        {
            GameManager.fixedUpdate += value;
            fixedUpdate += value;
        }

        remove
        {
            GameManager.fixedUpdate -= value;
            fixedUpdate -= value;
        }
    }

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

    System.Action update;
    System.Action fixedUpdate;

    void OnPlay()
    {
        GameManager.update += update;
        GameManager.fixedUpdate += fixedUpdate;
    }

    void OnPause()
    {
        GameManager.update -= update;
        GameManager.fixedUpdate -= fixedUpdate;
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

    private void OnEnable()
    {
        OnPlay();
    }

    private void OnDisable()
    {
        OnPause();
    }
}


