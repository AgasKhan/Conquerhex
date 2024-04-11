using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using System.Threading.Tasks;

public class GameManager : SingletonMono<GameManager>
{
    public static event UnityAction OnPlay
    {
        add
        {
            instance.fsmGameMaganer.gamePlay.onEnterGamePlay += value;
        }

        remove
        {
            instance.fsmGameMaganer.gamePlay.onEnterGamePlay -= value;
        }
    }

    public static event UnityAction OnPause
    {
        add
        {
            instance.fsmGameMaganer.gamePlay.onExitGamePlay += value;
        }

        remove
        {
            instance.fsmGameMaganer.gamePlay.onExitGamePlay -= value;
        }
    }

    public static Queue<System.Action> eventQueue = new Queue<System.Action>(); 

    public static Pictionarys<MyScripts, UnityAction> fixedUpdate => instance._fixedUpdate;
    public static Pictionarys<MyScripts, UnityAction> update => instance._update;

    public static bool HightFrameRate => instance.stopwatch.ElapsedMilliseconds > (1000 / 120);

    public static bool MediumFrameRate => instance.stopwatch.ElapsedMilliseconds > (1000 / 60);

    public static bool SlowFrameRate => instance.stopwatch.ElapsedMilliseconds > (1000 / 30);

    public LayerMask obstacleAvoidanceLayer;

    public Character playerCharacter;

    [SerializeField]
    public EventManager eventManager;

    [Header("Executions")]

    public UnityEvent awakeUnityEvent;

    public UnityEvent updateUnityEvent;

    public UnityEvent fixedUpdateUnityEvent;

    public UnityEvent onDestroyUnityEvent;

    Pictionarys<MyScripts, UnityAction> _update = new Pictionarys<MyScripts, UnityAction>();

    Pictionarys<MyScripts, UnityAction> _fixedUpdate = new Pictionarys<MyScripts, UnityAction>();

    [SerializeField]
    FSMGameMaganer fsmGameMaganer;

    System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

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

    public string ActualStringState()
    {
        return fsmGameMaganer?.CurrentState?.GetType().Name ?? "Off";
    }

    public void Load()
    {
        fsmGameMaganer.CurrentState = fsmGameMaganer.load;
    }

    public void GamePlay()
    {
        fsmGameMaganer.CurrentState = fsmGameMaganer.gamePlay;
    }

    public void TogglePause()
    {
        if (fsmGameMaganer.CurrentState == fsmGameMaganer.pause || fsmGameMaganer.gamePlay == fsmGameMaganer.CurrentState)
            fsmGameMaganer.CurrentState = (fsmGameMaganer.CurrentState == fsmGameMaganer.pause) ? fsmGameMaganer.gamePlay : fsmGameMaganer.pause;
    }

    public void Pause(bool pause)
    {
        if (fsmGameMaganer.CurrentState != fsmGameMaganer.load)
            fsmGameMaganer.CurrentState = (!pause) ? fsmGameMaganer.gamePlay : fsmGameMaganer.pause;
    }

    public void Defeat(string msj)
    {
        TimersManager.Create(1f, 0f, 2, Mathf.Lerp, (save) => Time.timeScale = save).AddToEnd(() =>
        {
            EndGame();

            fsmGameMaganer.endGame.defeat.Invoke();

            MenuManager.instance.modulesMenu.ObtainMenu<PopUp>(false).SetActiveGameObject(true).SetWindow(msj, "").AddButton("Reiniciar", () => LoadSystem.instance.Reload()).AddButton("Volver a la base", () => LoadSystem.instance.Load("Base"));

            eventManager.events.SearchOrCreate<SingleEvent>("defeat").delegato.Invoke();
            
        }).SetUnscaled(true);

        /*
        TimersManager.Create(1f, 0f, 2, Mathf.Lerp, (save) => Time.timeScale = save).AddToEnd(() =>
        {
            
        }).SetUnscaled(true);
        */
    }

    public void Victory()
    {
        EndGame();

        fsmGameMaganer.endGame.victory.Invoke();

        eventManager.events.SearchOrCreate<SingleEvent>("victory").delegato.Invoke(); ;
    }

    void EndGame()
    {
        fsmGameMaganer.CurrentState = fsmGameMaganer.endGame;
    }

    void MyUpdate(Pictionarys<MyScripts, UnityAction> update)
    {
        foreach (var item in update)
        {
            item.value();
        }

        /*
        
        for (int i = 0; i < update.Count; i++)
        {
            update[i]();
        }

        Task[] tasks = new Task[update.Count];

        for (int i = 0; i < update.Count; i++)
        {
            update[i]();
            int index = i; // Captura de la variable en el contexto del bucle
            tasks[index] = Task.Run(() => update[index]());
        }

        Task.WaitAll(tasks);
        */
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
        fsmGameMaganer.Init(this);
        fsmGameMaganer.EnterState(fsmGameMaganer.load);

        updateUnityEvent.AddListener(MyUpdate);

        fixedUpdateUnityEvent.AddListener(MyFixedUpdate);

        awakeUnityEvent?.Invoke();
    }

    private void Update()
    {
        stopwatch.Restart();

        updateUnityEvent?.Invoke();

        do
        {
            if (eventQueue.TryDequeue(out var action))
                action();

        } while (eventQueue.Count > 0 && !MediumFrameRate);
    }

    private void FixedUpdate()
    {
        fixedUpdateUnityEvent?.Invoke();
    }

    private void OnDestroy()
    {
        onDestroyUnityEvent?.Invoke();
    }

    #endregion
}

[System.Serializable]
public class FSMGameMaganer : FSMSerialize<FSMGameMaganer, GameManager>
{
    public Load load = new Load();
    public Gameplay gamePlay = new Gameplay();
    public Pause pause = new Pause();
    public EndGame endGame = new EndGame();
}

[System.Serializable]
public class Load : IState<FSMGameMaganer>
{
    public UnityEvent onStartLoad;

    public UnityEvent onFinishLoad;

    public void OnEnterState(FSMGameMaganer param)
    {
        onStartLoad.Invoke();
    }

    public void OnExitState(FSMGameMaganer param)
    {
        onFinishLoad.Invoke();
    }

    public void OnStayState(FSMGameMaganer param)
    {
    }
}

[System.Serializable]
public class Gameplay : IState<FSMGameMaganer>
{
    public UnityEvent onEnterGamePlayUnityEvent;

    public UnityEvent onExitGamePlayUnityEvent;

    public event UnityAction onEnterGamePlay;

    public event UnityAction onExitGamePlay;

    public void OnEnterState(FSMGameMaganer param)
    {
        onEnterGamePlayUnityEvent.Invoke();
        onEnterGamePlay?.Invoke();
    }

    public void OnExitState(FSMGameMaganer param)
    {
        onExitGamePlayUnityEvent.Invoke();
        onExitGamePlay?.Invoke();
    }

    public void OnStayState(FSMGameMaganer param)
    {
    }
}

[System.Serializable]
public class EndGame : IState<FSMGameMaganer>
{
    public UnityEvent victory;

    public UnityEvent defeat;

    public void OnEnterState(FSMGameMaganer param)
    {
        //param.context.eventManager.events.SearchOrCreate<SingleEvent>("close").delegato.Invoke();
    }

    public void OnExitState(FSMGameMaganer param)
    {
    }

    public void OnStayState(FSMGameMaganer param)
    {
    }
}

[System.Serializable]
public class Pause : IState<FSMGameMaganer>
{
    public UnityEvent onPauseUnityEvent;

    public UnityEvent onDesPauseUnityEvent;

    public void OnEnterState(FSMGameMaganer param)
    {
        Time.timeScale = 0;
        onPauseUnityEvent.Invoke();
        param.context.enabled = false;
    }

    public void OnExitState(FSMGameMaganer param)
    {
        onDesPauseUnityEvent.Invoke();
        Time.timeScale = 1;
        param.context.enabled = true;
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
        warning.Add("<color=yellow>" + t + "</color>");
    }

    public static void Error(string t)
    {
        error.Add("<color=red>" + t + "</color>");
    }

    public static bool chk()
    {
        if (debug.LenghtChk() || error.LenghtChk() || warning.LenghtChk())
            return true;
        return false;
    }

    public static string PrintSalida()
    {
        if (chk())
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
        return pantalla.Length > 0 ? true : false;
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
        pantalla = "";
    }

    public void Clear()
    {
        pantalla = "";
    }
}