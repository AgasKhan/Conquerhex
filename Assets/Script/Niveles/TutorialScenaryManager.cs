using System.Collections.Generic;
using UnityEngine;

public class TutorialScenaryManager : SingletonMono<TutorialScenaryManager>
{
    [SerializeField]
    DialogEvents[] allDialogs;

    int currentDialog = 0;
    UI.TextCompleto dialogText;

    [Header("References")]
    public GameObject goal;
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
    bool weaponGive = false;


    Timer timerToEvents;
    Timer timerToNextDialog;
    bool isShowingADialogue = false;
    string messageToShow = "";

    System.Action dialogueManagment;

    System.Action<Hexagone,int> teleportEvent;
    System.Action<int, MeleeWeapon> weaponEvent;
    System.Action<Damage> dummyTakeDamageEvent;

    public void SetHexagons()
    {
        firstHexagon.ladosArray = newBorders;
        HexagonsManager.SetRenders(firstHexagon);
    }

    public void EnableExit()
    {
        goal.SetActive(true);
    }

    public void SpawnExplotion(Transform lever)
    {
        Damage dmg = Damage.Create<DamageTypes.Perforation>(40);

        player.TakeDamage(dmg);

        var index = PoolManager.SrchInCategory("Particles", "SmokeyExplosion 2");
        PoolManager.SpawnPoolObject(index, lever.position, Quaternion.identity);
    }

    public void WaitToEnableDialog(float time)
    {
        if(timerToNextDialog.Chck)
            timerToNextDialog.Set(time);
    }

    public void Teleport1Dialog()
    {
        teleportEvent = Teleport1;
    }

    public void Teleport2Dialog()
    {
        teleportEvent = Teleport2;
    }

    public void EquipeWeaponDialog()
    {
        weaponEvent = EquipeWeapon;

        if (!weaponGive)
        {
            weaponGive = true;
            GiveToPlayer();
        }
    }
    public void TakeDamageDummyDialog()
    {
        dummyTakeDamageEvent = TakeDamageDummy;
    }

    void TakeDamageDummy(Damage dmg)
    {
        currentDialog++;
        dummyTakeDamageEvent = null;
    }

    private void EquipeWeapon(int arg1, MeleeWeapon arg2)
    {
        if(arg1>=0)
        {
            currentDialog++;
            weaponEvent = null;
        }
    }

    void Teleport1(Hexagone arg1, int arg2)
    {
        currentDialog++;
        teleportEvent -= Teleport1;
    }

    void Teleport2(Hexagone arg1, int arg2)
    {
        if (arg1.biomes.nameDisplay == "Nieve")
            nieve = true;
        if (arg1.biomes.nameDisplay == "Desierto")
            desierto = true;

        if (nieve && desierto)
        {
            currentDialog++;
            teleportEvent -= Teleport2;
        }
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

    public void FinishCurrentDialogue()
    {
        dialogText.AddAccelerationMsg(4);
        //dialogText.ShowMsg(messageToShow);
        //dialogueManagment = () => SkipDialogue();
    }

    public void SkipDialogue()
    {
        dialogText.ClearMsg();
        EndDialog();
        Debug.Log("SkipDial");
    }

    public void DialogueManagment()
    {
        dialogueManagment?.Invoke();
    }

    public void NextDialog()
    {
        currentDialog++;
    }

    void EndDialog()
    {
        if (player.CurrentState == null)
            player.CurrentState = playerIA;

        dialogueManagment = null;
        isShowingADialogue = false;

        VirtualControllers.Interact.eventDown -= InteractDialog;
        VirtualControllers.Principal.eventDown -= InteractDialog;
    }

    /// <summary>
    /// Show Dialog
    /// </summary>
    public void ShowDialog()
    {
        if (currentDialog >= allDialogs.Length)
            return;

        messageToShow = allDialogs[currentDialog].dialog;

        dialogText.ClearMsg();
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

        if(playerIA==null && player.CurrentState!=null)
            playerIA = player.CurrentState;

        player.CurrentState = null;

        TimersManager.Create(0.2f, ()=> isShowingADialogue = true);

        VirtualControllers.Interact.eventDown += InteractDialog;
        VirtualControllers.Principal.eventDown += InteractDialog;
    }

    void InteractDialog(Vector2 arg1, float arg2)
    {
        if (isShowingADialogue)
            DialogueManagment();

        VirtualControllers.Interact.eventDown -= InteractDialog;
        VirtualControllers.Principal.eventDown -= InteractDialog;
    }

    void TeleportEvent(Hexagone arg1, int arg2)
    {
        teleportEvent?.Invoke(arg1, arg2);
    }

    private void WeaponEvent(int arg1, MeleeWeapon arg2)
    {
        weaponEvent?.Invoke(arg1, arg2);
    }

    private void DummyTakeDamage(Damage obj)
    {
        dummyTakeDamageEvent?.Invoke(obj);
    }

    void Init()
    {
        player.move.onTeleport += TeleportEvent;

        player.caster.weapons[0].toChange += WeaponEvent;

        if(dummy!=null)
            dummy.onTakeDamage += DummyTakeDamage;

        var title = UI.Interfaz.SearchTitle("Titulo");
        var titleSec = UI.Interfaz.SearchTitle("Titulo secundario");

        title.ClearMsg();
        titleSec.ClearMsg();

        timerToNextDialog = TimersManager.Create(1, NextDialog).Stop().SetInitCurrent(0);

        TimersManager.Create(2, () =>
        {
            title.AddMsg("Aviso");
        });

        TimersManager.Create(3, () =>
        {
            titleSec.AddMsg("Simulación corrupta");
        });
    }



    private void Start()
    {
        dialogText = UI.Interfaz.SearchTitle("Subtitulo");
        dialogText.off += EndDialog;
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