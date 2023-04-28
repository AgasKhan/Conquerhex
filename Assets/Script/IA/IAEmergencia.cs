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

    AutomatickAttack automatick;

    // Update is called once per frame
    void Update()
    {
        if (enemy == null || character==null)
            return;

        if((enemy.transform.position - transform.position).sqrMagnitude < distanceAttack * distanceAttack && timer.Chck && automatick.attack.Chck)
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

        timer = TimersManager.Create(2);

        automatick = new AutomatickAttack(character.ter);

    }

    public override void OnExitState(Character param)
    {

    }

    public override void OnStayState(Character param)
    {
  
    }
}

public class AutomatickAttack
{
    public Timer attack;
    WeaponKata kata;

    public event System.Action onAttack;

    //recibe el porcentaje de la carga del ataque
    public event System.Action<float> waitToAttack;

    public AutomatickAttack(WeaponKata kata)
    {
        this.kata = kata;

        attack = null;

        attack = TimersManager.Create(Random.Range(3, 6) / 3f,()=> {

            waitToAttack?.Invoke(attack.InversePercentage());

        }, () =>
        {
            kata.ControllerUp(Vector2.zero, 0);

            onAttack?.Invoke();

            waitToAttack = null;
        }
        
        , false);
    }

    public void Attack()
    {
        attack.Reset();
        kata.ControllerDown(Vector2.zero, 0);
    }
}