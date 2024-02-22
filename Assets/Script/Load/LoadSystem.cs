using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Diagnostics;
using System.IO;
using System.Linq;

public class LoadSystem : SingletonMono<LoadSystem>
{
    static List<WaitForCorutines.MyCoroutine> preLoad = new List<WaitForCorutines.MyCoroutine>();
    static List<WaitForCorutines.MyCoroutine> postLoad = new List<WaitForCorutines.MyCoroutine>();

    static List<System.Action> preLoadEvent = new List<System.Action>();
    static List<System.Action> postLoadEvent = new List<System.Action>();

    public static Stopwatch stopwatch = new Stopwatch();

    [SerializeReference]
    LoadScreen loadScreen;

    public bool loadPause;

    public string[] noSerailizeProperties;

    [SerializeReference]
    SaveWithJSON saveWithJSON;

    [SerializeField]
    Lenguages lenguages;

    // Start is called before the first frame update
    protected override void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        saveWithJSON = new SaveWithJSON();

        saveWithJSON.Init();

        lenguages.Init();

        DontDestroyOnLoad(gameObject);



        StartCoroutine(PostLoad((b)=>{ }, (s)=> { loadScreen.Progress(s);  }));
        preLoad.Add(loadScreen.LoadImage);
    }

    //delegado especifico

    public static void AddPreLoadCorutine(WaitForCorutines.MyCoroutine myCoroutine, int insert=-1)
    {
        preLoad.AddOrInsert(myCoroutine, insert);
    }

    public static void AddPostLoadCorutine(WaitForCorutines.MyCoroutine myCoroutine, int insert = -1)
    {
        postLoad.AddOrInsert(myCoroutine, insert);
    }

   
    //delegado adaptado
    public static void AddPreLoadCorutine(System.Action myCoroutine, int insert = -1)
    {
        preLoadEvent.AddOrInsert(myCoroutine, insert);
    }

    public static void AddPostLoadCorutine(System.Action myCoroutine, int insert = -1)
    {
        postLoadEvent.AddOrInsert(myCoroutine, insert);
    }

    public static Object[] LoadAssets(string path)
    {
        Object[] aux = Resources.LoadAll(path);

        //Debug.Log("Cantidad de assets cargados: " + aux.Length.ToString());

        return aux;
    }

    public static T[] LoadAssets<T>(string path) where T : Object
    {
        T[] aux = Resources.LoadAll<T>(path);

        //Debug.Log("Cantidad de assets cargados: " + aux.Length.ToString());

        return aux;
    }

    public void Load(string scn, bool pause = false)
    {
        loadPause = pause;

        StopAllCoroutines();

        loadScreen.Open();
        StartCoroutine(LoadScene(scn));
        Time.timeScale = 1;
    }

    public void LoadAndSavePlayer(string scn, bool pause = false)
    {
        SaveWithJSON.SaveClassInPictionary("Player", GameManager.instance.playerCharacter);
        Load(scn, pause);
        
    }

    public void Reload()
    {
        Load(SceneManager.GetActiveScene().name, loadPause);
    }

    IEnumerator LoadScene(string scene)
    {
        loadScreen.Progress(0, "Start loading");

        yield return null;

        stopwatch.Start();

        //loadscene = true;
        for (int i = 0; i < preLoad.Count; i++)
        {
            yield return new WaitForCorutines(this, preLoad[i], (s) => loadScreen.Progress((((i + 1f) / (preLoad.Count)) * (1f / 6) + 0) * 100, s));
            stopwatch.Restart();
        }

        loadScreen.Progress("Close action scripts");

        yield return null;

        for (int i = 0; i < preLoadEvent.Count; i++)
        {
            preLoadEvent[i]();
            loadScreen.Progress((((i + 1f) / (preLoadEvent.Count)) * (1f / 6) + (1f / 6)) * 100);
            if (stopwatch.ElapsedMilliseconds > 1f/60)
            {
                yield return null;
                stopwatch.Restart();
            }
        }

        loadScreen.Progress("Start scene load");

        preLoad.Clear();
        preLoadEvent.Clear();

        postLoad.Clear();
        postLoadEvent.Clear();

        yield return null;

        AsyncOperation async = SceneManager.LoadSceneAsync(scene);

        ///////////////////////////////////////////////////////////////////////////////////////////////////
        while (!async.isDone)
        {
            loadScreen.Progress( ((1f / 3) + async.progress *(1f / 3)) * 100, "loading scene");
            yield return null;
        }
        //////////////////////////////////////////////////////////////////////////////////////////////////

        //awake ejecutar
        yield return new WaitForCorutines(this, PostLoad, (s) => loadScreen.Progress(s));
    }

    IEnumerator PostLoad(System.Action<bool> end, System.Action<string> msg)
    {
        msg("loading script scene");

        Time.timeScale = 0;

        stopwatch.Restart();

        //espera un frame a que se carguen todos los postload
        yield return null;

        for (int i = 0; i < postLoad.Count; i++)
        {
            yield return new WaitForCorutines(this, postLoad[i], (s) => loadScreen.Progress((((i + 1f) / (postLoad.Count)) * (1f / 6) + (2f / 3)) * 100, s));
        }

        yield return null;

        for (int i = 0; i < postLoadEvent.Count; i++)
        {
            postLoadEvent[i]();
            loadScreen.Progress(( ((i + 1f) / (postLoadEvent.Count)) * (1f / 6) + 5f / 6 )*100, $"Load scripts: {i}/{postLoadEvent.Count}");

            if (stopwatch.ElapsedMilliseconds > 1f / 60)
            {
                yield return null;
                stopwatch.Restart();
            }
        }

        loadScreen.Progress(100, "<size=50>Carga finalizada</size>");

        stopwatch.Stop();

        yield return null;

        if (loadPause)
        {
            loadScreen.Progress("<size=50>Carga finalizada</size>" +
            "\n<size=20> Presione <color=green>espacio</color> para continuar </size>");

            while (!Input.GetKeyDown(KeyCode.Space) && !(Input.touches.Length > 0))
            {
                yield return null;
            }
        }        

        loadScreen.Close();

        preLoad.Add(loadScreen.LoadImage);

        Time.timeScale = 1;
        end(true);
    }
}

public class WaitForCorutines : CustomYieldInstruction
{
    public delegate IEnumerator MyCoroutine(System.Action<bool> end, System.Action<string> msg);

    public override bool keepWaiting => !end;

    bool end=false;

    public WaitForCorutines(MonoBehaviour mono,MyCoroutine coroutine, System.Action<string> msg)
    {
        mono.StartCoroutine(coroutine((b)=>end=b, msg));
    }
}


[System.Serializable]
public class Lenguages : Init
{
    [SerializeField]
    TextAsset read;

    [SerializeField]
    TextAsset write;

    string path = "Assets/Lenguage/";

    StreamReader csvArchiveRead;

    StreamWriter csvArchiveWrite;

    string toWrite;

    HashSet<string> keysNotFinded = new HashSet<string>();

    public string lenguage = "español";

    public string rowSeparator="\n";

    public string colSeparator="\t";

    string[,] textArray;

    int indexLenguage;

    public static Lenguages instance;

    public string this[string aux] 
    { 
        get 
        {
            for (int i = 0; i < textArray.GetLength(0); i++)
            {
                if(textArray[i, 0] == aux)
                {
                    return textArray[i, indexLenguage];
                }
            }

            UnityEngine.Debug.LogWarning("no se encontro el key en lenguage");

            keysNotFinded.Add(aux);

            return string.Empty;
        } 
    }

    public void ChangeLenguage(string lenguage)
    {
        this.lenguage = lenguage;

        if(RefreshLenguage())
        {
            SaveWithJSON.SaveInPictionary("Lenguage", lenguage);
        }
        else
        {
            UnityEngine.Debug.LogError("Lenguage no encontrado");
        }
    }

    public bool RefreshLenguage()
    {
        for (indexLenguage = 1; indexLenguage < textArray.GetLength(1); indexLenguage++)
        {
            if(lenguage == textArray[0, indexLenguage])
            {
                return true;
            }
        }

        return false;
    }

    public void Init()
    {
        instance = this;

        csvArchiveRead = new StreamReader(path + read.name);

        csvArchiveWrite = new StreamWriter(path + write.name);

        toWrite = csvArchiveRead.ReadToEnd();

        csvArchiveRead.Close();

        var filas = toWrite.Split(rowSeparator, System.StringSplitOptions.RemoveEmptyEntries);

        textArray = new string[filas.Length, filas[0].Split(colSeparator).Length];

        for (int i = 0; i < filas.Length; i++)
        {
            var aux = filas[i].Split(colSeparator);

            for (int j = 0; j < aux.Length; j++)
            {
                textArray[i, j] = aux[j];
            }
        }

        lenguage = SaveWithJSON.LoadFromPictionary("Lenguage",lenguage);

        RefreshLenguage();
    }

    ~Lenguages()
    {
        csvArchiveWrite.Write(toWrite + string.Join('\n', keysNotFinded.OrderBy(str => str).ToArray()));
        csvArchiveWrite.Close();
    }
}
