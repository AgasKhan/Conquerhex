using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogAction : InteractAction<(InteractEntityComponent interact, Character character)>
{
    [SerializeField]
    string dialog = "";
    UI.TextCompleto dialogText;
    Timer interactTim;
    InteractEntityComponent myInteract;

    private void Awake()
    {
        LoadSystem.AddPostLoadCorutine(() => {dialogText = UI.Interfaz.SearchTitle("Subtitulo"); });
        interactTim = TimersManager.Create(3, ()=> { myInteract.interactuable = true; }).Stop();
    }

    public override void Activate((InteractEntityComponent interact, Character character) genericParams)
    {
        myInteract = genericParams.interact;
        dialogText.AddMsg(dialog);
        myInteract.interactuable = false;
        interactTim.Reset();
    }
}