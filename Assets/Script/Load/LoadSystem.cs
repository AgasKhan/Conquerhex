using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSystem : MonoBehaviour
{
    public static LoadSystem instance;

    List<WaitForCorutines.MyCoroutine> preLoad = new List<WaitForCorutines.MyCoroutine>();

    List<WaitForCorutines.MyCoroutine> postLoad = new List<WaitForCorutines.MyCoroutine>();

    [SerializeReference]
    LoadScreen loadScreen;

    // Start is called before the first frame update
    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        DontDestroyOnLoad(gameObject);
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

    IEnumerator LoadScene(string scene)
    {
        loadScreen.Progress(0, "Empieza la carga");

        //loadscene = true;
        for (int i = 0; i < preLoad.Count; i++)
        {
            yield return new WaitForCorutines(this, preLoad[i], (s) => loadScreen.Progress(((i+1f) / (preLoad.Count))  * 100 * (1f / 3), s));
        }

        AsyncOperation async = SceneManager.LoadSceneAsync(scene);
        while (!async.isDone)
        {
            loadScreen.Progress( ((1f / 3) + async.progress *(1f / 3)) * 100, "Cargando la escena");
            yield return null;
        }

        //awake ejecutar
        yield return null;

        for (int i = 0; i < postLoad.Count; i++)
        {
            yield return new WaitForCorutines(this, postLoad[i], (s)=> loadScreen.Progress((((i+1f) / (postLoad.Count)) * (1f / 3) + (2f / 3)) * 100, s));
        }

        loadScreen.Progress(100, "Termino la carga");

        preLoad.Clear();
        postLoad.Clear();

        loadScreen.Close();
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
