using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [SerializeField]
    Pictionarys<string, LogicActive> allDialogs = new Pictionarys<string, LogicActive>();

    int currentDialog;
    bool masterBool = false;

    void Awake()
    {
        LoadSystem.AddPostLoadCorutine(AddToEvents);
    }

    void Update()
    {
        
    }

    void AddToEvents()
    {

    }

    void NextDialog()
    {
        currentDialog++;

        if (allDialogs[currentDialog] != null)
        {
            allDialogs[currentDialog].Activate();
        }
    }

    IEnumerator PlayTutorial()
    {
        while (masterBool == false)
        {
            yield return null;
        }
        
    }
}
