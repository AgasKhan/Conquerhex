using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : DinamicEntityWork, ISwitchState<Character>
{
    [field: SerializeField]
    public BodyBase bodyBase
    {
        get;
        private set;
    }

    // Start is called before the first frame update
    [SerializeReference]
    public WeaponKata prin;
    [SerializeReference]
    public WeaponKata sec;
    [SerializeReference]
    public WeaponKata ter;

    [SerializeField]
    Detect<RecolectableItem> areaFarming;

    public Damage[] additiveDamage => bodyBase.additiveDamage;

    public IState<Character> CurrentState
    {
        get => _ia;
        set
        {
            if(_ia!=null)
            {
                _ia.OnExitState(this);
            }
            else if(value!=null)
            {
                MyUpdates += IAUpdate;
            }

            _ia = value;
            _ia.OnEnterState(this);
        }
    }

    [SerializeField]
    IState<Character> _ia;

    public void SetWeaponKataCombo(ref WeaponKata set,WeaponKataCombo combo)
    {
        if (combo.kata == null)
            return;

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

        //---------------------
        if(team == Team.enemy)
        {
            var aux = gameObject.GetComponent<Animator>();

            if( aux!= null)
                aux.SetTrigger("Dañado");
        }
        //---------------------

        base.TakeDamage(dmg);
    }


    protected override void Config()
    {
        base.Config();
        MyAwakes += MyAwake;
        MyUpdates += MyUpdate;
    }

    void MyAwake()
    {
        health.Init(bodyBase.life, bodyBase.regen);

        areaFarming.radius = bodyBase.areaFarming;

        SetWeaponKataCombo(ref prin, bodyBase.principal);

        SetWeaponKataCombo(ref sec, bodyBase.secondary);

        SetWeaponKataCombo(ref ter, bodyBase.tertiary);

        _ia = GetComponent<IState<Character>>();
        if (_ia != null)
        {
            _ia.OnEnterState(this);
            MyUpdates += IAUpdate;
        }

        //--------------------------
        health.noLife += ShowLoserWindow;
        //--------------------------

        

        //ver move con su velocidad
    }

    void MyUpdate()
    {
        var recolectables = areaFarming.Area(transform.position, (algo) => { return true; });
        Debug.Log("depredadores " + recolectables.Count);
        foreach (var recolectable in recolectables)
        {
            recolectable.Recolect(this);
        }
    }

    void IAUpdate()
    {
        _ia.OnStayState(this);
    }

    //--------------------------------------------
    void ShowLoserWindow()
    {
        if (team == Team.player)
        {
            GameManager.instance.Pause(false);
            MenuManager.instance.ShowWindow("Defeat");
        }
        else
        {
            gameObject.SetActive(false);
            Drop();
        }
    }
    //--------------------------------------------

}


