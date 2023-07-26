using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IAAnimator : IAFather
{
    public IGetEntity enemy;

    public AutomaticAttack automatick;

    public CircleCollider2D coll;

    Animator anim;

    public override void OnEnterState(Character param)
    {
        character = param;

        timerStun.Set(((BodyBase)character.flyweight).stunTime);

        automatick = new AutomaticAttack(character.ActualKata(2));
    }

    public override void OnExitState(Character param)
    {
    }

    public override void OnStayState(Character param)
    {
    }

    private void OnEnable()
    {
        if(character!=null)
            character.enabled = true;
    }

    private void OnDisable()
    {
        if(anim != null && anim.gameObject.activeSelf)
            anim.SetTrigger("Damaged");
        if (character != null)
            character.enabled = false;
    }

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out IGetEntity enemy))
        {
            if (enemy.GetEntity().team != character.team && enemy.GetEntity().team != Team.recursos)
                this.enemy = enemy;
        }
    }

 
    private void Update()
    {
        if (enemy == null)
            return;
        
        if (Mathf.Pow(coll.radius,2) < ((enemy as Component).transform.position - transform.position).sqrMagnitude)
            enemy = null;
        
    }


}