using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [SerializeField]
    DialogEvents[] allDialogs;

    public Character player;

    public Hexagone[] newBorders = new Hexagone[6];
    public Hexagone firstHexagon;

    public GameObject dirigible;

    int currentDialog = 0;
    bool masterBool = false;
    public int tpsCounter = 0;

    [HideInInspector]
    public bool dialogEnable = true;

    public UnityEngine.UI.Button DialogButton;
    TextCompleto dialogText;
    IState<Character> playerIA;

    void Awake()
    {
        currentDialog = 0;
        tpsCounter = 0;
       

        DialogButton.onClick.RemoveAllListeners();
        DialogButton.onClick.AddListener(NextDialog);

        LoadSystem.AddPostLoadCorutine(AddToEvents);
    }
    private void Start()
    {
        dialogText = Interfaz.SearchTitle("Subtitulo");
        dialogText.off += EndDialog;

        //StartCoroutine(PlayTutorial());
    }
    void AddToEvents()
    {
        player.move.onTeleport += TeleportEvent;

        playerIA = player.CurrentState;
    }

    void EndDialog()
    {
        player.CurrentState = playerIA;
    }
    bool nieve = false;
    bool desierto = false;
    private void TeleportEvent(Hexagone arg1, int arg2)
    {
        if (currentDialog == 1)
        {
            nextDialog = false;
            tpsCounter++;
            EnableButton();

            if (tpsCounter >= 3)
            {
                nextDialog = true;
                EnableButton();
            }
                
        }

        if (currentDialog > 2)
        {
            if (arg1.biomes.nameDisplay == "Nieve")
                nieve = true;
            if (arg1.biomes.nameDisplay == "Desierto")
                desierto = true;

            if (nieve && desierto)
            {
                EnableButton();
                player.move.onTeleport -= TeleportEvent;
            }
        }

    }

    void EnableButton()
    {
        dialogEnable = true;

        if (!DialogButton.gameObject.activeSelf)
            DialogButton.gameObject.SetActive(true);
    }

    bool nextDialog = true;
    void NextDialog()
    {
        player.CurrentState = null;
        
        if (currentDialog >= allDialogs.Length)
            return;

        dialogText.AddMsg(allDialogs[currentDialog].dialog);

        allDialogs[currentDialog].logicActive?.Activate(this);

        if(nextDialog)
            currentDialog++;

        if (!dialogEnable)
            DialogButton.gameObject.SetActive(false);
    }

    void ButtonAction()
    {
        masterBool = true;
        NextDialog();
        DialogButton.gameObject.SetActive(false);
    }

    IEnumerator PlayTutorial()
    {
        while (currentDialog < allDialogs.Length)
        {
            while (masterBool == false)
            {
                yield return null;
            }
            NextDialog();

            masterBool = false;
        }
    }
}

[System.Serializable]
public struct DialogEvents
{
    public string dialog;
    public LogicActive logicActive;
}
