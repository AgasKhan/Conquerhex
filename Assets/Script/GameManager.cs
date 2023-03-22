using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    /*
    public event System.Action OnPause;

    public event System.Action OnPlay;
    */

    public static GameManager instance;

    public bool menuMusic = true;

    public GameObject player;

    public Menu menu;

    public AudioListener audioListener;


    void Update()
    {
        
            
        /*
        if(menuMusic)
        {
            if (AudioManager.instance.Srch("menu").source.isPlaying)
            {
                AudioManager.instance.Srch("menu").source.volume -= Time.deltaTime / 7;

                if (AudioManager.instance.Srch("menu").source.volume < 0.1)
                {
                    AudioManager.instance.Srch("menu").source.Stop();
                }
            }
        } */       
    }

    private void LateUpdate()
    {
        if (player != null)
            transform.position = new Vector2(Mathf.Lerp(transform.position.x, player.transform.position.x, Time.deltaTime), Mathf.Lerp(transform.position.y, player.transform.position.y, Time.deltaTime * 10));
    }

    private void Awake()
    {
        instance = this;
    }



    private void OnDestroy()
    {
        AudioManager.instance.Srch("ambiente").source.Stop();
    }


    #region funciones

   

    
    #endregion
}


static class Euler
{
    public static float DifAngulosVectores(Vector2 vec1, Vector2 vec2)
    {
        float angle = Vector2.SignedAngle(vec1, vec2);

        return angle < 0 ? 360 + angle : angle;
    }

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

    public static Object[] LoadAsset(string path)
    {
        Object[] aux = Resources.LoadAll(path);

        DebugPrint.Log("Cantidad de assets cargados: " +aux.Length.ToString());

        return aux;
    }

    public static Vector2 TransVec3to2(Vector3 vec)
    {
        return new Vector2(vec.x,vec.y);
    }

    public static Vector3 TransVec2To3(Vector2 v, float z)
    {
        return new Vector3(v.x,v.y, z);
    }

    public static Vector2 VecFromDegs(float x, float m=1)
    {
        x *= Mathf.Deg2Rad;
        return new Vector2( Mathf.Cos(x) *m , Mathf.Sin(x)*m);
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