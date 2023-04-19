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

    public static event System.Action update;

    public static event System.Action fixedUpdate;

    FSMGameMaganer fsmGameMaganer;

    public GameObject player;

    void Update()
    {
        update?.Invoke();
    }

    private void FixedUpdate()
    {
        fixedUpdate?.Invoke();
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

static class Euler
{

    public static float[,] LocalSidePosHex(float[,] auxCalc2, float apotema, float magnitud = 1f)
    {


        DebugPrint.Log("Calculo de posición de lados");

        //calcula las coordenadas relativas de los lados de los hexagonos y lo retorna
        for (int i = 0; i < auxCalc2.GetLength(0); i++)
        {
            /*
            auxCalc[i, 0] = ((((lado/2) * Mathf.Sin((1f / 3f) * Mathf.PI)) / (Mathf.Sin((1f / 6f) )* Mathf.PI))) * Mathf.Cos((1f / 2f) * Mathf.PI + (1f / 3f * Mathf.PI) * i);
            auxCalc[i, 1] = ((((lado/2) * Mathf.Sin((1f / 3f) * Mathf.PI)) / (Mathf.Sin((1f / 6f) )* Mathf.PI))) * Mathf.Sin((1f / 2f) * Mathf.PI + (1f / 3f * Mathf.PI) * i);
            */

            //Cuenta que calcula los puntos relativos
            auxCalc2[i, 0] = (apotema) * Mathf.Cos((1f / 2f) * Mathf.PI - (1f / 3f) * Mathf.PI * i) * magnitud;
            auxCalc2[i, 1] = (apotema) * Mathf.Sin((1f / 2f) * Mathf.PI - (1f / 3f) * Mathf.PI * i) * magnitud;

            DebugPrint.Log(auxCalc2[i, 0] + " " + auxCalc2[i, 1]);

        }

        
        return auxCalc2;

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