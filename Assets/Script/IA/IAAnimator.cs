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

        character.onTakeDamage += Character_onTakeDamage;
    }

    public override void OnExitState(Character param)
    {
        character.onTakeDamage -= Character_onTakeDamage;
    }

    public override void OnStayState(Character param)
    {
    }

    private void Character_onTakeDamage(Damage obj)
    {
        anim.SetTrigger("Damaged");
    }

    void Detect(Collider2D collision)
    {
        if (collision.TryGetComponent(out IGetEntity enemy))
        {
            if (enemy.GetEntity() != null && enemy.GetEntity().team != character.team && enemy.GetEntity().team != Team.recursos)
            {
                this.enemy = enemy;
                enabled = true;
            }
                
        }
    }


    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Detect(collision);
    }


    private void Update()
    {
        if (enemy == null)
        {
            enabled = false;
            return;
        }
        
        
        if (Mathf.Pow(coll.radius,2) < (enemy.transform.position - transform.position).sqrMagnitude)
            enemy = null;
    }


}