using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Diagnostics;
using System.IO;
using System.Linq;

[CreateAssetMenu(menuName = "BaseData/LoadSystem", fileName = "LoadSystem")]
public class LoadSystem : SingletonScript <LoadSystem>
{
    static List<WaitForCorutinesForLoad.MyCoroutine> preLoad = new List<WaitForCorutinesForLoad.MyCoroutine>();
    static List<WaitForCorutinesForLoad.MyCoroutine> postLoad = new List<WaitForCorutinesForLoad.MyCoroutine>();

    static List<System.Action> preLoadEvent = new List<System.Action>();
    static List<System.Action> postLoadEvent = new List<System.Action>();

    public event System.Action onStartLoad;

    public event System.Action<float, string> onFeedbackLoad;

    public event System.Action onFinishtLoad;

    public bool loadPause;

    //public string[] noSerailizeProperties;

    [SerializeReference]
    SaveWithJSON saveWithJSON;

    [SerializeField]
    Lenguages lenguages;

    //delegado especifico

    public static void AddPreLoadCorutine(WaitForCorutinesForLoad.MyCoroutine myCoroutine, int insert=-1)
    {
        preLoad.AddOrInsert(myCoroutine, insert);
    }

    public static void AddPostLoadCorutine(WaitForCorutinesForLoad.MyCoroutine myCoroutine, int insert = -1)
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

    public IEnumerator LoadAndSavePlayer(string scn, bool pause = false)
    {
        SaveWithJSON.SaveClassInPictionary("Player", GameManager.instance.playerCharacter);
        return ExitCoroutine(SceneManager.GetActiveScene().name, loadPause);
    }

    public IEnumerator Reload()
    {
        return ExitCoroutine(SceneManager.GetActiveScene().name, loadPause);
    }

    public IEnumerator EnterCoroutine()
    {
        onFeedbackLoad?.Invoke(-1, "loading script scene");

        onStartLoad?.Invoke();

        Time.timeScale = 0;

        //espera un frame a que se carguen todos los postload
        yield return null;

        for (int i = 0; i < postLoad.Count; i++)
        {
            yield return new WaitForCorutinesForLoad(GameManager.instance, postLoad[i], (s) => onFeedbackLoad?.Invoke((((i + 1f) / (postLoad.Count)) * (1f / 6) + (2f / 3)) * 100, s));
        }

        yield return null;

        for (int i = 0; i < postLoadEvent.Count; i++)
        {
            postLoadEvent[i]();
            onFeedbackLoad?.Invoke((((i + 1f) / (postLoadEvent.Count)) * (1f / 6) + 5f / 6) * 100, $"Load scripts: {i}/{postLoadEvent.Count}");

            if (GameManager.VerySlowFrameRate)
            {
                yield return null;
                //stopwatch.Restart();
            }
        }

        onFeedbackLoad?.Invoke(100, "<size=50>Carga finalizada</size>");

        //stopwatch.Stop();

        yield return null;

        if (loadPause)
        {
            onFeedbackLoad?.Invoke(-1, "<size=50>Carga finalizada</size>" +
            "\n<size=20> Presione <color=green>espacio</color> para continuar </size>");

            while (!Input.GetKeyDown(KeyCode.Space) && !(Input.touches.Length > 0))
            {
                yield return null;
            }
        }

        Time.timeScale = 1;

        onFinishtLoad?.Invoke();
    }

    public IEnumerator ExitCoroutine(string scene, bool pause = false)
    {
        loadPause = pause;

        onStartLoad?.Invoke();

        onFeedbackLoad?.Invoke(0, "Start loading");

        yield return null;

        //stopwatch.Start();

        //loadscene = true;
        for (int i = 0; i < preLoad.Count; i++)
        {
            yield return new WaitForCorutinesForLoad(GameManager.instance, preLoad[i], (s) => onFeedbackLoad?.Invoke((((i + 1f) / (preLoad.Count)) * (1f / 6) + 0) * 100, s));
            //stopwatch.Restart();
        }

        onFeedbackLoad?.Invoke(-1,"Close action scripts");

        yield return null;

        for (int i = 0; i < preLoadEvent.Count; i++)
        {
            preLoadEvent[i]();
            onFeedbackLoad?.Invoke((((i + 1f) / (preLoadEvent.Count)) * (1f / 6) + (1f / 6)) * 100, string.Empty);
            if (GameManager.VerySlowFrameRate)
            {
                yield return null;
                //stopwatch.Restart();
            }
        }

        onFeedbackLoad?.Invoke(-1,"Start scene load");

        preLoad.Clear();
        preLoadEvent.Clear();

        postLoad.Clear();
        postLoadEvent.Clear();

        yield return null;

        AsyncOperation async = SceneManager.LoadSceneAsync(scene);

        ///////////////////////////////////////////////////////////////////////////////////////////////////
        while (!async.isDone)
        {
            onFeedbackLoad?.Invoke( ((1f / 3) + async.progress *(1f / 3)) * 100, "loading scene");
            yield return null;
        }
        //////////////////////////////////////////////////////////////////////////////////////////////////

        //awake ejecutar
        //yield return new WaitForCorutinesForLoad(GameManager.instance, PostLoad, (s) => loadScreen.Progress(s));

        onFinishtLoad?.Invoke();
    }

    public void MyAwake()
    {
        lenguages.Init();
        saveWithJSON = new SaveWithJSON();

        saveWithJSON.Init();
    }

    public void MyDestroy()
    {
        lenguages.OnDestroy();
    }
}

public class WaitForCorutinesForLoad : CustomYieldInstruction
{
    public delegate IEnumerator MyCoroutine(System.Action<bool> end, System.Action<string> msg);

    public override bool keepWaiting => !end;

    protected bool end = false;

    protected IEnumerator coroutine;

    protected MonoBehaviour mono;

    IEnumerator MyRutine()
    {
        yield return mono.StartCoroutine(coroutine);
        end = true;
    }

    public WaitForCorutinesForLoad(MonoBehaviour mono,MyCoroutine coroutine, System.Action<string> msg)
    {
        this.mono = mono;
        this.coroutine = coroutine.Invoke((b) => end = b, msg);

        mono.StartCoroutine(MyRutine());
    }
}



[System.Serializable]
public class Lenguages : Init
{
    [SerializeField]
    TextAsset read;
    
    [SerializeField]
    TextAsset write;

    #if UNITY_EDITOR

    const string path = "Assets/Resources/Lenguages/";

    //StreamReader csvArchiveRead;

    StreamWriter csvArchiveWrite;

    #endif

    string toWrite;

    HashSet<string> keysNotFinded = new HashSet<string>();

    public string lenguage = "español";

    string rowSeparator="\n";

    string colSeparator="\t";

    string[,] textArray;

    int indexLenguage;

    public static Lenguages instance;

    public string this[object index] 
    { 
        get 
        {
            string aux;

            System.Type indexType = index.GetType();

            if (index is string)
                aux = index as string;
            else if (!indexType.IsGenericType)
            {
                aux = index.GetType().Name;
            }
            else
                aux = indexType.Name.Substring(0, indexType.Name.IndexOf('`')) + "_" + indexType.GetGenericArguments().Select(type => type.Name).Aggregate((str, str2) => str + '-' + str2);

            for (int i = 0; i < textArray.GetLength(0); i++)
            {
                if(textArray[i, 0] == aux)
                {
                    return textArray[i, indexLenguage];
                }
            }

            //UnityEngine.Debug.LogWarning("no se encontro el key en lenguage: " + aux);

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

        #if UNITY_EDITOR

        //csvArchiveRead = new StreamReader(path + read.name + ".txt");
        csvArchiveWrite = new StreamWriter(path + write.name + ".txt", false ,System.Text.Encoding.UTF8);

        #endif

        toWrite = read.text;

        //csvArchiveRead.Close();

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

        //lenguage = SaveWithJSON.LoadFromPictionary("Lenguage",lenguage);

        RefreshLenguage();
    }

 
    public void OnDestroy()
    {
        #if UNITY_EDITOR
        if (keysNotFinded.Count > 0)
        {
            csvArchiveWrite.Write(toWrite + csvArchiveWrite.NewLine + string.Join(csvArchiveWrite.NewLine, keysNotFinded.OrderBy(str => str).ToArray()));
        }
        csvArchiveWrite.Close();
        #endif
    }
}
