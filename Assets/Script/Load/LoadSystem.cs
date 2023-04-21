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

    // Start is called before the first frame update
    protected override void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        DontDestroyOnLoad(gameObject);

        
        StartCoroutine(PostLoad((b)=>{ }, (s)=> { loadScreen.Progress(s);  }));
        preLoad.Add(loadScreen.LoadImage);
    }

    //delegado especifico

    public static void AddPreLoadCorutine(WaitForCorutines.MyCoroutine myCoroutine)
    {
        preLoad.Add(myCoroutine);
    }

    public static void AddPostLoadCorutine(WaitForCorutines.MyCoroutine myCoroutine)
    {
        postLoad.Add(myCoroutine);
    }

   
    //delegado adaptado
    public static void AddPreLoadCorutine(System.Action myCoroutine)
    {
        preLoadEvent.Add(myCoroutine);
    }

    public static void AddPostLoadCorutine(System.Action myCoroutine)
    {
        postLoadEvent.Add(myCoroutine);
    }

    public void Load(string scn, bool pause = false)
    {
        loadPause = pause;
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
            yield return new WaitForCorutines(this, preLoad[i], (s) => loadScreen.Progress(((i) / (preLoad.Count))  * 100 * (1f / 3), s));
        }

        foreach (var item in preLoadEvent)
        {
            item();
            yield return null;
        }
        

        loadScreen.Progress("Start scene load");

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

        //espera un frame a que se carguen todos los postload
        yield return null;

        for (int i = 0; i < postLoad.Count; i++)
        {
            yield return new WaitForCorutines(this, postLoad[i], (s) => loadScreen.Progress((((i + 1f) / (postLoad.Count)) * (1f / 3) + (2f / 3)) * 100, s));
        }

        foreach (var item in postLoadEvent)
        {
            item();
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

        preLoad.Clear();
        postLoad.Clear();

        postLoadEvent.Clear();
        preLoadEvent.Clear();

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
