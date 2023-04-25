using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : DinamicEntityWork, ISwitchState<Character>
{
    [SerializeField]
    BodyBase bodyBase;

    // Start is called before the first frame update
    [SerializeField]
    public WeaponKata prin;
    [SerializeField]
    public WeaponKata sec;
    [SerializeField]
    public WeaponKata ter;

    public Damage[] additiveDamage => bodyBase.additiveDamage;

    public IState<Character> CurrentState
    {
        get => _ia;
        set
        {
            _ia.OnExitState(this);
            _ia = value;
            _ia.OnEnterState(this);
        }
    }

    [SerializeField]
    IState<Character> _ia;

    public void SetWeaponKataCombo(ref WeaponKata set,WeaponKataCombo combo)
    {
        if (combo == null || combo.kata == null)
        {
            set.Init(this);
            return;
        }

        set = (WeaponKata)combo.kata.Create();

        var weapon = combo.weapon.Create();

        set.Init(this, weapon);
    }

    public override void TakeDamage(Damage dmg)
    {
        var vulDmg = bodyBase.vulnerabilities;

        for (int i = 0; i < vulDmg.Length; i++)
        {
            if (dmg.typeInstance == vulDmg[i].typeInstance)
            {
                dmg.amount *= vulDmg[i].amount;
            }
        }

        base.TakeDamage(dmg);
    }


    protected override void Config()
    {
        base.Config();
        MyAwakes += MyAwake;
    }

    void MyAwake()
    {
        health.Init(bodyBase.life, bodyBase.regen);

        SetWeaponKataCombo(ref prin, bodyBase.principal);

        SetWeaponKataCombo(ref sec, bodyBase.secondary);

        SetWeaponKataCombo(ref ter, bodyBase.tertiary);

        _ia = GetComponent<IState<Character>>();

        _ia.OnEnterState(this);

        

        //ver move con su velocidad
    }

}

    
