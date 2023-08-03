using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IAFather : MonoBehaviour, IState<Character>, IDamageable
{
    [SerializeField]
    protected Detect<RecolectableItem> areaFarming;

    //public Timer timerStun = null;

    protected Character _character;

    protected BodyBase flyWeight => ((BodyBase)_character.flyweight);

    public Character character => _character;


    void Awake()
    {
        /*
        timerStun = TimersManager.Create(0.33f, () =>
        {
            enabled = true;
        }).Stop();
        */
    }

    public void TakeDamage(ref Damage dmg)
    {
        if (dmg.amount <= 0)
            return;
        //enabled = false;
        //timerStun.Reset();
    }

    public virtual void OnEnterState(Character param)
    {
        _character = param;
        areaFarming.radius = flyWeight.areaFarming;
        param.health.death += Health_death;
    }

    public virtual void OnExitState(Character param)
    {
        param.health.death -= Health_death;
    }

    public virtual void OnStayState(Character param)
    {
        var recolectables = areaFarming.Area(transform.position, (algo) => { return true; });

        foreach (var recolectable in recolectables)
        {
            //if (currentWeight + recolectable.weight <= weightCapacity)
            {
                recolectable.Recolect(param);
            }
        }
    }


    protected virtual void Health_death()
    {
        gameObject.SetActive(false);
    }
}
