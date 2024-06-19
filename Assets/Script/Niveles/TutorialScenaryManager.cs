using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialScenaryManager : SingletonMono<TutorialScenaryManager>
{
    [SerializeField]
    DialogEvents[] allDialogs;

    int currentDialog = 0;
    UI.TextCompleto dialogText;

    [HideInInspector]
    public bool DialogEnable 
    {
        get => _dialogEnable;
        set
        {
            npc.interactuable = value;
            _dialogEnable = value;
        } 
    }

    bool _dialogEnable = true;

    bool nextDialog = true;

    [Header("References")]
    public GameObject goal;
    public InteractEntityComponent npc;
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
    public Ingredient weaponForPlayer;
    public List<AbilityToEquip> abilitiesForPlayer = new List<AbilityToEquip>();

    Timer timerToEvents;
    bool isShowingADialogue = false;
    string messageToShow = "";

    System.Action dialogueManagment;

   

   





    void EndDialog()
    {
        if(player.CurrentState==null)
            player.CurrentState = playerIA;

        dialogueManagment = () => SkipDialogue();
        isShowingADialogue = false;
    }

    private void AttackDummyEvent(Damage obj)
    {
        if (currentDialog == 1 && attacksCounter < 1)
        {
            TimersManager.Create(1, () => { NextDialog(); });
            
            attacksCounter++;

            Damage dmg = Damage.Create<DamageTypes.Perforation>(30);

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
                //-----------------dummy.onTakeDamage -= AttackDummyEvent;
            }
        }
    }

    public void ChangeDialogueBool()
    {
        DialogEnable = false;
    }

    public void SetHexagons()
    {
        firstHexagon.ladosArray = newBorders;
        HexagonsManager.SetRenders(firstHexagon);
        DialogEnable = false;
    }

    public void EnableExit()
    {
        goal.SetActive(true);
        DialogEnable = false;
    }

    public void SpawnExplotion(Transform lever)
    {
        Damage dmg = Damage.Create<DamageTypes.Perforation>(40);

        player.TakeDamage(dmg);

        var index = PoolManager.SrchInCategory("Particles", "SmokeyExplosion 2");
        PoolManager.SpawnPoolObject(index, lever.position, Quaternion.identity);

        EnableButton();
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
        DialogEnable = true;
        npc.interactuable = true;
    }
    
    public void GiveToPlayer()
    {
        GiveToPlayer(weaponForPlayer.Item, weaponForPlayer.Amount);
    }

    public void GiveToPlayer(ItemBase item, int amount)
    {
        player.GetInContainer<InventoryEntityComponent>().AddItem(item, amount);
    }

    public void SetPlayerAbility()
    {
        for (int i = 0; i < abilitiesForPlayer.Count; i++)
        {
            SetPlayerAbility(abilitiesForPlayer[i]);
        }
    }

    public void SetPlayerAbility(AbilityToEquip abilityTo)
    {
        player.caster.SetAbility(abilityTo);
    }

    public void WaitToEnableDialog(float time)
    {
        DialogEnable = false;
        TimersManager.Create(time, () => DialogEnable = true).Reset();
    }
    
    public void SkipDialogue()
    {
        dialogText.ClearMsg();
        EndDialog();
        Debug.Log("SkipDial");
    }

    public void FinishCurrentDialogue()
    {
        dialogueManagment = () => SkipDialogue();
        dialogText.ShowMsg(messageToShow);
        Debug.Log("FinishCurrDial");
    }

    public void DialogueManagment()
    {
        dialogueManagment?.Invoke();
    }

    public void NextDialog()
    {
        if (currentDialog >= allDialogs.Length)
            return;

        messageToShow = allDialogs[currentDialog].dialog;
        dialogText.AddMsg(messageToShow);
        
        dialogueManagment = ()=> FinishCurrentDialogue();

        if (allDialogs[currentDialog].timeToCallEvent != 0)
        {
            int actualDialog = currentDialog;
            timerToEvents = TimersManager.Create(allDialogs[actualDialog].timeToCallEvent, ()=> allDialogs[actualDialog].logicActive?.Invoke());
            timerToEvents.Reset();
        }
        else
        {
            allDialogs[currentDialog].logicActive?.Invoke();
        }

        if (nextDialog)
            currentDialog++;

        if (!DialogEnable)
            npc.interactuable = false;

        if(playerIA==null && player.CurrentState!=null)
            playerIA = player.CurrentState;

        player.CurrentState = null;

        TimersManager.Create(0.2f, ()=> isShowingADialogue = true);
    }

    void Init()
    {
        player.move.onTeleport += TeleportEvent;

        var title = UI.Interfaz.SearchTitle("Titulo");
        var titleSec = UI.Interfaz.SearchTitle("Titulo secundario");

        title.ClearMsg();
        titleSec.ClearMsg();

        TimersManager.Create(2, () =>
        {
            title.AddMsg("Aviso");
        });

        TimersManager.Create(3, () =>
        {
            titleSec.AddMsg("Simulación corrupta");
            DialogEnable = true;
        });

    }

    private void Update()
    {
        if (isShowingADialogue && Input.GetKeyDown(KeyCode.F))
            DialogueManagment();
    }

    private void Start()
    {
        dialogText = UI.Interfaz.SearchTitle("Subtitulo");
        dialogText.off += EndDialog;
        dialogueManagment = () => NextDialog();
        DialogEnable = false;
        //StartCoroutine(PlayTutorial());
    }

    protected override void Awake()
    {
        base.Awake();

        currentDialog = 0;
        tpsCounter = 0;

        attacksCounter = 0;

        LoadSystem.AddPostLoadCorutine(Init);
    }
}

[System.Serializable]
public struct DialogEvents
{
    [TextArea(6,12)]
    public string dialog;
    public UnityEngine.Events.UnityEvent logicActive;
    public float timeToCallEvent;
}

public interface Tutorial
{
    //public DialogEvents[];
}