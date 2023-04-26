using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IAEmergencia : IAFather
{
    MoveAbstract enemy;
    Persuit pursuit;

    Character character;

    Timer timer;

    [SerializeField]
    float distanceAttack;

    // Update is called once per frame
    void Update()
    {
        if (enemy == null || character==null)
            return;

        if((enemy.transform.position - transform.position).sqrMagnitude < distanceAttack * distanceAttack && timer.Chck)
        {
            character.ter.ControllerDown(Vector2.zero, 0);
            character.ter.ControllerUp(Vector2.zero, 0);
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

    private void OnTriggerStay2D(Collider2D collision)
    {
    }

    public override void OnEnterState(Character param)
    {
        character = param;

        timer = TimersManager.Create(2);

    }

    public override void OnExitState(Character param)
    {

    }

    public override void OnStayState(Character param)
    {
  
    }
}
