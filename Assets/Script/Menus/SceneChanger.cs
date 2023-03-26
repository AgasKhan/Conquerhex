using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class SceneChanger : MonoBehaviour
{
    public Animator transition;

    bool isCharging = false;

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ReloadSc()
    {
        StartCoroutine(LoadSc(SceneManager.GetActiveScene().name));
    }

    public void Load(string scn, bool charge)
    {
        if (isCharging)
            return;

        //Controllers.MouseLock();
        Cursor.lockState = CursorLockMode.Locked;
        /*
        int index = scn.IndexOf('_');
        if (index > -1)
        {
            int numberLevel = System.Convert.ToInt32(scn.Substring(index + 1));
            CSVReader.SaveInPictionary<int>("CurrentLevel", numberLevel);
            BaseData.currentLevel = numberLevel;

            foreach (var item in Quests.SrchIncomplete(numberLevel))
            {
                item.active = true;
            }
        }
        else if (!charge)
        {
            foreach (var item in Quests.incomplete)
            {
                item.active = false;
            }
        }

        print("mandaste a cargar");
        */
        isCharging = true;
        
        StartCoroutine(LoadSc(scn));
        
    }

    public void Load(string scn)
    {
        Load(scn, false);
    }


    IEnumerator LoadSc(string sceneName)
    {
        Time.timeScale = 1;


        yield return null;

        Time.timeScale = 1;

        AsyncOperation async = SceneManager.LoadSceneAsync(sceneName);

        while (!async.isDone)
        {
            Debug.Log("Load: " + async.progress);

            yield return null;
        }

    }

}
