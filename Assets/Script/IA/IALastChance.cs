using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IALastChance : IAFather
{
    MoveAbstract enemy;

    Timer doryEnemy;

    Timer timer;

    AutomatickAttack prin;

    AutomatickAttack sec;

    [SerializeField]
    float distanceAttack;

    // Update is called once per frame
    void Update()
    {
        if (enemy == null || character==null)
            return;

        if((enemy.transform.position - transform.position).sqrMagnitude < distanceAttack * distanceAttack && prin.attack.Chck && sec.attack.Chck)
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
        int rng = Random.Range(2, 5);

        Debug.Log("El numero magico es: " + rng);

        switch (rng)
        {
            case 1:
                prin.Attack();
                break;

            case 2:
                sec.Attack();
                break;
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
        doryEnemy.Reset();
    }

    public override void OnEnterState(Character param)
    {
        character = param;

        timerStun.Set(character.bodyBase.stunTime);

        doryEnemy = TimersManager.Create(10, () => enemy = null);

        prin = new AutomatickAttack(character.prin);

        timer = TimersManager.Create(1);

        prin.onAttack += () => timer.Set(prin.attack.total);

        sec = new AutomatickAttack(character.sec);

        sec.onAttack += () => timer.Set(sec.attack.total);
    }

    public override void OnExitState(Character param)
    {

    }

    public override void OnStayState(Character param)
    {
  
    }
}
