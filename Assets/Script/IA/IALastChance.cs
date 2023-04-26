using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IALastChance : IAFather
{
    MoveAbstract enemy;

    Character character;

    Timer timer;

    Timer doryEnemy;

    [SerializeField]
    float distanceAttack;

    // Update is called once per frame
    void Update()
    {
        if (enemy == null || character==null)
            return;

        if((enemy.transform.position - transform.position).sqrMagnitude < distanceAttack * distanceAttack && timer.Chck)
        {
            Attack();
        }
        else
        {
            character.move.ControllerPressed((enemy.transform.position - transform.position).normalized * (timer.Chck ? 1 : -1),0);
        }
    }

    void Attack()
    {
        int rng = Random.Range(1, 4);

        switch (rng)
        {
            case 1:
                character.prin.ControllerDown(Vector2.zero, 0);
                character.prin.ControllerUp(Vector2.zero, 0);

                break;

            case 2:
                character.sec.ControllerDown(Vector2.zero, 0);
                character.sec.ControllerUp(Vector2.zero, 0);

                break;

            case 3:
                character.ter.ControllerDown(Vector2.zero, 0);
                character.ter.ControllerUp(Vector2.zero, 0);
                break;
        }

        timer.Set(rng);
        
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
        doryEnemy.Reset();
    }

    public override void OnEnterState(Character param)
    {
        character = param;

        timer = TimersManager.Create(3);

        doryEnemy = TimersManager.Create(10, () => enemy = null, false);
    }

    public override void OnExitState(Character param)
    {

    }

    public override void OnStayState(Character param)
    {
  
    }
}
