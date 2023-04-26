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
            _update.action += value;
        }

        remove
        {
            _update.action -= value;
        }
    }

    protected event Action MyFixedUpdates
    {
        add
        {
           _fixed.action += value;
        }

        remove
        {
            _fixed.action -= value;
        }
    }

    protected event Action onPause
    {
        add
        {
            _pause.action += value;
        }

        remove
        {
            _pause.action -= value;
        }
    }

    protected event Action onPlay
    {
        add
        {
            _play.action += value;
        }

        remove
        {
            _play.action -= value;
        }
    }

    MyDelegates _update = new MyDelegates((a)=> GameManager.update+=a, (a) => GameManager.update -= a, true);

    MyDelegates _fixed = new MyDelegates((a) => GameManager.fixedUpdate += a, (a) => GameManager.fixedUpdate -= a, true);

    MyDelegates _pause = new MyDelegates((a) => GameManager.onPause += a, (a) => GameManager.onPause -= a, true);

    MyDelegates _play = new MyDelegates((a) => GameManager.onPlay += a, (a) => GameManager.onPlay -= a, true);

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

    protected virtual void OnEnable()
    {
        _update.passToChck = true;
        _fixed.passToChck = true;
        _pause.passToChck = true;
        _play.passToChck = true;
    }

    protected virtual void OnDisable()
    {
        _update.passToChck = false;
        _fixed.passToChck = false;
        _pause.passToChck = false;
        _play.passToChck = false;
    }
}



//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

namespace Internal
{
    public struct MyDelegates
    {
        System.Action<System.Action> passTo;

        System.Action<System.Action> removeTo;

        System.Action list;

        bool _passToChck;//positivo NO estoy en pausa

        public bool passToChck
        {
            get => _passToChck;
            set
            {
                _passToChck = value;

                if (list == null)
                    return;

                if (value)
                    PassTo();
                else
                    RemoveTo();
            }
        }

        public event System.Action action
        {
            add
            {
                list += value;
                if(_passToChck)
                    passTo(value);

            }
            remove
            {
                list -= value; 
                if (_passToChck)
                    removeTo(value);
            }
        }

        void PassTo()
        {
            passTo(list);
        }

        void RemoveTo()
        {
            removeTo(list);
        }

        public MyDelegates(Action<Action> passTo, Action<Action> removeTo, bool passToChck) : this()
        {
            this.passTo = passTo;
            this.removeTo = removeTo;
            this.passToChck = passToChck;

            list = null;
        }

    }
}


