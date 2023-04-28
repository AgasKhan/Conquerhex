using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public abstract class WeaponKataBase : FatherWeaponAbility<WeaponKataBase>
{
    [Space]

    [Header("tipo de boton")]
    public bool joystick;

    [Space]

    [Header("Particulas a mostrar")]
    public GameObject[] particles;

    [Header("Deteccion")]

    [SerializeField]
    public Detect<Entity> detect;

    [Header("Multiplicadores danio")]
    public Damage[] damagesMultiply = new Damage[0];

    public override Pictionarys<string, string> GetDetails()
    {
        var aux = base.GetDetails();

        if(damagesMultiply.Length>0)
            aux.Add("Modificadores", damagesMultiply.ToString("= x", "\n"));

        if (damages.Length > 0)
            aux.Add("Requisitos", damages.ToString("<","\n"));

        return aux;
    }

    public void Attack(Entity caster, Vector2 direction, Weapon weapon)
    {       

        Damage[] damagesCopy = (Damage[])weapon.itemBase.damages.Clone();

        if (caster is Character)
        {
            var casterCharacter = (Character)caster;

            List<Damage> additives = new List<Damage>(casterCharacter.additiveDamage);

            for (int i = 0; i < damagesCopy.Length; i++)
            {
                for (int ii = additives.Count - 1; ii >= 0; ii--)
                {
                    if(damagesCopy[i].typeInstance == additives[ii].typeInstance)
                    {
                        damagesCopy[i].amount += additives[ii].amount;

                        additives.RemoveAt(ii);

                        continue;
                    }
                }
            }

            additives.AddRange(damagesCopy);

            damagesCopy = additives.ToArray();

        }


        for (int i = 0; i < damagesMultiply.Length; i++)
        {
            for (int ii = 0; ii < damagesCopy.Length; ii++)
            {
                if(damagesMultiply[i].typeInstance == damagesCopy[ii].typeInstance)
                {
                    damagesCopy[ii].amount *= damagesMultiply[i].amount;
                    break;
                }
            }
        }

        InternalAttack(caster, direction, damagesCopy);
    }

    protected void Damage(ref Damage[] damages, Entity caster, params IDamageable[] damageables)
    {
        foreach (var entitys in damageables)
        {
            Color originalColor;

            if (entitys is DinamicEntity)
            {
                var aux = (DinamicEntity)entitys;

               Timer tim = null;

                var sprite = aux.GetComponentInChildren<SpriteRenderer>();

                originalColor = sprite.color;

            tim = TimersManager.Create(0.5f, ()=> {

            if(((int)(tim.Percentage() * 10)) % 2 == 0)
            {
                //parpadeo rapido

                
                sprite.color =  Color.magenta;
            }
            else
            {
               //el mantenido

               sprite.color = Color.yellow;
            }

        },()=> {

            //volver al original

            sprite.color = originalColor;
        });

                aux.move.Acelerator(aux.move.vectorVelocity * -2);
            }

            foreach (var dmg in damages)
            {
                entitys.TakeDamage(dmg);
            }
        }
    }

    /// <summary>
    /// En caso de ser verdadero le estoy diciendo a la IA que puede ATACAR
    /// </summary>
    /// <param name="caster"></param>
    /// <returns></returns>
    public virtual List<IDamageable> IADetect(Entity caster, Vector2 dir)
    {
        return new List<IDamageable>(detect.AreaWithRay(caster.transform.position, caster.transform.position, 
            (entidad)=> 
            { 
                return caster.team != entidad.team; 
            },
            (tr)=>
            {
                return caster.transform == tr;
            }
            ));
    }

    /*
    public abstract void ControllerDown(Entity caster, Vector2 dir, float button, Weapon weapon, Timer cooldownEnd, Vector2Int[] particles);
    public abstract void ControllerPressed(Entity caster, Vector2 dir, float button, Weapon weapon, Timer cooldownEnd, Vector2Int[] particles);
    public abstract void ControllerUp(Entity caster, Vector2 dir, float button, Weapon weapon, Timer cooldownEnd, Vector2Int[] particles);
    */
    protected abstract void InternalAttack(Entity caster, Vector2 direction, Damage[] damages);
}

[System.Serializable]
public abstract class WeaponKata : Item<WeaponKataBase>,Init, IControllerDir
{
    public event System.Action<Weapon> equipedWeapon;
    public event System.Action<Weapon> desEquipedWeapon;
    public event System.Action<Weapon> rejectedWeapon;


    public event System.Action<float> updateTimer;

    void TriggerTimerEvent()
    {
        updateTimer?.Invoke(cooldown.InversePercentage());
    }


    System.Action<Vector2, float> pressed;

    System.Action<Vector2, float> up;

    public Weapon weapon
    {
        get => _weapon;
        set => ChangeWeapon(value);
    }

    [SerializeField]
    Weapon _weapon;
    protected Timer cooldown;
    protected Entity caster;
    protected Vector2Int[] indexParticles;

    void ChangeWeapon(Weapon weapon)
    {
        foreach (var ability in itemBase.damages)
        {
            foreach (var dmg in weapon.damages)
            {
                if(ability.typeInstance == dmg.typeInstance && ability.amount>dmg.amount)
                {
                    rejectedWeapon(weapon);
                    Debug.Log("arma no aceptada");
                    return;
                }
            }
        }

        desEquipedWeapon?.Invoke(this._weapon);//puede devolver o no null en base a si ya tenia un arma previa o no

        this._weapon = weapon;

        equipedWeapon?.Invoke(this._weapon);//Jamas recibira un arma null al menos que le este pasando un null como parametro
    }

    #region interfaces

    /// <summary>
    /// primer parametro caster, segundo weapon
    /// </summary>
    /// <param name="param"></param>
    public override void Init(params object[] param)
    {
        pressed = MyControllerVOID;
        up = MyControllerVOID;

        if (itemBase == null)
            return;

        if (param.Length > 0)
            this.caster = param[0] as Entity;

        if (param.Length > 1)
            _weapon = param[1] as Weapon;

        cooldown = TimersManager.Create(itemBase.velocity, TriggerTimerEvent, ()=> { }, false);

        if(weapon!=null)
            weapon.Init();

        indexParticles = new Vector2Int[itemBase.particles.Length];

        for (int i = 0; i < itemBase.particles.Length; i++)
        {
            indexParticles[i] = PoolManager.SrchInCategory("Particles", itemBase.particles[i].name);
        }
    }

    public void ControllerDown(Vector2 dir, float tim)
    {
        if(cooldown.Chck)
        {
            InternalControllerDown(dir, tim);
            pressed = InternalControllerPress;
            up = InternalControllerUp;
        }
    }

    public void ControllerPressed(Vector2 dir, float tim)
    {
        pressed(dir, tim);
    }

    public void ControllerUp(Vector2 dir, float tim)
    {
        up(dir, tim);

        pressed = MyControllerVOID;
        up = MyControllerVOID;
    }

    #endregion

    #region internal functions

    void MyControllerVOID(Vector2 dir, float tim)
    {
    }


    protected abstract void InternalControllerDown(Vector2 dir, float tim);

    protected abstract void InternalControllerPress(Vector2 dir, float tim);

    protected abstract void InternalControllerUp(Vector2 dir, float tim);

    #endregion
}

