using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSystem : SingletonMono<LoadSystem>
{
    static List<WaitForCorutines.MyCoroutine> preLoad = new List<WaitForCorutines.MyCoroutine>();
    static List<WaitForCorutines.MyCoroutine> postLoad = new List<WaitForCorutines.MyCoroutine>();

    static List<System.Action> preLoadEvent = new List<System.Action>();
    static List<System.Action> postLoadEvent = new List<System.Action>();

    [SerializeReference]
    LoadScreen loadScreen;

    public bool loadPause;

    public string[] noSerailizeProperties;

    [SerializeReference]
    SaveWithJSON saveWithJSON;

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

    public void Reload()
    {
        Load(SceneManager.GetActiveScene().name, loadPause);
    }

    IEnumerator LoadScene(string scene)
    {
        loadScreen.Progress(0, "Start loading");

        yield return null;

        //loadscene = true;
        for (int i = 0; i < preLoad.Count; i++)
        {
            yield return new WaitForCorutines(this, preLoad[i], (s) => loadScreen.Progress((((i + 1f) / (preLoad.Count)) * (1f / 6) + 0) * 100, s));
        }

        loadScreen.Progress("Close action scripts");

        yield return null;

        for (int i = 0; i < preLoadEvent.Count; i++)
        {
            preLoadEvent[i]();
            loadScreen.Progress((((i + 1f) / (preLoadEvent.Count)) * (1f / 6) + (1f / 6)) * 100);
            if (i % 100 == 0)
                yield return null;
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
            
            if(i % 100 == 0)
                yield return null;
        }

        loadScreen.Progress(100, "<size=50>Carga finalizada</size>");

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
