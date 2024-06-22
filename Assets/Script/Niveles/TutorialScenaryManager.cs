using System.Collections.Generic;
using UnityEngine;

public class TutorialScenaryManager : SingletonMono<TutorialScenaryManager>
{
    [SerializeField]
    DialogEvents[] allDialogs;

    int currentDialog = 0;
    UI.TextCompleto dialogText;
    UI.TextCompleto objectiveText;

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

    bool ability0 = false;
    bool ability1 = false;
    bool ability3 = false;
    bool ability4 = false;


    Timer timerToEvents;
    Timer timerToNextDialog;
    bool isShowingADialogue = false;
    string messageToShow = "";

    string objectiveToShow => string.Join('\n', _objectivesToShow);

    List<string> _objectivesToShow = new List<string>();

    System.Action dialogueManagment;

    System.Action<Hexagone,int> teleportEvent;
    System.Action<int, MeleeWeapon> weaponEvent;
    System.Action<Damage> dummyTakeDamageEvent;
    System.Action<Health> healthEvent;

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

    public void HealthDialog()
    {
        healthEvent = HealthEvent2;
    }

    void HealthEvent2(Health obj)
    {
        if(obj.actualLife==obj.maxLife && obj.actualRegen == obj.maxRegen)
        {
            healthEvent = null;
            NextDialog();
        }
    }


    public void TakeDamageDummyDialog()
    {
        dummyTakeDamageEvent = TakeDamageDummy;
    }

    void TakeDamageDummy(Damage dmg)
    {
        NextDialog();
        dummyTakeDamageEvent = null;
    }

    private void EquipeWeapon(int arg1, MeleeWeapon arg2)
    {
        if(arg1>=0)
        {
            NextDialog();
            weaponEvent = null;
        }
    }

    void Teleport1(Hexagone arg1, int arg2)
    {
        NextDialog();
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
            NextDialog();
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

    #region abilities

    public void SetPlayerAbility(AbilityToEquip abilityTo)
    {
        player.caster.SetAbility(abilityTo);
    }

    public void SetPlayerAbility()
    {
        for (int i = 0; i < abilitiesForPlayer.Count; i++)
        {
            SetPlayerAbility(abilitiesForPlayer[i]);
        }

        player.caster.abilities[0].equiped.onCast += EquipedOnCast0;
        player.caster.abilities[1].equiped.onCast += EquipedOnCast1;
        player.caster.abilities[3].equiped.onCast += EquipedOnCast3;
        player.caster.abilities[4].equiped.onCast += EquipedOnCast4;
    }

    private void EquipedOnCast0()
    {
        ability0 = true;
        player.caster.abilities[0].equiped.onCast -= EquipedOnCast0;

        _objectivesToShow[0] = _objectivesToShow[0].RichText("s");

        objectiveText.ShowMsg("Objetivos:\n" + objectiveToShow);

        if (ability0 && ability1 && ability3 && ability4)
            NextDialog();
    }
    private void EquipedOnCast1()
    {
        ability1 = true;
        player.caster.abilities[1].equiped.onCast -= EquipedOnCast1;

        _objectivesToShow[1] = _objectivesToShow[1].RichText("s");

        objectiveText.ShowMsg("Objetivos:\n" + objectiveToShow);

        if (ability0 && ability1 && ability3 && ability4)
            NextDialog();
    }
    private void EquipedOnCast3()
    {
        ability3 = true;
        player.caster.abilities[3].equiped.onCast -= EquipedOnCast3;

        _objectivesToShow[2] = _objectivesToShow[2].RichText("s");

        objectiveText.ShowMsg("Objetivos:\n" + objectiveToShow);

        if (ability0 && ability1 && ability3 && ability4)
            NextDialog();
    }
    private void EquipedOnCast4()
    {
        ability4 = true;
        player.caster.abilities[4].equiped.onCast -= EquipedOnCast4;

        _objectivesToShow[3] = _objectivesToShow[3].RichText("s");

        objectiveText.ShowMsg("Objetivos:\n" + objectiveToShow);

        if (ability0 && ability1 && ability3 && ability4)
            NextDialog();
    }

    #endregion


    public void FinishCurrentDialogue()
    {
        dialogText.AcelerateMsg(dialogText.velocityMultiply * 2);
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

        objectiveText.CompleteEffect();

        objectiveText.ShowMsg("Objetivos:\n" + objectiveToShow.RichText("s"));
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

        _objectivesToShow.Clear();

        _objectivesToShow.AddRange(allDialogs[currentDialog].objective.Split('\n'));

        dialogText.ClearMsg();
        dialogText.AddMsg(messageToShow);

        objectiveText.ClearMsg();
        objectiveText.AddMsg("Objetivos:\n"+ objectiveToShow);

        dialogueManagment = ()=> FinishCurrentDialogue();

        if (allDialogs[currentDialog].timeToCallEvent != 0)
        {
            timerToEvents.Set(allDialogs[currentDialog].timeToCallEvent);
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

        //VirtualControllers.Interact.eventDown -= InteractDialog;
        //VirtualControllers.Principal.eventDown -= InteractDialog;
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

    private void HealthEvent(Health obj)
    {
        healthEvent?.Invoke(obj);
    }


    void Init()
    {
        player.move.onTeleport += TeleportEvent;

        player.caster.weapons[0].toChange += WeaponEvent;

        player.health.helthUpdate += HealthEvent;

        if (dummy!=null)
            dummy.onTakeDamage += DummyTakeDamage;

        var title = UI.Interfaz.SearchTitle("Titulo");
        var titleSec = UI.Interfaz.SearchTitle("Titulo secundario");

        title.ClearMsg();
        titleSec.ClearMsg();

        timerToNextDialog = TimersManager.Create(1, NextDialog).Stop().SetInitCurrent(0);

        timerToEvents = TimersManager.Create(2, () => allDialogs[currentDialog].logicActive?.Invoke()).Stop();

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
        objectiveText = UI.Interfaz.SearchTitle("Objetivos");
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
    [TextArea(6, 12)]
    public string objective;
    public UnityEngine.Events.UnityEvent logicActive;
    public float timeToCallEvent;
}