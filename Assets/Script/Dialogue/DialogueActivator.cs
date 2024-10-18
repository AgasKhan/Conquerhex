using System.Collections;
using System.Collections.Generic;
using DialogueSystem;
using UnityEngine;

public class DialogueActivator : LogicActive<(InteractEntityComponent interact, Character character)>
{
    [SerializeField] private DialogueNew _dialogueToShow;
    [SerializeField] private string _entityName;
    
    public override void Activate((InteractEntityComponent interact, Character character) genericParams)
    {
        DialogueManager.instance.StartDialogue(_entityName, _dialogueToShow.RootNode);
    }
}
