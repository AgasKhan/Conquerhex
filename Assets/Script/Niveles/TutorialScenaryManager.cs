using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialScenaryManager : SingletonMono<TutorialScenaryManager>
{
    [SerializeField]
    DialogEvents[] allDialogs;
    int currentDialog = 0;
    TextCompleto dialogText;
    [HideInInspector]
    public bool dialogEnable = true;
    bool nextDialog = true;

    [Header("References")]
    public GameObject goal;
    public NPCTutorial npc;
    public Character player;
    IState<Character> playerIA;

    [Header("Scenary")]
    public Hexagone[] newBorders = new Hexagone[6];
    public Hexagone firstHexagon;
    public int tpsCounter = 0;
    bool nieve = false;
    bool desierto = false;

    [Header("Combat")]
    public DestructibleObjects dummy;
    public int attacksCounter = 0;

    protected override void Awake()
    {
        base.Awake();

        currentDialog = 0;
        tpsCounter = 0;

        attacksCounter = 0;

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

        if(dummy != null)
            dummy.onTakeDamage += AttackDummyEvent;

        var title = Interfaz.SearchTitle("Titulo");
        var titleSec = Interfaz.SearchTitle("Titulo secundario");

        title.ClearMsg();
        titleSec.ClearMsg();

        TimersManager.Create(2, () => 
        {
            title.AddMsg("Aviso");
        }
        );

        TimersManager.Create(3, () =>
        {
            titleSec.AddMsg("Simulación corrupta");
        }
);

    }

    void EndDialog()
    {
        player.CurrentState = playerIA;
    }

    private void AttackDummyEvent(Damage obj)
    {
        if (currentDialog == 1 && attacksCounter < 1)
        {
            TimersManager.Create(1, () => { NextDialog(); });
            
            attacksCounter++;

            Damage dmg = new Damage();

            dmg.typeInstance = (ClassDamage)Manager<ShowDetails>.pic["Perforation"];

            dmg.amount = 30;

            player.TakeDamage(dmg);

            var index = PoolManager.SrchInCategory("Particles", "SmokeyExplosion 2");
            PoolManager.SpawnPoolObject(index, player.transform.position, Quaternion.identity, player.transform);

            EnableButton();
        }

        if(currentDialog == 3)
        {
            attacksCounter++;

            if(attacksCounter >= 3)
            {
                EnableButton();
                dummy.onTakeDamage -= AttackDummyEvent;
            }
        }
    }

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
                currentDialog++;
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
        ((Interactuable) npc).visible = true;
    }

   
    public void NextDialog()
    {
        if (currentDialog >= allDialogs.Length)
            return;

        dialogText.AddMsg(allDialogs[currentDialog].dialog);

        allDialogs[currentDialog].logicActive?.Activate(this);

        if (nextDialog)
            currentDialog++;

        if (!dialogEnable)
            ((Interactuable)npc).visible = false;

        player.CurrentState = null;
    }

}

[System.Serializable]
public struct DialogEvents
{
    [TextArea(6,12)]
    public string dialog;
    public LogicActive logicActive;
}
