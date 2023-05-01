using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IAEmergencia : IAFather
{
    MoveAbstract enemy;
    Persuit pursuit;



    Timer timer;

    [SerializeField]
    float distanceAttack;

    AutomaticAttack automatick;

    // Update is called once per frame
    void Update()
    {
        if (enemy == null || character==null)
            return;

        if((enemy.transform.position - transform.position).sqrMagnitude < distanceAttack * distanceAttack && timer.Chck && automatick.timerToAttack.Chck)
        {
            automatick.Attack();
        }
        else
        {
            character.move.ControllerPressed((enemy.transform.position - transform.position).normalized, 0);
            //character.move.ControllerPressed((enemy.transform.position - transform.position).normalized, 0);
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out Entity enemy))
        {
            if (enemy.team != character.team)
                this.enemy = enemy.GetComponent<MoveAbstract>();
        }
    }

    public override void OnEnterState(Character param)
    {
        character = param;

        timerStun.Set(character.bodyBase.stunTime);

        timer = TimersManager.Create(2);

        automatick = new AutomaticAttack(character.ter);
    }

    public override void OnExitState(Character param)
    {

    }

    public override void OnStayState(Character param)
    {
  
    }
}

