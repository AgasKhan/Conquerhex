using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI;

public class TutorialCombatManager : SingletonMono<TutorialCombatManager>
{
    [SerializeField]
    DialogEvents[] allDialogs;

    public Character player;
    public NPCTutorial npc;

    public GameObject dirigible;

    int currentDialog = 0;

    [HideInInspector]
    public bool dialogEnable = true;

    TextCompleto dialogText;
    IState<Character> playerIA;

    protected override void Awake()
    {
        base.Awake();
        currentDialog = 0;

        LoadSystem.AddPostLoadCorutine(AddToEvents);
    }

    void Start()
    {
        dialogText = Interfaz.SearchTitle("Subtitulo");
        dialogText.off += EndDialog;
    }

    void AddToEvents()
    {
        playerIA = player.CurrentState;
    }

    void EndDialog()
    {
        player.CurrentState = playerIA;
    }

    void EnableButton()
    {
        dialogEnable = true;
        ((Interactuable)npc).interactuable = true;
    }

    bool nextDialog = true;
    public void NextDialog()
    {
        if (currentDialog >= allDialogs.Length)
            return;

        dialogText.AddMsg(allDialogs[currentDialog].dialog);

        allDialogs[currentDialog].logicActive?.Activate(this);

        if (nextDialog)
            currentDialog++;

        if (!dialogEnable)
            ((Interactuable)npc).interactuable = false;

        player.CurrentState = null;
    }
}
