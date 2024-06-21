using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IAIO : IAFather
{
    [SerializeField]
    EventManager eventsManager;

    string originalTag;

    [SerializeField]
    Detect<InteractEntityComponent> detectInteractuable = new Detect<InteractEntityComponent>();

    [SerializeField]
    string lastCombo;

    string[] combos = new string[] { "↑↑","→→", "←←" , "↓↓" };

    Timer comboReset;

    InteractEntityComponent lastInteractuable;

    DoubleEvent<(IGetPercentage, float), (bool, bool, Sprite)> interactEvent;
    
    TripleEvent<(float, float, float), float, float> energyEvent;

    SingleEvent<Character> characterEvent;

    System.Action<(Ability, ItemBase)>[] equipedEvents = new System.Action<(Ability, ItemBase)>[12];

    System.Action<int, MeleeWeapon> meleeWeaponUIMediator;
    System.Action<int, WeaponKata>[] kataUIMediator = new System.Action<int, WeaponKata>[4];
    System.Action<int, AbilityExtCast>[] abilityExtUIMediator = new System.Action<int, AbilityExtCast>[6];


    void TriggerUI()
    {
        equipedEvents[0].Invoke((caster.weapons[0].equiped?.defaultKata, caster.weapons[0].equiped?.itemBase));
        equipedEvents[1].Invoke((caster.abilities[0].equiped, caster.abilities[0].equiped?.itemBase));
        equipedEvents[2].Invoke((caster.abilities[1].equiped, caster.abilities[1].equiped?.itemBase));

        for (int i = 0; i < 4 && (i) < caster.abilities.Count; i++)
        {
            equipedEvents[i + 3].Invoke((caster.katasCombo[i].equiped, caster.katasCombo[i].equiped?.itemBase));
        }

        for (int i = 0; i < 4 && (i + 2) < caster.abilities.Count; i++)
        {
            equipedEvents[i + 7].Invoke((caster.abilities[i + 2].equiped, caster.abilities[i + 2].equiped?.itemBase));
        }
    }

    void SuscribeUI()
    {
        caster.weapons[0].toChange += meleeWeaponUIMediator;
        caster.abilities[0].toChange += abilityExtUIMediator[0];
        caster.abilities[1].toChange += abilityExtUIMediator[1];

        for (int i = 0; i < 4 && (i) < caster.abilities.Count; i++)
        {
            caster.katasCombo[i].toChange += kataUIMediator[i];
        }

        for (int i = 0; i < 4 && (i + 2) < caster.abilities.Count; i++)
        {
            caster.abilities[i + 2].toChange += abilityExtUIMediator[i+2];
        }
    }

    void DesuscribiUI()
    {
        caster.weapons[0].toChange -= meleeWeaponUIMediator;
        caster.abilities[0].toChange -= abilityExtUIMediator[0];
        caster.abilities[1].toChange -= abilityExtUIMediator[1];

        for (int i = 0; i < 4 && (i) < caster.abilities.Count; i++)
        {
            caster.katasCombo[i].toChange -= kataUIMediator[i];
        }

        for (int i = 0; i < 4 && (i + 2) < caster.abilities.Count; i++)
        {
            caster.abilities[i + 2].toChange -= abilityExtUIMediator[i + 2];
        }
    }

    #region controles numericos

    void AlphaController(Controllers.Axis axis, EventControllerMediator eventControllerMediator, System.Action down)
    {
        VirtualControllers.Alpha1.DesuscribeController(eventControllerMediator);
        VirtualControllers.Alpha2.DesuscribeController(eventControllerMediator);
        VirtualControllers.Alpha3.DesuscribeController(eventControllerMediator);
        VirtualControllers.Alpha4.DesuscribeController(eventControllerMediator);
        character.castingActionCharacter.OnExit += () => axis.DesuscribeController(eventControllerMediator);
        axis.SuscribeController(eventControllerMediator);
        down();
    }

    private void Alpha1_eventDown(Vector2 arg1, float arg2)
    {
        lastCombo = "↑↑";
        if (!Input.GetKey(KeyCode.LeftControl))
            AlphaController(VirtualControllers.Alpha1, character.attackEventMediator, ()=> AttackEventMediator_eventDown(Vector2.zero, 0));
        else
            AlphaController(VirtualControllers.Alpha1, character.abilityEventMediator, () => AbilityEventMediator_eventDown(Vector2.zero, 0));
    }

    private void Alpha2_eventDown(Vector2 arg1, float arg2)
    {
        lastCombo = "→→";
        if (!Input.GetKey(KeyCode.LeftControl))
            AlphaController(VirtualControllers.Alpha2, character.attackEventMediator, () => AttackEventMediator_eventDown(Vector2.zero, 0));
        else
            AlphaController(VirtualControllers.Alpha2, character.abilityEventMediator, () => AbilityEventMediator_eventDown(Vector2.zero, 0));
    }

    private void Alpha3_eventDown(Vector2 arg1, float arg2)
    {
        lastCombo = "←←";
        if (!Input.GetKey(KeyCode.LeftControl))
            AlphaController(VirtualControllers.Alpha3, character.attackEventMediator, () => AttackEventMediator_eventDown(Vector2.zero, 0));
        else
            AlphaController(VirtualControllers.Alpha3, character.abilityEventMediator, () => AbilityEventMediator_eventDown(Vector2.zero, 0));
    }

    private void Alpha4_eventDown(Vector2 arg1, float arg2)
    {
        lastCombo = "↓↓";
        if (!Input.GetKey(KeyCode.LeftControl))
            AlphaController(VirtualControllers.Alpha4, character.attackEventMediator, () => AttackEventMediator_eventDown(Vector2.zero, 0));
        else
            AlphaController(VirtualControllers.Alpha4, character.abilityEventMediator, () => AbilityEventMediator_eventDown(Vector2.zero, 0));
    }

    #endregion

    public void ChangeCharacter(Character newCharacter)
    {
        characterEvent.delegato.Invoke(newCharacter);
    }

    private void MoveEventMediator_eventDown(Vector2 arg1, float arg2)
    {
        Vector2 tecla = arg1.AproxDir();

        if (tecla != Vector2.zero)
        {
            comboReset.Reset();

            if (lastCombo.Length >= 2)
                lastCombo = new string(lastCombo[^1], 1);

            if (tecla.x > 0)
            {
                lastCombo += "→";
            }
            else if (tecla.x < 0)
            {
                lastCombo += "←";
            }
            else if (tecla.y > 0)
            {
                lastCombo += "↑";
            }
            else if (tecla.y < 0)
            {
                lastCombo += "↓";
            }
        }
    }

    private void AttackEventMediator_eventDown(Vector2 arg1, float arg2)
    {
        for (int i = 0; i < combos.Length; i++)
        {
            if (combos[i] == lastCombo)
            {
                character.Attack(i + 1);
                return;
            }
        }

        character.Attack(0);
    }

    private void AbilityEventMediator_eventDown(Vector2 arg1, float arg2)
    {
        for (int i = 0; i < combos.Length; i++)
        {
            if (combos[i] == lastCombo)
            {
                character.Ability(i + 1);
                return;
            }
        }

        character.Ability(0);
    }

    private void DashEventMediator_eventDown(Vector2 arg1, float arg2)
    {
        character.AlternateAbility();
    }

    

    public override void OnEnterState(Character param)
    {
        base.OnEnterState(param);

        originalTag = param.gameObject.tag;

        param.gameObject.tag = "Player";

        param.move.onTeleport += TeleportEvent;

        //param.health.lifeUpdate += UpdateLife;
        //param.health.regenUpdate += UpdateRegen;
        //param.health.regenTimeUpdate += UpdateRegenTime;

        //param.onTakeDamage += OnTakeDamage;

        param.health.helthUpdate += Health_helthUpdate;

        param.caster.energyUpdate += EnergyUpdate;
        param.caster.leftEnergyUpdate += LeftEnergyUpdate;
        param.caster.rightEnergyUpdate += RightEnergyUpdate;

        SuscribeUI();

        TriggerUI();

        attackEventMediator.eventDown += AttackEventMediator_eventDown;

        abilityEventMediator.eventDown += AbilityEventMediator_eventDown;

        moveEventMediator.eventDown += MoveEventMediator_eventDown;

        dashEventMediator.eventDown += DashEventMediator_eventDown;

        VirtualControllers.Alpha1.eventDown += Alpha1_eventDown;
        VirtualControllers.Alpha2.eventDown += Alpha2_eventDown;
        VirtualControllers.Alpha3.eventDown += Alpha3_eventDown;
        VirtualControllers.Alpha4.eventDown += Alpha4_eventDown;

        VirtualControllers.Movement.SuscribeController(moveEventMediator);

        VirtualControllers.Principal.SuscribeController(attackEventMediator);

        VirtualControllers.Secondary.SuscribeController(abilityEventMediator);

        VirtualControllers.Terciary.SuscribeController(dashEventMediator);
    }

    public override void OnExitState(Character param)
    {
        param.move.onTeleport -= TeleportEvent;

        //param.health.lifeUpdate -= UpdateLife;
        //param.health.regenUpdate -= UpdateRegen;
        //param.health.regenTimeUpdate -= UpdateRegenTime;
        param.health.helthUpdate -= Health_helthUpdate;
        param.caster.energyUpdate -= EnergyUpdate;
        param.caster.leftEnergyUpdate -= LeftEnergyUpdate;
        param.caster.rightEnergyUpdate -= RightEnergyUpdate;
        //param.onTakeDamage -= OnTakeDamage;

        attackEventMediator.eventDown -= AttackEventMediator_eventDown;

        abilityEventMediator.eventDown -= AbilityEventMediator_eventDown;

        moveEventMediator.eventDown -= MoveEventMediator_eventDown;

        dashEventMediator.eventDown -= DashEventMediator_eventDown;

        DesuscribiUI();

        VirtualControllers.Alpha1.eventDown -= Alpha1_eventDown;
        VirtualControllers.Alpha2.eventDown -= Alpha2_eventDown;
        VirtualControllers.Alpha3.eventDown -= Alpha3_eventDown;
        VirtualControllers.Alpha4.eventDown -= Alpha4_eventDown;

        VirtualControllers.Movement.DesuscribeController(moveEventMediator);

        VirtualControllers.Principal.DesuscribeController(attackEventMediator);

        VirtualControllers.Secondary.DesuscribeController(abilityEventMediator);

        VirtualControllers.Terciary.DesuscribeController(dashEventMediator);

        VirtualControllers.Interact.eventDown -= Interact_eventDown;

        interactEvent.secondDelegato?.Invoke((false, false, null));



        lastInteractuable = null;
        param.gameObject.tag = originalTag;

        base.OnExitState(character);
    }

    public override void OnStayState(Character param)
    {
        base.OnStayState(param);

        //eventsManager.events.SearchOrCreate<SingleEvent<Vector3>>("move").delegato.Invoke(transform.position);

        var buildings = detectInteractuable.Area(character.transform.position, (edificio) => { return edificio.interactuable; });

        if (buildings == null || buildings.Count == 0)
        {
            interactEvent.secondDelegato?.Invoke((false, false, null));

            VirtualControllers.Interact.eventDown -= Interact_eventDown;

            lastInteractuable = null;
        }
        else if (buildings[0] != lastInteractuable)
        {
            VirtualControllers.Interact.eventDown -= Interact_eventDown;

            lastInteractuable = buildings[0];

            interactEvent.secondDelegato?.Invoke((true, false, lastInteractuable.Image));

            VirtualControllers.Interact.eventDown += Interact_eventDown;
        }
    }

    private void Interact_eventDown(Vector2 arg1, float arg2)
    {
        lastInteractuable.Interact(character);
    }

    private void TeleportEvent(Hexagone obj, int lado)
    {
        HexagonsManager.SetRenders(obj, HexagonsManager.LadoOpuesto(lado), obj.AristaMasCercana(character.transform));


        if (HexagonsManager.idMaxLevel == obj.id)
        {
            UI.Interfaz.instance?["Titulo secundario"].ShowMsg("Árbol dorado a la vista");
        }
        else
        {
            for (int i = 0; i < obj.ladosArray.Length; i++)
            {
                if(obj.ladosArray[i].id == HexagonsManager.idMaxLevel)
                {
                    UI.Interfaz.instance?["Titulo secundario"].ShowMsg("Árbol dorado cercano en los alrededores");
                }
            }
        }
    }

    protected override void Health_death()
    {
        var chr = character;

        character.CurrentState = null;

        TimersManager.Create(0.5f, ()=>
        {
            if (chr != GameManager.instance.playerCharacter)
            {
                enabled = true;
                chr.SetActiveGameObject(false);
                VirtualControllers.Principal.eventDown += NoCharacterSelected;
            }
            else
            {
                GameManager.instance.Defeat("Has muerto");
            }
        });
    }

    private void Health_helthUpdate(Health obj)
    {
        eventsManager.events.SearchOrCreate<SingleEvent<Health>>(LifeType.all).delegato?.Invoke(obj);
    }

    private void EnergyUpdate((float,float,float) obj)
    {
        energyEvent.delegato?.Invoke(obj);
    }

    private void LeftEnergyUpdate(float obj)
    {
        energyEvent.secondDelegato?.Invoke(obj);
    }

    private void RightEnergyUpdate(float obj)
    {
        energyEvent.thirdDelegato?.Invoke(obj);
    }

    private void OnCharacterSelected(Character chara)
    {
        if(character!=null)
            character.CurrentState = character.GetComponent<IAFather>();

        chara.CurrentState = this;
    }

    private void NoCharacterSelected(Vector2 arg1, float arg2)
    {
        characterEvent.delegato.Invoke(GameManager.instance.playerCharacter);
        VirtualControllers.Principal.eventDown -= NoCharacterSelected;
    }

    private void Update()
    {
        if (character != null)
        {
            UI.Interfaz.instance?["Titulo secundario"].ClearMsg();
            enabled = false;
            return;
        }

        UI.Interfaz.instance?["Titulo secundario"].ShowMsg("Presiona click izquierdo para volver a tu cuerpo");
    }

    private void Awake()
    {
        comboReset = TimersManager.Create(0.5f, () => lastCombo = string.Empty);
        characterEvent = eventsManager.events.SearchOrCreate<SingleEvent<Character>>("Character");
        interactEvent = eventsManager.events.SearchOrCreate<DoubleEvent<(IGetPercentage, float), (bool, bool, Sprite)>>("Interact");

        energyEvent = eventsManager.events.SearchOrCreate<TripleEvent<(float, float, float), float, float>>("EnergyUpdate");

        for (int i = 0; i < equipedEvents.Length; i++)
        {
            var aux = eventsManager.events.SearchOrCreate<SingleEvent<(Ability, ItemBase)>>("abilityUI" + i);
            equipedEvents[i] = (param) => aux.delegato?.Invoke(param);
        }

        meleeWeaponUIMediator = (index, item) => equipedEvents[0].Invoke((item?.defaultKata, item?.itemBase));

        abilityExtUIMediator[0] = (index, item) => equipedEvents[1].Invoke((item, item?.itemBase));
        abilityExtUIMediator[1] = (index, item) => equipedEvents[2].Invoke((item, item?.itemBase));

        kataUIMediator[0] = (index, item) => equipedEvents[3].Invoke((item, item?.itemBase));
        kataUIMediator[1] = (index, item) => equipedEvents[4].Invoke((item, item?.itemBase));
        kataUIMediator[2] = (index, item) => equipedEvents[5].Invoke((item, item?.itemBase));
        kataUIMediator[3] = (index, item) => equipedEvents[6].Invoke((item, item?.itemBase));

        abilityExtUIMediator[2] = (index, item) => equipedEvents[7].Invoke((item, item?.itemBase));
        abilityExtUIMediator[3] = (index, item) => equipedEvents[8].Invoke((item, item?.itemBase));
        abilityExtUIMediator[4] = (index, item) => equipedEvents[9].Invoke((item, item?.itemBase));
        abilityExtUIMediator[5] = (index, item) => equipedEvents[10].Invoke((item, item?.itemBase));
    }

    private void Start()
    {
        LoadSystem.AddPostLoadCorutine(() =>
        {
            characterEvent.delegato += OnCharacterSelected;
            //VirtualControllers.Principal.eventDown += NoCharacterSelected;
            NoCharacterSelected(Vector2.zero , 0);
        },100);
    }
}