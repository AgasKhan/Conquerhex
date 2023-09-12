using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;


public class GameManager : SingletonMono<GameManager>
{

    public static event UnityAction onPause
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

    public static event UnityAction onPlay
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

    public static Pictionarys<MyScripts, UnityAction> fixedUpdate => instance._fixedUpdate;

    public static Pictionarys<MyScripts, UnityAction> update => instance._update;

    public LayerMask obstacleAvoidanceLayer;

    public Character playerCharacter;

    public UnityEvent awakeUnityEvent;

    public UnityEvent updateUnityEvent;

    public UnityEvent fixedUpdateUnityEvent;

    Pictionarys<MyScripts, UnityAction> _update = new Pictionarys<MyScripts, UnityAction>();

    Pictionarys<MyScripts, UnityAction> _fixedUpdate = new Pictionarys<MyScripts, UnityAction>();

    FSMGameMaganer fsmGameMaganer;

    public static void RetardedOn(System.Action<bool> retardedOrder)
    {
        instance.StartCoroutine(instance.RetardedOnCoroutine(retardedOrder));
    }

    IEnumerator RetardedOnCoroutine(System.Action<bool> retardedOrder)
    {
        yield return new WaitForEndOfFrame();

        retardedOrder?.Invoke(false);
        yield return null;
        retardedOrder?.Invoke(true);
    }

    #region funciones

    public void TogglePause()
    {
        fsmGameMaganer.CurrentState = (fsmGameMaganer.CurrentState == fsmGameMaganer.pause) ? fsmGameMaganer.gamePlay : fsmGameMaganer.pause;
    }

    public void Pause(bool pause)
    {
        fsmGameMaganer.CurrentState = (!pause) ? fsmGameMaganer.gamePlay : fsmGameMaganer.pause;
    }

    public void Defeat()
    {
        TimersManager.Create(1f, 0f, 2, Mathf.Lerp, (save) => Time.timeScale = save).AddToEnd(() =>
        {
            Pause(true);
            MenuManager.instance.modulesMenu.ObtainMenu<PopUp>(false).SetActiveGameObject(true).SetWindow("Has muerto", "").AddButton("Reiniciar", () => LoadSystem.instance.Reload()).AddButton("Volver a la base", () => LoadSystem.instance.Load("Base"));
            //MenuManager.instance.modulesMenu.ObtainMenu<PopUp>(false).SetActiveGameObject(true).SetWindow("Has muerto", "").AddButton("Reiniciar", () => LoadSystem.instance.Reload()).AddButton("Ir al menu", () => LoadSystem.instance.Load("MainMenu"));
        }).SetUnscaled(true);
    }

    void MyUpdate(Pictionarys<MyScripts, UnityAction> update)
    {
        for (int i = 0; i < update.Count; i++)
        {
            update[i]();
        }
    }

    void MyUpdate()
    {
        MyUpdate(_update);
    }

    void MyFixedUpdate()
    {
        MyUpdate(_fixedUpdate);
    }

    protected override void Awake()
    {
        base.Awake();
        fsmGameMaganer = new FSMGameMaganer(this);

        updateUnityEvent.AddListener(MyUpdate);

        fixedUpdateUnityEvent.AddListener(MyFixedUpdate);

        awakeUnityEvent?.Invoke();
    }

    private void Update()
    {
        updateUnityEvent.Invoke();
    }

    private void FixedUpdate()
    {
        fixedUpdateUnityEvent.Invoke();
    }

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
        param.context.enabled = true;
    }

    public void OnExitState(FSMGameMaganer param)
    {
        param.context.enabled=(false);
    }

    public void OnStayState(FSMGameMaganer param)
    {
    }
}

public class Pause : IState<FSMGameMaganer>
{
    public event UnityAction onPause;

    public event UnityAction onPlay;

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