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

    int currentDialog = 0;
    bool masterBool = false;
    bool dialogAble = true;

    public UnityEngine.UI.Button DialogButton;
    TextCompleto dialogText;

    void Awake()
    {
        currentDialog = 0;
        
        DialogButton.onClick.RemoveAllListeners();
        DialogButton.onClick.AddListener(ButtonAction);

        LoadSystem.AddPostLoadCorutine(AddToEvents);
    }
    private void Start()
    {
        dialogText = Interfaz.SearchTitle("Subtitulo");
        dialogText.off += () => { DialogButton.SetActive(true); };

        StartCoroutine(PlayTutorial());
    }


    private void FirstTeleport(Hexagone arg1, int arg2)
    {
        dialogAble = true;
    }


    void AddToEvents()
    {
        player.move.onTeleport += FirstTeleport;
    }

    void NextDialog()
    {
        if (currentDialog >= allDialogs.Length)
            return;

        DialogButton.SetActive(false);

        dialogText.AddMsg(allDialogs[currentDialog].dialog);

        allDialogs[currentDialog].logicActive?.Activate(this);

        currentDialog++;
    }

    void ButtonAction()
    {
        masterBool = true;
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
