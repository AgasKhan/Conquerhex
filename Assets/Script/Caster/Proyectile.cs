using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MoveEntityComponent))]
public class Proyectile : Entity
{
    public System.Action<Collision2D> actions;

    public Vector2Int[] objectSpawner;

    public MoveAbstract move => moveComponent.move;

    Damage[] damages;

    Timer off;

    [SerializeField]
    Transform collision;

    [SerializeField]
    Detect<Entity> detect;

    MoveEntityComponent moveComponent;  


    //protected override Damage[] vulnerabilities => null;


    protected override void Config()
    {
        base.Config();
        MyAwakes += MyAwake;
        MyUpdates += Proyectile_MyUpdates;
    }
    
    void MyAwake()
    {
        off = TimersManager.Create(10, () => gameObject.SetActive(false)).Stop();

        //moveComponent = GetInContainer<MoveEntityComponent>();

        if(TryGetInContainer(out moveComponent))
            move.onMove += Move_onMove;
    }

    private void Move_onMove(Vector2 obj)
    {
        transform.up = obj;
    }

    private void Proyectile_MyUpdates()
    {
        
        var affected = detect.Area(collision.position, (entity) => entity.team != team);
        if(affected.Count>0)
        {
            affected[0].TakeDamage(damages);
            damages = null;
            gameObject.SetActive(false);
            off.Reset();
            off.Stop();
        }
    }

    public virtual void Throw(Entity owner ,Damage[] dmg, Vector3 dir)
    {
        gameObject.SetActive(true);
        //team = owner.team;
        damages = dmg;
        move.Velocity(dir.normalized * move.objectiveVelocity);
        off.Start();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawSphere(collision.position, detect.radius);
    }
}