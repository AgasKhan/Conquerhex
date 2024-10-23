using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class IAIO : IAFather
{
    [SerializeField]
    EventManager eventsManager;

    [SerializeField]
    Detect<InteractEntityComponent> detectInteractuable = new Detect<InteractEntityComponent>();

    [SerializeField]
    string lastCombo;

    [SerializeField]
    UnityEvent effectBackToBaseStart;

    [SerializeField]
    UnityEvent effectBackToBaseEnd;

    [Header("Player sets"), SerializeField]
    string playerTag = "Player";
    string originalTag;

    [SerializeField]
    LayerMask playerLayerMask;
    LayerMask originalLayerMask;

    [SerializeField]
    Controllers.TriggerAxis cameraJoystickMouse;
    [SerializeField]
    Controllers.TriggerAxis cameraPositionMouse;

    string[] comboRapido = new string[] { "↑↑", "→→", "←←", "↓↓" };

    char[] womboCOMBO = new char[] { '↑', '→', '←', '↓' };

    Timer comboReset;

    InteractEntityComponent lastInteractuable;

    DoubleEvent<(IGetPercentage, float), (bool, bool, Sprite)> interactEvent;

    TripleEvent<(float, float, float), float, float> energyEvent;

    SingleEvent<Character> characterEvent;

    SingleEvent<Health> healthEvent;

    System.Action<(Ability, ItemBase)>[] equipedEvents = new System.Action<(Ability, ItemBase)>[12];

    System.Action<int, MeleeWeapon> meleeWeaponUIMediator;
    System.Action<int, WeaponKata>[] kataUIMediator = new System.Action<int, WeaponKata>[4];
    System.Action<int, AbilityExtCast>[] abilityExtUIMediator = new System.Action<int, AbilityExtCast>[6];

    Vector2 automaticMoveToBase;

    EventControllerMediator inventoryEventMediator;


    public override void OnStayState(Character param)
    {
        base.OnStayState(param);

        #region vuelta a base hardcodeada

        if (Input.GetKeyDown(KeyCode.Y))
        {
            if (automaticMoveToBase == Vector2.zero)
            {
                int ladoToGo = character.HexagoneParent.ladoToBase;

                if (ladoToGo == -1)
                {
                    if (character.HexagoneParent.id == 0)
                        UI.Interfaz.instance["Titulo secundario"].ShowMsg("No puedes volver a base desde desde la base");
                    else
                        UI.Interfaz.instance["Titulo secundario"].ShowMsg("No puedes volver a base desde aqui");
                }
                else
                {
                    effectBackToBaseStart.Invoke();
                    automaticMoveToBase = Vector2.one;
                    character.move.objectiveVelocity *= 5;
                    character.gameObject.layer = 14;
                }
            }
            else
            {
                effectBackToBaseEnd.Invoke();
                character.move.objectiveVelocity /= 5;
                automaticMoveToBase = Vector2.zero;
                character.gameObject.layer = 7;
            }
        }


        if (automaticMoveToBase != Vector2.zero)
        {
            int ladoToGo = character.HexagoneParent.ladoToBase;

            if (character.HexagoneParent.id == 0 || ladoToGo == -1)
            {
                effectBackToBaseEnd.Invoke();

                if (character.HexagoneParent.id == 0)
                    UI.Interfaz.instance["Titulo secundario"].ShowMsg("Llegaste a base");
                else
                    UI.Interfaz.instance["Titulo secundario"].ShowMsg("Se perdio el rumbo");

                character.move.objectiveVelocity /= 5;
                automaticMoveToBase = Vector2.zero;
                character.gameObject.layer = 7;
                return;
            }

            Vector2 dir = character.HexagoneParent.ladosPuntos[ladoToGo].Vect3To2XZ();

            dir -= character.transform.position.Vect3To2XZ();

            if (dir.sqrMagnitude > 0.25f)
            {
                automaticMoveToBase = dir.normalized;
            }

            moveEventMediator.ControllerPressed(automaticMoveToBase, 0);
            return;
        }

        #endregion

        #region cambio perspectiva hardcodeada
        if (Input.GetKeyDown(KeyCode.V))
        {
            if (character.aiming.mode == AimingEntityComponent.Mode.perspective)
                character.aiming.mode = AimingEntityComponent.Mode.topdown;
            else
                character.aiming.mode = AimingEntityComponent.Mode.perspective;
        }
        #endregion

        var buildings = detectInteractuable.Area(character.transform.position, (edificio) => { return edificio.interactuable; });

        if (buildings == null || buildings.Count == 0)
        {
            interactEvent.secondDelegato?.Invoke((false, false, null));

            VirtualControllers.Interact.eventDown -= Interact_eventDown;

            if (lastInteractuable != null)
            {
                UI.Interfaz.instance.interactButton.Play("InteractClose");
            }

            lastInteractuable = null;
        }
        else if (buildings[0] != lastInteractuable)
        {
            VirtualControllers.Interact.eventDown -= Interact_eventDown;

            lastInteractuable = buildings[0];

            interactEvent.secondDelegato?.Invoke((true, false, lastInteractuable.Image));

            VirtualControllers.Interact.eventDown += Interact_eventDown;

            if (lastInteractuable != null)
            {
                UI.Interfaz.instance.interactButton.Play("InteractOpen");
            }
        }

        if (lastInteractuable != null)
            UI.Interfaz.instance.interactButton.transform.position = Camera.main.WorldToScreenPoint(lastInteractuable.offsetInteractView);
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
        param.aiming.onMode -= Aiming_onMode;
        //param.onTakeDamage -= OnTakeDamage;

        DesuscribiUI();

        attackEventMediator.eventDown -= AttackEventMediator_eventDown;

        abilityEventMediator.eventDown -= AbilityEventMediator_eventDown;

        dashEventMediator.eventDown -= DashEventMediator_eventDown;

        inventoryEventMediator.eventDown -= InventoryEventMediator_eventDown;


        VirtualControllers.Movement.eventDown -= MoveEventMediator_eventDown;

        

        DesuscribiUI();
        VirtualControllers.Inventory.DesuscribeController(inventoryEventMediator);

        //VirtualControllers.Camera.DesuscribeController(aimingEventMediator);

        VirtualControllers.Movement.DesuscribeController(moveEventMediator);

        VirtualControllers.Principal.DesuscribeController(attackEventMediator);

        VirtualControllers.Secondary.DesuscribeController(abilityEventMediator);

        VirtualControllers.Terciary.DesuscribeController(dashEventMediator);

        VirtualControllers.Interact.eventDown -= Interact_eventDown;

        

        interactEvent.secondDelegato?.Invoke((false, false, null));

        lastInteractuable = null;

        param.gameObject.tag = originalTag;

        foreach (Transform child in param.GetComponentsInChildren<Transform>())
        {
            child.gameObject.layer = originalLayerMask;
        }

        base.OnExitState(character);
    }


    public override void OnEnterState(Character param)
    {
        base.OnEnterState(param);

        originalTag = param.gameObject.tag;
        param.gameObject.tag = playerTag;

        originalLayerMask = param.gameObject.layer;
        foreach (Transform child in param.GetComponentsInChildren<Transform>())
        {
            child.gameObject.layer = 15;
        }

        param.move.onTeleport += TeleportEvent;

        //param.health.lifeUpdate += UpdateLife;
        //param.health.regenUpdate += UpdateRegen;
        //param.health.regenTimeUpdate += UpdateRegenTime;

        //param.onTakeDamage += OnTakeDamage;

        param.health.helthUpdate += Health_helthUpdate;

        Health_helthUpdate(param.health);

        param.aiming.onMode += Aiming_onMode;

        param.caster.energyUpdate += EnergyUpdate;
        param.caster.leftEnergyUpdate += LeftEnergyUpdate;
        param.caster.rightEnergyUpdate += RightEnergyUpdate;

        Aiming_onMode(param.aiming.mode);

        SuscribeUI();

        TriggerUI();

        

        attackEventMediator.eventDown += AttackEventMediator_eventDown;

        abilityEventMediator.eventDown += AbilityEventMediator_eventDown;

        dashEventMediator.eventDown += DashEventMediator_eventDown;

        inventoryEventMediator = new EventControllerMediator();
        inventoryEventMediator.eventDown += InventoryEventMediator_eventDown;
        VirtualControllers.Inventory.unscaled = true;

        VirtualControllers.Movement.eventDown += MoveEventMediator_eventDown;

        VirtualControllers.Principal.SwitchGetDir(character.aiming);

        VirtualControllers.Secondary.SwitchGetDir(character.aiming);

        VirtualControllers.Terciary.SwitchGetDir(character.aiming);


        VirtualControllers.Inventory.SuscribeController(inventoryEventMediator);

        //VirtualControllers.Camera.SuscribeController(aimingEventMediator);

        VirtualControllers.Movement.SuscribeController(moveEventMediator);

        VirtualControllers.Principal.SuscribeController(attackEventMediator);

        VirtualControllers.Secondary.SuscribeController(abilityEventMediator);

        VirtualControllers.Terciary.SuscribeController(dashEventMediator);
    }

    protected override void Health_death()
    {
        var chr = character;

        character.CurrentState = null;

        TimersManager.Create(0.5f, () =>
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



    void SuscribeUI()
    {
        caster.weapons[0].toChange += meleeWeaponUIMediator;
        caster.abilities[0].toChange += abilityExtUIMediator[0];
        caster.abilities[1].toChange += abilityExtUIMediator[1];

        for (int i = 0; i < 4 && (i) < caster.abilities.Count; i++)
        {
            caster.katas[i].toChange += kataUIMediator[i];
        }

        for (int i = 0; i < 4 && (i + 2) < caster.abilities.Count; i++)
        {
            caster.abilities[i + 2].toChange += abilityExtUIMediator[i + 2];
        }
    }

    void DesuscribiUI()
    {
        caster.weapons[0].toChange -= meleeWeaponUIMediator;
        caster.abilities[0].toChange -= abilityExtUIMediator[0];
        caster.abilities[1].toChange -= abilityExtUIMediator[1];

        for (int i = 0; i < 4 && (i) < caster.abilities.Count; i++)
        {
            caster.katas[i].toChange -= kataUIMediator[i];
        }

        for (int i = 0; i < 4 && (i + 2) < caster.abilities.Count; i++)
        {
            caster.abilities[i + 2].toChange -= abilityExtUIMediator[i + 2];
        }
    }

    void TriggerUI()
    {
        equipedEvents[0].Invoke((caster.weapons[0].equiped?.defaultKata, caster.weapons[0].equiped?.itemBase));
        equipedEvents[1].Invoke((caster.abilities[0].equiped, caster.abilities[0].equiped?.itemBase));
        equipedEvents[2].Invoke((caster.abilities[1].equiped, caster.abilities[1].equiped?.itemBase));

        for (int i = 0; i < 4 && (i) < caster.abilities.Count; i++)
        {
            equipedEvents[i + 3].Invoke((caster.katas[i].equiped, caster.katas[i].equiped?.itemBase));
        }

        for (int i = 0; i < 4 && (i + 2) < caster.abilities.Count; i++)
        {
            equipedEvents[i + 7].Invoke((caster.abilities[i + 2].equiped, caster.abilities[i + 2].equiped?.itemBase));
        }
    }

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
        for (int i = 0; i < comboRapido.Length; i++)
        {
            if (comboRapido[i] == lastCombo)
            {
                character.Attack(i + 1, arg1);
                lastCombo = string.Empty;
                return;
            }
        }

        if (character.actualCombo == -2)
        {
            character.Attack(0, arg1);
        }
        else
        {
            int number = 0;

            if (!lastCombo.IsEmpty())
                for (int i = 0; i < womboCOMBO.Length; i++)
                {
                    if (lastCombo[^1] == womboCOMBO[i])
                    {
                        number = (i);
                        break;
                    }
                }

            /*
            tier 1: 0 a 4 NO SUMO
            tier 2: 5 a 9 debo de sumar 5
            tier 3: del 10 al 14 debo sumar 10

            reinicio el combo si es mayor que 10
            */

            if (character.actualCombo < 10)
            {
                if (character.actualCombo >= 5)
                    number += 10;
                else if (character.actualCombo >= 0)
                    number += 5;
            }

            character.ComboAttack(number);
        }
    }

    private void AbilityEventMediator_eventDown(Vector2 arg1, float arg2)
    {
        System.Action onEnter = () => abilityEventMediator.eventDown -= AbilityEventMediator_eventDown;
        System.Action onExit = () =>  abilityEventMediator.eventDown += AbilityEventMediator_eventDown;

        for (int i = 0; i < comboRapido.Length; i++)
        {
            if (comboRapido[i] == lastCombo)
            {
                character.Ability(i + 1, arg1, onEnter, onExit);
                lastCombo = string.Empty;
                comboReset.Stop();
                return;
            }
        }

        character.Ability(0, arg1, onEnter, onExit);
    }


    private void DashEventMediator_eventDown(Vector2 arg1, float arg2)
    {
        System.Action onEnter = () => dashEventMediator.eventDown -= DashEventMediator_eventDown;
        System.Action onExit = () => dashEventMediator.eventDown += DashEventMediator_eventDown;

        character.AlternateAbility(arg1, onEnter, onExit);
    }

    private void PerspectiveConfigAndCameraBlockOEnter(Vector2 arg1, float arg2)
    {
        cameraJoystickMouse.enable = false;
        cameraPositionMouse.enable = true;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        VirtualControllers.Camera.DesuscribeController(aimingEventMediator);
        //aimingEventMediator.DesuscribeController(character.aiming);
    }

    private void TopdownConfigAndCameraBlockOnExit(Vector2 arg1, float arg2)
    {
        cameraJoystickMouse.enable = true;
        cameraPositionMouse.enable = false;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        
        VirtualControllers.Camera.SuscribeController(aimingEventMediator);
        //aimingEventMediator.SuscribeController(character.aiming);
    }

    private void Aiming_onMode(AimingEntityComponent.Mode obj)
    {
        VirtualControllers.CameraBlock.eventDown -= PerspectiveConfigAndCameraBlockOEnter;
        VirtualControllers.CameraBlock.eventUp -= TopdownConfigAndCameraBlockOnExit;

        switch (obj)
        {
            case AimingEntityComponent.Mode.topdown:

                TopdownConfigAndCameraBlockOnExit(Vector2.zero, 0);

                VirtualControllers.CameraBlock.eventDown += PerspectiveConfigAndCameraBlockOEnter;
                VirtualControllers.CameraBlock.eventUp += TopdownConfigAndCameraBlockOnExit;

                break;


            case AimingEntityComponent.Mode.perspective:

                PerspectiveConfigAndCameraBlockOEnter(Vector2.zero, 0);

                break;


            case AimingEntityComponent.Mode.focus:

                break;
        }
    }

    private void Interact_eventDown(Vector2 arg1, float arg2)
    {
        character.aiming.AimingToObjective2D = arg1;
        lastInteractuable.Interact(character);
        UI.Interfaz.instance.interactButton.Play("InteractAccept");
    }

    private void InventoryEventMediator_eventDown(Vector2 arg1, float arg2)
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "MainMenu" && !UIE_MenusManager.instance.isInMenu)
            UIE_MenusManager.instance.EnableMenu(UIE_MenusManager.instance.EquipmentMenu);
        else
            UIE_MenusManager.instance.TriggerOnClose();
    }


    private void TeleportEvent(Hexagone obj, int lado)
    {
        HexagonsManager.SetRenders(obj, HexagonsManager.LadoOpuesto(lado), obj.AristaMasCercana(character.transform));


        if (HexagonsManager.idMaxLevel == obj.id)
        {
            UI.Interfaz.instance?["Titulo secundario"].ShowMsg("Fortaleza a la vista");
        }
        else
        {
            for (int i = 0; i < obj.ladosArray.Length; i++)
            {
                if (obj.ladosArray[i].id == HexagonsManager.idMaxLevel)
                {
                    UI.Interfaz.instance?["Titulo secundario"].ShowMsg("Fortaleza cercana en los alrededores");
                }
            }
        }
    }

    private void Health_helthUpdate(Health obj)
    {
        healthEvent.delegato?.Invoke(obj);
    }

    private void EnergyUpdate((float, float, float) obj)
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
        if (character != null)
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
        healthEvent = eventsManager.events.SearchOrCreate<SingleEvent<Health>>(LifeType.all);

        //Cursor Hardcodeado
        Cursor.lockState = CursorLockMode.Confined;

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
            NoCharacterSelected(Vector2.zero, 0);
        }, 100);
    }
}

/*
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
*/