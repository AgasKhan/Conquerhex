using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IALastChance : IAFather
{
    MoveAbstract enemy;

    Timer doryEnemy;

    Timer timer;

    AutomaticAttack prin;

    AutomaticAttack sec;

    [SerializeField]
    float distanceAttack;

    // Update is called once per frame
    void Update()
    {
        if (enemy == null || character==null)
            return;

        if((enemy.transform.position - transform.position).sqrMagnitude < distanceAttack * distanceAttack && prin.timerToAttack.Chck && sec.timerToAttack.Chck)
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
        int rng = Random.Range(1, 3);

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

        timerStun.Set(((BodyBase)character.flyweight).stunTime);

        doryEnemy = TimersManager.Create(10, () => enemy = null);

        prin = new AutomaticAttack(character.prin);

        sec = new AutomaticAttack(character.sec);

        timer = TimersManager.Create(1);

        prin.onAttack += () => timer.Set(prin.timerToAttack.total);

        sec.onAttack += () => timer.Set(sec.timerToAttack.total);
    }

    public override void OnExitState(Character param)
    {

    }

    public override void OnStayState(Character param)
    {
  
    }
}
