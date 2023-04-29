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
            _update += value;
            if(gameObject.activeSelf)
                GameManager.update.SearchOrCreate(this, _update);
        }
        remove
        {
            _update -= value;
            if (gameObject.activeSelf)
                GameManager.update.SearchOrCreate(this, _update);
        }
    }

    protected event Action MyFixedUpdates
    {
        add
        {
            _fixedUpdate += value;
            if (gameObject.activeSelf)
                GameManager.update.SearchOrCreate(this, _fixedUpdate);
        }
        remove
        {
            _fixedUpdate -= value;
            if (gameObject.activeSelf)
                GameManager.update.SearchOrCreate(this, _fixedUpdate);
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

    protected abstract void Config();

    System.Action _update;

    System.Action _fixedUpdate;

    internal void Awake()
    {
        GameManager.onPlay += GameManager_onPlay;

        GameManager.onPause += GameManager_onPause;
        Config();

        MyAwakes?.Invoke();
    }    

    private void GameManager_onPlay()
    {
        gameObject.SetActive(true);
    }

    private void GameManager_onPause()
    {
        gameObject.SetActive(false);
    }

    internal void Start()
    {
        MyStarts?.Invoke();
    }

    private void OnEnable()
    {
        if(_update!=null)   
            GameManager.update.SearchOrCreate(this, _update);

        if (_fixedUpdate != null)
            GameManager.fixedUpdate.SearchOrCreate(this, _fixedUpdate);
    }

    private void OnDisable()
    {
        GameManager.update.Remove(this);
        GameManager.fixedUpdate.Remove(this);
    }
    private void OnDestroy()
    {
        GameManager.onPlay -= GameManager_onPlay;

        GameManager.onPause -= GameManager_onPause;
    }
}


