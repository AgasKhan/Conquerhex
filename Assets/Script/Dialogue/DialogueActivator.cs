using System.Collections;
using System.Collections.Generic;
using DialogueSystem;
using UnityEngine;

public class DialogueActivator : MonoBehaviour
{
    [SerializeField] private DialogueNew _dialogueToShow;
    [SerializeField] private string _entityName;

    public void ShowDialogue()
    {
        DialogueManager.instance.StartDialogue(_entityName, _dialogueToShow.RootNode);
    }
}
