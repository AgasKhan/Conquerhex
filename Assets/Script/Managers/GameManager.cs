using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : SingletonMono<GameManager>
{

    public static event System.Action onPause
    {
        add
        {
            instance.fsmGameMaganer.pause.onPause += value;
        }

        remove
        {
            instance.fsmGameMaganer.pause.onPause -= value;
        }
    }

    public static event System.Action onPlay
    {
        add
        {
            instance.fsmGameMaganer.pause.onPlay += value;
        }

        remove
        {
            instance.fsmGameMaganer.pause.onPlay -= value;
        }
    }

    public static event System.Action update
    {
        add
        {
            instance._update += value;
        }
        remove
        {
            instance._update -= value;
        }
    }

    public static event System.Action fixedUpdate
    {
        add
        {
            instance._fixedUpdate += value;
        }
        remove
        {
            instance._fixedUpdate -= value;
        }
    }

    event System.Action _update;

    event System.Action _fixedUpdate;

    FSMGameMaganer fsmGameMaganer;

    public GameObject player;

    void Update()
    {
        _update?.Invoke();
    }

    private void FixedUpdate()
    {
        _fixedUpdate?.Invoke();
    }

    protected override void Awake()
    {
        base.Awake();
        fsmGameMaganer = new FSMGameMaganer(this);
    }

    #region funciones

    public void Pause()
    {
        fsmGameMaganer.CurrentState = (fsmGameMaganer.CurrentState == fsmGameMaganer.pause) ? fsmGameMaganer.gamePlay : fsmGameMaganer.pause;
    }

    public void Pause(bool pause)
    {
        fsmGameMaganer.CurrentState = (pause) ? fsmGameMaganer.gamePlay : fsmGameMaganer.pause;
    }

    /*

    static public void RetardedFrame(System.Action action, int frames = 1)
    {
        instance.StartCoroutine(instance._RetardedFrame(action));
    }

    public IEnumerator _RetardedFrame(System.Action action, int frames = 1)
    {
        for (int i = 0; i < frames; i++)
        {
            yield return null;
        }

        action();
    }

    */
    #endregion
}

public class FSMGameMaganer : FSM<FSMGameMaganer, GameManager>
{
    public Load load = new Load();
    public Gameplay gamePlay = new Gameplay();
    public Pause pause = new Pause();

    public FSMGameMaganer(GameManager reference) : base(reference)
    {
        Init(gamePlay);
    }
}

public class Load : IState<FSMGameMaganer>
{
    public void OnEnterState(FSMGameMaganer param)
    {
    }

    public void OnExitState(FSMGameMaganer param)
    {
    }

    public void OnStayState(FSMGameMaganer param)
    {
    }
}

public class Gameplay : IState<FSMGameMaganer>
{
    public void OnEnterState(FSMGameMaganer param)
    {
        param.context.gameObject.SetActive(true);
    }

    public void OnExitState(FSMGameMaganer param)
    {
        param.context.gameObject.SetActive(false);
    }

    public void OnStayState(FSMGameMaganer param)
    {
    }
}

public class Pause : IState<FSMGameMaganer>
{
    public event System.Action onPause;

    public event System.Action onPlay;

    public void OnEnterState(FSMGameMaganer param)
    {
        onPause?.Invoke();
        Time.timeScale = 0;
    }

    public void OnExitState(FSMGameMaganer param)
    {
        onPlay?.Invoke();
        Time.timeScale = 1;
    }

    public void OnStayState(FSMGameMaganer param)
    {
    }
}


static class DebugPrint
{
    static PrintF debug;
    static PrintF warning;
    static PrintF error;

    public static void Log(string t) 
    {
        debug.Add(t);
    }

    public static void Warning(string t)
    {
        warning.Add("<color=yellow>"+t+"</color>");
    }

    public static void Error(string t)
    {
        error.Add("<color=red>"+t+"</color>");
    }

    public static bool chk()
    {
        if (debug.LenghtChk() || error.LenghtChk() || warning.LenghtChk())
            return true;
        return false;
    }

    public static string PrintSalida()
    {
        if(chk())

            return error.Out() +
                    warning.Out() +
                    debug.Out();

        return "";
    }


    public static void PrintConsola()
    {
        error.Print("error");
        warning.Print("warning");
        debug.Print();
    }

}


struct PrintF
{
    string pantalla;

    public void Add(string palabra)
    {
        if (pantalla != null && pantalla != "")
            pantalla += "\n" + palabra;
        else
            pantalla = palabra;
    }

    public string Out()
    {
        string aux = pantalla;
        pantalla = "";
        return aux;

    }

    public bool LenghtChk()
    {
        return pantalla.Length > 0 ? true: false ;
    }

    public void Print(string debugMode = "debug")
    {
        
        if (pantalla != null && pantalla != "")
        {
            switch (debugMode)
            {
                case "warning":
                    Debug.LogWarning(pantalla);
                    break;

                case "error":
                    Debug.LogError(pantalla);
                    break;

                default:
                    Debug.Log(pantalla);
                    break;
            }
        }
        pantalla="";
    }

    public void Clear()
    {
        pantalla = "";
    }
}