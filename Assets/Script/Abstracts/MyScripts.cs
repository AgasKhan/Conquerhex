using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Internal;

public abstract class MyScripts : MonoBehaviour
{
    protected Action MyAwakes;

    protected Action MyStarts;

    protected Action MyOnEnables;

    protected Action MyOnDisables;

    protected Action MyOnDestroys;

    protected event UnityEngine.Events.UnityAction MyUpdates
    {
        add
        {
            if (!bUpdate && _update == null)
            {
                bUpdate = true;
                GameManager.GamePlayUpdate += MyUpdate;
            }

            _update += value;
        }
        remove
        {
            _update -= value;

            if (bUpdate && _update == null)
            {
                bUpdate = false;
                GameManager.GamePlayUpdate -= MyUpdate;
            }
        }
    }

    protected event UnityEngine.Events.UnityAction MyFixedUpdates
    {
        add
        {
            if (!bFixed && _fixedUpdate == null)
            {
                bFixed = true;
                GameManager.GamePlayFixedUpdate += MyFixedUpdate;
            }

            _fixedUpdate += value;
        }
        remove
        {
            _fixedUpdate -= value;

            if (bFixed && _fixedUpdate == null)
            {
                bFixed = false;
                GameManager.GamePlayFixedUpdate -= MyFixedUpdate;
            }
        }
    }

    protected event UnityEngine.Events.UnityAction onPause
    {
        add
        {
            GameManager.OnPause += value;
        }

        remove
        {
            GameManager.OnPause -= value;
        }
    }

    protected event UnityEngine.Events.UnityAction onPlay
    {
        add
        {
            GameManager.OnPlay += value;
        }

        remove
        {
            GameManager.OnPlay -= value;
        }
    }

    bool bUpdate;
    UnityEngine.Events.UnityAction _update;
    bool bFixed;
    UnityEngine.Events.UnityAction _fixedUpdate;

 

    void MyUpdate()
    {
        _update();
    }

    void MyFixedUpdate()
    {
        _fixedUpdate();
    }

    protected abstract void Config();


    internal void OnEnable()
    {
        if (!bUpdate && _update != null)
        {
            bUpdate = true;
            GameManager.GamePlayUpdate += MyUpdate;
        }

        if (!bFixed && _fixedUpdate != null)
        {
            bFixed = true;
            GameManager.GamePlayFixedUpdate += MyFixedUpdate;
        }

        MyOnEnables?.Invoke();
    }

    internal void OnDisable()
    {
        if (bUpdate)
            GameManager.GamePlayUpdate -= MyUpdate;

        if (bFixed)
            GameManager.GamePlayFixedUpdate -= MyFixedUpdate;

        bUpdate = false;
        bFixed = false;

        MyOnDisables?.Invoke();
    }

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
        MyOnDestroys?.Invoke();
        _update = null;
        _fixedUpdate = null;
        MyAwakes = null;
        MyStarts = null;
        MyOnEnables = null;
        MyOnDisables = null;
        MyOnDestroys = null;
    }
}


