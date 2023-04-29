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

        timerStun.Set(character.bodyBase.stunTime);

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

    Color attackColor
    {
        get
        {
            if (kata.reference != null)
                return kata.reference.attackColor;
            else
                return Color.white;
        }
    }

    Color areaColor
    {
        get
        {
            if (kata.reference != null)
                return kata.reference.areaColor;
            else
                return Color.white;
        }
    }

    Color actual
    {
        get
        {
            if (kata.reference != null)
                return kata.reference.color;
            else
                return Color.white;
        }
        set
        {
            if (kata.reference != null) 
                kata.reference.color = value;
        }
    }

    public void Attack()
    {
        kata.ControllerDown(Vector2.zero, 0);

        attack.Reset();
    }

    public AutomatickAttack(WeaponKata kata)
    {
        this.kata = kata;

        attack = null;

        attack = TimersManager.Create(Random.Range(3, 6) / 3f,()=> {

            actual = Color.Lerp(areaColor, attackColor, attack.InversePercentage());

        }, () =>
        {
            kata.ControllerUp(Vector2.zero, 0);

            onAttack?.Invoke();
        });
    }
}