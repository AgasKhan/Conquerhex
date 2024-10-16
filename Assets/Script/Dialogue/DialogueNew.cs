
using UnityEngine;
using DialogueSystem;

[CreateAssetMenu(fileName = "NewDialogueNew", menuName = "DialogueSystem/Dialogo", order = 1)]
public class DialogueNew : ScriptableObject
{
    public DialogueNode RootNode;
    public AudioClip dialogueSound;

}

