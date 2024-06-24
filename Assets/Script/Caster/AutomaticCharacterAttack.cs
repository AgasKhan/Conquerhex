using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AutomaticCharacterAttack
{
    public TimedAction timerToAttack;
    public Timer timerChargeAttack;

    public Vector3 Aiming
    {
        get => _aiming.Vect2To3XZ(0);
        set
        {
            if(value!=Vector3.zero)
                _aiming =  Vector2.Lerp(_aiming, value.Vect3To2XZ(), Time.deltaTime* precisionTime);
        }
    }

    Vector2 _aiming;

    [SerializeField]
    int timeToAttack = 2;

    [SerializeField]
    int timeToChargeAttack = 1;

    [SerializeField, Tooltip("more is best")]
    float precisionTime = 5;


    Character owner;

    Ability _ability;

    SlotItem slotItem;

    EventControllerMediator eventControllerMediator;

    System.Action Cast;

    int index;

    Ability ability => slotItem.equiped == _ability ? _ability : null;

    public event System.Action onAttack
    {
        add
        {
            _ability.onCast += value;
        }
        remove
        {
            _ability.onCast -= value;
        }
    }

    public bool cooldown => ability != null ? ability.onCooldownTime && timerChargeAttack.Chck : false;

    FadeColorAttack FeedBackReference
    {
        get
        {
            if (ability != null && ability.FeedBackReference != null)
            {
                _ability.FeedbackDetect();

                return _ability.FeedBackReference;
            }

            return null;
        }
    }

    public float radius
    {
        get
        {
            if (ability != null)
                return ability.FinalMaxRange;
            else
                return 0;
        }
    }

    Color attackColor
    {
        get
        {
            if (FeedBackReference != null)
            {
                return FeedBackReference.attackColor;
            } 
            else
                return Color.white;
        }
    }

    Color areaColor
    {
        get
        {
            if (FeedBackReference != null)
            {
                return FeedBackReference.areaColor;
            } 
            else
                return Color.white;
        }
    }

    Color actual
    {
        get
        {
            if (FeedBackReference != null)
            {
                return FeedBackReference.color;
            }
            else
                return Color.white;
        }
        set
        {
            if (FeedBackReference != null)
                FeedBackReference.color = value.ChangeAlphaCopy(FeedBackReference.color.a);
        }
    }

    public void Attack()
    {
        if (ability == null || timerChargeAttack == null || !timerChargeAttack.Chck)
            return;

        Cast();

        timerChargeAttack.Reset();
    }

    public void ResetAttack()
    {
        if (!timerChargeAttack.Chck || !timerToAttack.Chck)
            return;
        
        //timerChargeAttack.Stop();

        timerToAttack.Reset();
    }

    public void StopTimers()
    {
        ability.StopCast();
        eventControllerMediator.ControllerUp(_aiming, timerChargeAttack.total);
        timerChargeAttack.Stop().SetInitCurrent(0);
        timerToAttack.Set(1).Stop().SetInitCurrent(0);
    }

    /// <summary>
    /// En base al slot recibido setea el resto de las variables
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="slotItem"></param>
    void Set<T>(SlotItem<T> slotItem) where T : ItemEquipable
    {
        this.slotItem = slotItem;

        if (typeof(WeaponKata).IsAssignableFrom(typeof(T)))
        {
            _ability = slotItem.equiped as Ability;
            index = slotItem.indexSlot;

            eventControllerMediator = owner.attackEventMediator;

            Cast = () => owner.Attack(index + 1);
        }
        else if (typeof(AbilityExtCast).IsAssignableFrom(typeof(T)))
        {
            _ability = slotItem.equiped as Ability;
            index = slotItem.indexSlot;

            if (index == 1)
            {
                Cast = owner.AlternateAbility;
                eventControllerMediator = owner.dashEventMediator;
            }
            else
            {
                Cast = () => owner.Ability(index);
                eventControllerMediator = owner.abilityEventMediator;
            }
        }
        else
        {
            _ability = (slotItem.equiped as MeleeWeapon)?.defaultKata;

            eventControllerMediator = owner.attackEventMediator;

            index = 0;

            Cast = () => owner.Attack(0);
        }
    }

    public void Init<T>(Character entity, SlotItem<T> slotItem) where T : ItemEquipable
    {
        owner = entity;

        Set(slotItem);

        timerToAttack = (TimedAction)TimersManager.Create(timeToAttack, () =>
        {
            if(timerToAttack.current<0.5f)
            {
                float percentage = (1 - timerToAttack.current / 0.5f);

                FeedBackReference?.Area(radius* percentage).Angle(Mathf.Lerp(360, ability.Angle, percentage)).Direction(Aiming);
            }

        }, Attack).Stop().SetInitCurrent(0);

        //timerChargeAttack = null;

        timerChargeAttack = TimersManager.Create(timeToChargeAttack, () =>
        {

            eventControllerMediator.ControllerPressed(_aiming, timerChargeAttack.total - timerChargeAttack.current);

        }, () =>
        {
            eventControllerMediator.ControllerUp(_aiming, timerChargeAttack.total);

            //weaponKata.FeedBackReference=null;

        }).Stop().SetInitCurrent(0);
    }


}

/*
[System.Serializable]
public class AutomaticAttack
{
    public TimedAction timerToAttack;
    public Timer timerChargeAttack;

    CasterEntityComponent owner;

    int indexKata;

    SlotItem<WeaponKata> kata => owner.katasCombo[indexKata];

    public event System.Action<Ability> onAttack
    {
        add
        {
            weaponKata.onPreCast += value;
        }
        remove
        {
            weaponKata.onPreCast -= value;
        }
    }

    public bool cooldown => kata.equiped!=null ? kata.equiped.onCooldownTime && timerChargeAttack.Chck : false;

    public WeaponKata weaponKata => kata.equiped;

    FadeColorAttack FeedBackReference
    {
        get
        {
            if (weaponKata != null && weaponKata.FeedBackReference != null)
            {
                var aux = kata.equiped.FeedBackReference;
                aux.Area(radius);
                return aux;
            }

            return null;
        }
    }

    public float radius
    {
        get
        {
            if (weaponKata != null)
                return weaponKata.FinalMaxRange;
            else
                return 0;
        }
    }

    Color attackColor
    {
        get
        {
            if (FeedBackReference != null)
            {
                return FeedBackReference.attackColor;
            } 
            else
                return Color.white;
        }
    }

    Color areaColor
    {
        get
        {
            if (FeedBackReference != null)
            {
                return FeedBackReference.areaColor;
            } 
            else
                return Color.white;
        }
    }

    Color actual
    {
        get
        {
            if (FeedBackReference != null)
            {
                return FeedBackReference.color;
            }
            else
                return Color.white;
        }
        set
        {
            if (FeedBackReference != null)
                FeedBackReference.color = value.ChangeAlphaCopy(FeedBackReference.color.a);
        }
    }

    public void Attack()
    {
        if (kata==null || weaponKata == null || timerChargeAttack == null || !timerChargeAttack.Chck)
            return;

        weaponKata.ControllerDown(Vector2.zero, 0);

        timerChargeAttack.Reset();
    }

    public void ResetAttack()
    {
        if (!timerChargeAttack.Chck || !timerToAttack.Chck)
            return;
        
        timerChargeAttack.Stop();

        timerToAttack.Reset();
    }

    public void StopTimers()
    {
        weaponKata?.ControllerUp(Vector2.zero, timerChargeAttack.total);
        weaponKata?.StopCast();
        timerChargeAttack.Stop().SetInitCurrent(0);
        timerToAttack.Set(1).Stop().SetInitCurrent(0);
    }

    public AutomaticAttack(CasterEntityComponent entity, int index)
    {
        owner = entity;
        indexKata = index;

        timerToAttack = (TimedAction)TimersManager.Create(1, ()=>
        {
            actual = Color.Lerp(areaColor, attackColor, timerToAttack.InversePercentage());

        },Attack).Stop().SetInitCurrent(0);

        timerChargeAttack = null;

        timerChargeAttack = TimersManager.Create(2, () => 
        {

            weaponKata?.ControllerPressed(Vector2.zero, timerChargeAttack.total - timerChargeAttack.current);

        }, () =>
        {
            weaponKata?.ControllerUp(Vector2.zero, timerChargeAttack.total);   

            weaponKata.FeedBackReference=null;

        }).Stop().SetInitCurrent(0);
        
    }
}

 
 */