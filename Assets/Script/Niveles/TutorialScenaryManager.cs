using System.Collections.Generic;
using UnityEngine;

public class TutorialScenaryManager : SingletonMono<TutorialScenaryManager>
{
    [SerializeField]
    DialogEvents[] allDialogs;

    int currentDialog = 0;
    UI.TextCompleto dialogText;
    UI.Interfaz interfaz;

    [Header("References")]
    public GameObject goal;
    public Character player;
    public Transform NPC;

    public float maxDist = 10;
    IState<Character> playerIA;

    [Header("Scenary")]
    public Hexagone[] newBorders = new Hexagone[6];
    public Hexagone firstHexagon;
    public int tpsCounter = 0;
    bool nieve = false;
    bool desierto = false;

    public float damageGivenByExplosion = 25;

    [Header("Combat")]
    public DestructibleObjects dummy;
    public int attacksCounter = 0;
    public Ingredient weaponForPlayer;
    public List<AttackBase.AbilityToEquip> abilitiesForPlayer = new List<AttackBase.AbilityToEquip>();
    bool weaponGive = false;


    bool ability0 = false;
    bool ability1 = false;
    bool ability3 = false;
    bool ability4 = false;


    Timer timerToEvents;
    Timer timerToNextDialog;
    bool isShowingADialogue = false;
    string messageToShow = "";



    System.Action dialogueManagment;

    System.Action<Hexagone, int> teleportEvent;
    System.Action<int, MeleeWeapon> weaponEvent;
    System.Action<Damage> dummyTakeDamageEvent;
    System.Action<Health> healthEvent;

    bool distanceCheck;

    public void SetHexagons()
    {
        firstHexagon.ladosArray = newBorders;
        HexagonsManager.SetRenders(firstHexagon);
    }

    #region DISTANCE ULTRA HARDCODEADO
    public void NPCDistance()
    {
        float dist = Vector3.Distance(player.transform.position, NPC.position);

        if (dist >= maxDist)
        {
            NextDialog();
            distanceCheck = false;
        }
    }

    #endregion

    public void ChangeColorToLocked()
    {
        HexagonsManager.SetColorHexagone(firstHexagon, Color.red);
    }

    public void ChangeColorToFortress()
    {
        HexagonsManager.SetColorHexagone(firstHexagon, Color.yellow);
    }

    public void ChangeColorToBase()
    {
        HexagonsManager.SetColorHexagone(firstHexagon, Color.blue);
    }

    public void DistanceBoolSet(bool e) => distanceCheck = e;


    public void EnableExit()
    {
        goal.SetActive(true);
    }

    public void SpawnExplotion(Transform lever)
    {
        Damage dmg = Damage.Create<DamageTypes.Perforation>(damageGivenByExplosion);

        player.TakeDamage(dmg);

        var index = PoolManager.SrchInCategory("Particles", "SmokeyExplosion 2");
        PoolManager.SpawnPoolObject(index, lever.position, Quaternion.identity);
    }

    public void WaitToEnableDialog(float time)
    {
        if (timerToNextDialog.Chck)
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
        if (obj.actualLife >= obj.maxLife && obj.actualRegen >= obj.maxRegen)
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
        if (arg1 >= 0)
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
        {
            nieve = true;

            interfaz.CompleteObjective(1);
        }

        if (arg1.biomes.nameDisplay == "Desierto")
        {
            desierto = true;
            string obj = allDialogs[currentDialog].objective;

            interfaz.CompleteObjective(0);
        }

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

    public void SetPlayerAbility(AttackBase.AbilityToEquip abilityTo)
    {
        player.caster.SetAbility(abilityTo);
    }

    public void SetPlayerAbility()
    {
        for (int i = 0; i < abilitiesForPlayer.Count; i++)
        {
            abilitiesForPlayer[i].isBlocked = false;
            SetPlayerAbility(abilitiesForPlayer[i]);
        }

        player.caster.abilities[0].equiped.onApplyCast += EquipedOnCast0;
        player.caster.abilities[1].equiped.onApplyCast += EquipedOnCast1;
        //player.caster.abilities[3].equiped.onCast += EquipedOnCast3;
        //player.caster.abilities[4].equiped.onCast += EquipedOnCast4;
    }

    public void SetParryAbility()
    {
        abilitiesForPlayer[0].isBlocked = false;
        SetPlayerAbility(abilitiesForPlayer[0]);
        player.caster.abilities[0].equiped.onApplyCast += SetParry;
    }

    public void SetDashAbility()
    {
        abilitiesForPlayer[1].isBlocked = false;
        SetPlayerAbility(abilitiesForPlayer[1]);
        player.caster.abilities[1].equiped.onApplyCast += SetDash;
    }

    void SetParry(Ability a)
    {
        player.caster.abilities[0].equiped.onApplyCast -= SetParry;
        interfaz.CompleteObjective(0);
        NextDialog();
    }

    void SetDash(Ability a)
    {
        player.caster.abilities[1].equiped.onApplyCast -= SetDash;
        interfaz.CompleteObjective(1);
        NextDialog();
    }

    private void EquipedOnCast0(Ability ability)
    {
        ability0 = true;
        player.caster.abilities[0].equiped.onApplyCast -= EquipedOnCast0;

        interfaz.CompleteObjective(0);

        if (ability0 && ability1/* && ability3 && ability4 */)
            NextDialog();
    }

    private void EquipedOnCast1(Ability ability)
    {
        ability1 = true;
        player.caster.abilities[1].equiped.onApplyCast -= EquipedOnCast1;

        interfaz.CompleteObjective(1);

        if (ability0 && ability1/* && ability3 && ability4 */)
            NextDialog();
    }
    private void EquipedOnCast3(Ability ability)
    {
        ability3 = true;
        player.caster.abilities[3].equiped.onApplyCast -= EquipedOnCast3;

        interfaz.CompleteObjective(2);

        if (ability0 && ability1 && ability3 && ability4)
            NextDialog();
    }
    private void EquipedOnCast4(Ability ability)
    {
        ability4 = true;
        player.caster.abilities[4].equiped.onApplyCast -= EquipedOnCast4;

        interfaz.CompleteObjective(3);

        if (ability0 && ability1 && ability3 && ability4)
            NextDialog();
    }

    #endregion


    public void FinishCurrentDialogue()
    {
        dialogText.CompleteMsg();
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

    [ContextMenu("Next dialog")]
    public void NextDialog()
    {
        currentDialog++;

        interfaz.CompleteAllObjective();
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

        interfaz.ClearObjective();

        foreach (var item in allDialogs[currentDialog].objective.Split('\n'))
        {
            interfaz.AddObjective(item);
        }

        dialogText.ClearMsg();
        dialogText.AddMsg(messageToShow);

        dialogueManagment = () => FinishCurrentDialogue();

        if (allDialogs[currentDialog].timeToCallEvent != 0)
        {
            timerToEvents.Set(allDialogs[currentDialog].timeToCallEvent);
        }
        else
        {
            allDialogs[currentDialog].logicActive?.Invoke();
        }

        if (playerIA == null && player.CurrentState != null)
            playerIA = player.CurrentState;

        player.CurrentState = null;

        TimersManager.Create(0.2f, () => isShowingADialogue = true);

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

        if (dummy != null)
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
            titleSec.AddMsg("Simulaci�n corrupta");
        });
    }


    private void Start()
    {
        dialogText = UI.Interfaz.SearchTitle("Subtitulo");

        interfaz = UI.Interfaz.instance;

        dialogText.off += EndDialog;
    }

    void Update()
    {
        if (distanceCheck)
            NPCDistance();
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
    [TextArea(6, 12)]
    public string dialog;
    [TextArea(6, 12)]
    public string objective;
    public UnityEngine.Events.UnityEvent logicActive;
    public float timeToCallEvent;
}