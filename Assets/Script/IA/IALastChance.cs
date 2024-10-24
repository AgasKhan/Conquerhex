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
        if (enemy == null || _character==null)
            return;

        if((enemy.transform.position - transform.position).sqrMagnitude < distanceAttack * distanceAttack && prin.timerChargeAttack.Chck && sec.timerChargeAttack.Chck)
        {
            Attack();
        }
        else
        {
            _character.move.ControllerPressed((enemy.transform.position - transform.position).normalized * (timer.Chck ? 1 : -1),0);
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
            if (enemy.team != _character.team)
                this.enemy = enemy.GetComponent<MoveAbstract>();

            
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        doryEnemy.Reset();
    }

    public override void OnEnterState(Character param)
    {
        _character = param;

        //timerStun.Set(((BodyBase)character.flyweight).stunTime);

        doryEnemy = TimersManager.Create(10, () => enemy = null);

        prin = new AutomaticAttack(_character, 0);

        sec = new AutomaticAttack(_character, 1);

        timer = TimersManager.Create(1);

        prin.onAttack += () => timer.Set(prin.timerChargeAttack.total);

        sec.onAttack += () => timer.Set(sec.timerChargeAttack.total);
    }

    public override void OnExitState(Character param)
    {

    }

    public override void OnStayState(Character param)
    {
  
    }
}
