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
    public ItemBase weaponForPlayer;

    public Pictionarys<string, UnityEngine.Events.UnityEvent> betterEvents;

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
        dialogText = UI.Interfaz.SearchTitle("Subtitulo");
        dialogText.off += EndDialog;

        //StartCoroutine(PlayTutorial());
    }
    void AddToEvents()
    {
        player.move.onTeleport += TeleportEvent;

        //playerIA = player.CurrentState;

        /*
        if(dummy != null)
            dummy.onTakeDamage += AttackDummyEvent;
        */

        var title = UI.Interfaz.SearchTitle("Titulo");
        var titleSec = UI.Interfaz.SearchTitle("Titulo secundario");

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
        if(player.CurrentState==null)
            player.CurrentState = playerIA;
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
    public void SpawnExplotion(Transform lever)
    {
        Damage dmg = Damage.Create<DamageTypes.Perforation>(40);

        player.TakeDamage(dmg);

        var index = PoolManager.SrchInCategory("Particles", "SmokeyExplosion 2");
        PoolManager.SpawnPoolObject(index, lever.position, Quaternion.identity, player.transform);

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

    public void GiveToPlayer(ItemBase item, int amount)
    {
        player.GetInContainer<InventoryEntityComponent>().AddItem(item, amount);
    }

    public void SetPlayerAbility(AbilityToEquip abilityTo)
    {
        player.caster.SetAbility(abilityTo);
    }

    public void CallBetterEvent(string key)
    {
        betterEvents[key].Invoke();
    }
    
    public void NextDialog()
    {
        if (currentDialog >= allDialogs.Length)
            return;

        dialogText.AddMsg(allDialogs[currentDialog].dialog);

        allDialogs[currentDialog].logicActive?.Activate(this);

        if (nextDialog)
            currentDialog++;

        if (!DialogEnable)
            npc.interactuable = false;

        if(playerIA==null && player.CurrentState!=null)
            playerIA = player.CurrentState;

        player.CurrentState = null;
    }

}

[System.Serializable]
public struct DialogEvents
{
    [TextArea(6,12)]
    public string dialog;
    public LogicActive<TutorialScenaryManager> logicActive;
}

public interface Tutorial
{
    //public DialogEvents[];
}