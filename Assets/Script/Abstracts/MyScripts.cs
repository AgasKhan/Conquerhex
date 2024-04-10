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
            _update += value;
            if (gameObject.activeSelf)
                GameManager.eventQueue.Enqueue(SaveUpdate);
        }
        remove
        {
            _update -= value;


            if (gameObject.activeSelf)
                GameManager.eventQueue.Enqueue(SaveUpdate);

            if (_update == null)
            {
                GameManager.eventQueue.Enqueue(RemoveUpdate);
            }

        }
    }

    protected event UnityEngine.Events.UnityAction MyFixedUpdates
    {
        add
        {
            _fixedUpdate += value;
            if (gameObject.activeSelf)
                GameManager.eventQueue.Enqueue(SaveFixedUpdate);
        }
        remove
        {
            _fixedUpdate -= value;

            if (gameObject.activeSelf)
                GameManager.eventQueue.Enqueue(SaveFixedUpdate);

            if (_fixedUpdate == null)
            {
                GameManager.eventQueue.Enqueue(RemoveFixedUpdate);
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

    protected abstract void Config();

    UnityEngine.Events.UnityAction _update;

    UnityEngine.Events.UnityAction _fixedUpdate;

    bool active=true;

    private void SaveUpdate()
    {
        if(_update!=null)
            GameManager.update.CreateOrSave(this, _update);
    }

    private void RemoveUpdate()
    {
        GameManager.update.Remove(this);
    }

    private void SaveFixedUpdate()
    {
        if (_fixedUpdate != null)
            GameManager.fixedUpdate.CreateOrSave(this, _fixedUpdate);
    }

    private void RemoveFixedUpdate()
    {
        GameManager.fixedUpdate.Remove(this);
    }

    private void GameManager_onPlay()
    {
        enabled = active;
    }

    private void GameManager_onPause()
    {
        active = enabled;

        enabled = false;
    }

    internal void Awake()
    {
        GameManager.OnPlay += GameManager_onPlay;

        GameManager.OnPause += GameManager_onPause;
        Config();

        MyAwakes?.Invoke();
    }    

    internal void Start()
    {
        MyStarts?.Invoke();
    }

    private void OnEnable()
    {
        MyOnEnables?.Invoke();

        GameManager.eventQueue.Enqueue(()=>
            {
                SaveUpdate();

                SaveFixedUpdate();
            });
    }

    private void OnDisable()
    {
        MyOnDisables?.Invoke();

        GameManager.eventQueue.Enqueue(() =>
        {
            RemoveUpdate();
            RemoveFixedUpdate();
        });
    }
    private void OnDestroy()
    {
        MyOnDestroys?.Invoke();

        GameManager.OnPlay -= GameManager_onPlay;

        GameManager.OnPause -= GameManager_onPause;
    }
}


