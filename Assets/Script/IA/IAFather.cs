using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IAFather : MonoBehaviour, IState<Character>, IDamageable
{
    [SerializeField]
    Detect<RecolectableItem> areaFarming;

    //public Timer timerStun = null;

    protected Character character;

    protected BodyBase flyWeight => ((BodyBase)character.flyweight);


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
        character = param;
        areaFarming.radius = flyWeight.areaFarming;
    }

    public abstract void OnExitState(Character param);

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
}
