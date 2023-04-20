using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSystem : SingletonMono<LoadSystem>
{

    List<WaitForCorutines.MyCoroutine> preLoad = new List<WaitForCorutines.MyCoroutine>();

    List<WaitForCorutines.MyCoroutine> postLoad = new List<WaitForCorutines.MyCoroutine>();

    [SerializeReference]
    LoadScreen loadScreen;

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

        StartCoroutine(Init((b)=>{ }, (s)=> { loadScreen.Progress(s);  }));
        preLoad.Add(loadScreen.LoadImage);
    }


    public static void AddPreLoadCorutine(WaitForCorutines.MyCoroutine myCoroutine)
    {
        instance.preLoad.Add(myCoroutine);
    }

    public static void AddPostLoadCorutine(WaitForCorutines.MyCoroutine myCoroutine)
    {
        instance.postLoad.Add(myCoroutine);
    }

    public void Load(string scn)
    {
        loadScreen.Open();
        StartCoroutine(LoadScene(scn));
        Time.timeScale = 1;
    }

    public void Reload()
    {
        Load(SceneManager.GetActiveScene().name);
    }

    IEnumerator Init(System.Action<bool> end, System.Action<string> msg)
    {
        msg("loading script scene");
        yield return null;

        for (int i = 0; i < postLoad.Count; i++)
        {
            yield return new WaitForCorutines(this, postLoad[i], (s) => loadScreen.Progress((((i + 1f) / (postLoad.Count)) * (1f / 3) + (2f / 3)) * 100, s));
        }

        end(true);
    }

    IEnumerator LoadImage(System.Action<bool> end, System.Action<string> msg)
    {
        msg("LoadLoadSystem");
        yield return null;

        for (int i = 0; i < postLoad.Count; i++)
        {
            yield return new WaitForCorutines(this, postLoad[i], (s) => loadScreen.Progress((((i + 1f) / (postLoad.Count)) * (1f / 3) + (2f / 3)) * 100, s));
        }

        end(true);
    }

    IEnumerator LoadScene(string scene)
    {
        loadScreen.Progress(0, "Start loading");

        //loadscene = true;
        for (int i = 0; i < preLoad.Count; i++)
        {
            yield return new WaitForCorutines(this, preLoad[i], (s) => loadScreen.Progress(((i+1f) / (preLoad.Count))  * 100 * (1f / 3), s));
        }

        AsyncOperation async = SceneManager.LoadSceneAsync(scene);
        while (!async.isDone)
        {
            loadScreen.Progress( ((1f / 3) + async.progress *(1f / 3)) * 100, "loading scene");
            yield return null;
        }

        //awake ejecutar
        yield return new WaitForCorutines(this, Init, (s) => loadScreen.Progress(s));

        loadScreen.Progress(100,"<size=50>Carga finalizada</size>" +
            "\n<size=20> Presione <color=green>espacio</color> para continuar </size>");

        while (!Input.GetKeyDown(KeyCode.Space) && !(Input.touches.Length > 0))
        {
            yield return null;
        }

        Time.timeScale = 1;

        /*
        for (int i = 0; i < postLoad.Count; i++)
        {
            yield return new WaitForCorutines(this, postLoad[i], (s)=> loadScreen.Progress((((i+1f) / (postLoad.Count)) * (1f / 3) + (2f / 3)) * 100, s));
        }
        */

        preLoad.Clear();
        postLoad.Clear();

        loadScreen.Close();

        preLoad.Add(loadScreen.LoadImage);
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
