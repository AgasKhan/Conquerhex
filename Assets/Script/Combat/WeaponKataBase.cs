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
    public
    GameObject[] particles;

    [Header("Deteccion")]

    [SerializeField]
    protected Detect<Entity> detect;

    [Header("Multiplicadores danio")]
    public Damage[] damagesMultiply = new Damage[0];

    protected override void SetCreateItemType()
    {
        _itemType = typeof(WeaponKata);
    }

    public override Pictionarys<string, string> GetDetails()
    {
        var aux = base.GetDetails();

        if(damagesMultiply.Length>0)
            aux.Add("Modificadores", damagesMultiply.ToString("= x", "\n"));

        if (damages.Length > 0)
            aux.Add("Requisitos", damages.ToString("<","\n"));

        return aux;
    }

    protected void Attack(Entity caster, Vector2 direction, Weapon weapon)
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

    protected void Damage(ref Damage[] damages, params IDamageable[] damageables)
    {
        foreach (var entitys in damageables)
        {
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

    public abstract void ControllerDown(Entity caster, Vector2 dir, float button, Weapon weapon, Timer cooldownEnd, Vector2Int[] particles);
    public abstract void ControllerPressed(Entity caster, Vector2 dir, float button, Weapon weapon, Timer cooldownEnd, Vector2Int[] particles);
    public abstract void ControllerUp(Entity caster, Vector2 dir, float button, Weapon weapon, Timer cooldownEnd, Vector2Int[] particles);
    protected abstract void InternalAttack(Entity caster, Vector2 direction, Damage[] damages);
}

[System.Serializable]
public class WeaponKata : Item<WeaponKataBase>,Init, IControllerDir, IGetPercentage
{
    public event System.Action<Weapon> equipedWeapon;
    public event System.Action<Weapon> desEquipedWeapon;
    public event System.Action<Weapon> rejectedWeapon;


    public event System.Action updateTimer;

    void TriggerTimerEvent()
    {
        updateTimer?.Invoke();
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
    Timer cooldown;
    Entity caster;
    Vector2Int[] indexParticles;

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
        up = MyControllerUp;

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
            itemBase.ControllerDown(caster, dir, tim, weapon, cooldown, indexParticles);
            pressed = MyControllerPressed;
            up = MyControllerUp;
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

    public float Percentage()
    {
        return cooldown.Percentage();
    }

    public float InversePercentage()
    {
        return cooldown.InversePercentage();
    }

    #endregion

    #region internal functions
    void MyControllerPressed(Vector2 dir, float tim)
    {
        itemBase.ControllerPressed(caster, dir, tim, weapon, cooldown, indexParticles);
    }

    public void MyControllerUp(Vector2 dir, float tim)
    {
        
        itemBase.ControllerUp(caster, dir, tim, weapon, cooldown, indexParticles);
    }

    void MyControllerVOID(Vector2 dir, float tim)
    {
    }

    #endregion
}

[System.Serializable]
public class Detect<T>
{
    public float radius;

    public float distance;

    public LayerMask layerMask;

    public int maxDetects = -1;

    public int minDetects = 1;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="chck">Funcion que chequea si deseo hacerle danio (usualmente compara si no es el casteador)</param>
    /// <returns></returns>
    public T[] Area(Vector2 pos, System.Func<T, bool> chck)
    {
        var aux = Physics2D.OverlapCircleAll(pos, radius, layerMask);

        List<T> damageables = new List<T>();

        foreach (var item in aux)
        {
            var components = item.GetComponents<T>();

            foreach (var comp in components)
            {
                if(chck(comp))
                    damageables.Add(comp);
            }
        }

        return damageables.ToArray();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="caster"></param>
    /// <param name="chck">compara si deseo hacerle daño</param>
    /// <param name="chckTr">compara si es el casteador</param>
    /// <returns></returns>
    public T[] AreaWithRay(Vector2 pos, Vector2 caster, System.Func<T, bool> chck, System.Func< Transform , bool> chckTr)
    {
        List<T> damageables = new List<T>(Area(pos, chck));

        for (int i = damageables.Count; i >= 0; i--)
        {
            var aux = RayTransform(pos, (caster - pos), (caster - pos).magnitude);

            if ( aux != null && aux.Length > 0 && !chckTr(aux[minDetects]))
            {
                damageables.RemoveAt(i);
            }
        }

        return damageables.ToArray();
    }

    /// <summary>
    /// Ray que devuelve el transform
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="dir"></param>
    /// <param name="distance"></param>
    /// <returns></returns>
    public Transform[] RayTransform(Vector2 pos, Vector2 dir, float distance = -1)
    {
        var aux = Physics2D.RaycastAll(pos, dir, distance<0 ? this.distance : distance, layerMask);

        Debug.DrawRay(pos, dir, Color.red, 1);

        if (aux.Length >= minDetects)
        {
            List<Transform> tr = new List<Transform>();

            Transform[] result=new Transform[maxDetects > 0 ? maxDetects : tr.ToArray().Length - minDetects - 1];

            foreach (var item in aux)
            {
                tr.Add(item.transform);
            }

            System.Array.ConstrainedCopy(tr.ToArray(), minDetects-1, result, 0, result.Length);

            return result;
        }

        return null;
    }

    /// <summary>
    /// Ray que devuelve el tipo de la clase
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="dir"></param>
    /// <param name="distance"></param>
    /// <returns></returns>
    public T[] Ray(Vector2 pos, Vector2 dir, float distance = -1)
    {
        return Ray(pos, dir, (entity) => true, distance);
    }

    public T[] Ray(Vector2 pos, Vector2 dir, System.Func<T, bool> chck,  float distance = -1)
    {
        List<T> result = new List<T>();

        var tr = RayTransform(pos, dir, distance);

        foreach (var item in tr)
        {
            var aux = item.GetComponents<T>();
          
            foreach (var tType in aux)
            {
                if(chck(tType))
                    result.Add(tType);
            }
        }

        if (result.Count > 0)
            return result.ToArray();
        else
            return null;
    }


}
