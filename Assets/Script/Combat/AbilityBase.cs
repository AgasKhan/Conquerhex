using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public abstract class AbilityBase : FatherWeaponAbility<AbilityBase>
{
    [Space]

    [Header("tipo de boton")]
    public bool joystick;

    [Space]

    [Header("Particulas a mostrar")]
    [SerializeField]
    GameObject particles;

    [Header("Deteccion")]
    [SerializeField]
    protected Detect<IDamageable> detect;

    [Header("Multiplicadores danio")]
    public Damage[] damagesMultiply = new Damage[0];

    protected override void SetCreateItemType()
    {
        _itemType = typeof(Ability);
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
        //instacio particulas y bla bla


        Damage[] damagesCopy = (Damage[])weapon.itemBase.damages.Clone();

        for (int i = 0; i < damagesMultiply.Length; i++)
        {
            for (int ii = 0; ii < damagesCopy.Length; ii++)
            {
                if(damagesMultiply[i].type == damagesCopy[ii].type)
                {
                    damagesCopy[ii].amount *= damagesMultiply[ii].amount;
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

    public abstract void ControllerDown(Entity caster, Vector2 dir, float button, Weapon weapon, Timer cooldownEnd);
    public abstract void ControllerPressed(Entity caster, Vector2 dir, float button, Weapon weapon, Timer cooldownEnd);
    public abstract void ControllerUp(Entity caster, Vector2 dir, float button, Weapon weapon, Timer cooldownEnd);
    protected abstract void InternalAttack(Entity caster, Vector2 direction, Damage[] damages);
}

[System.Serializable]
public class Ability : Item<AbilityBase>,Init, IControllerDir, IGetPercentage
{
    public event System.Action<Weapon> equipedWeapon;
    public event System.Action<Weapon> desEquipedWeapon;
    public event System.Action<Weapon> rejectedWeapon;
    

    public Weapon weapon
    {
        get => _weapon;
        set => ChangeWeapon(value);
    }

    [SerializeField]
    Weapon _weapon;
    Timer cooldown;
    Entity caster;

    void ChangeWeapon(Weapon weapon)
    {
        foreach (var ability in itemBase.damages)
        {
            foreach (var dmg in weapon.damages)
            {
                if(ability.type==dmg.type && ability.amount>dmg.amount)
                {
                    rejectedWeapon(weapon);
                    return;
                }
            }
        }

        desEquipedWeapon?.Invoke(this._weapon);//puede devolver o no null en base a si ya tenia un arma previa o no

        this._weapon = weapon;

        equipedWeapon?.Invoke(this._weapon);//Jamas recibira un arma null al menos que le este pasando un null como parametro
    }

    public override void Init(params object[] param)
    {
        if(param.Length>0)
            this.caster = param[0] as Entity;

        cooldown = TimersManager.Create(itemBase.velocity);
        _weapon.Init();
    }

    public void ControllerDown(Vector2 dir, float tim)
    {
        itemBase.ControllerDown(caster, dir, tim, weapon, cooldown);
    }

    public void ControllerPressed(Vector2 dir, float tim)
    {
        itemBase.ControllerPressed(caster, dir, tim, weapon, cooldown);
    }

    public void ControllerUp(Vector2 dir, float tim)
    {
        itemBase.ControllerUp(caster, dir, tim, weapon, cooldown);
    }

    public float Percentage()
    {
        return cooldown.Percentage();
    }

    public float InversePercentage()
    {
        return cooldown.InversePercentage();
    }
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
    public T[] Area(Vector2 pos, System.Func<Transform, bool> chck)
    {
        var aux = Physics2D.OverlapCircleAll(pos, radius, layerMask);

        List<T> damageables = new List<T>();

        foreach (var item in aux)
        {
            var components = item.GetComponents<T>();

            if (components.Length > 0 && chck(item.transform))
            {
                damageables.AddRange(components);
            }
        }

        return damageables.ToArray();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="caster"></param>
    /// <param name="chck">compara si no es el casteador</param>
    /// <returns></returns>
    public T[] AreaWithRay(Vector2 pos, Vector2 caster, System.Func<Transform, bool> chck)
    {
        List<T> damageables = new List<T>(Area(pos, chck));

        for (int i = damageables.Count; i >= 0; i--)
        {
            var aux = RayTransform(pos, (caster - pos), (caster - pos).magnitude);

            if (!chck(aux[0]))
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
        List<T> result = new List<T>();

        var tr = RayTransform(pos, dir, distance);

        foreach (var item in tr)
        {
            var aux = item.GetComponents<T>();
            if (aux.Length > 0)
                result.AddRange(aux);
        }

        if (result.Count > 0)
            return result.ToArray();
        else
            return null;
    }


}


//padre de los pdoeres y debuffos
/*
/// <summary>
/// Clase abastracta padre de los poderes y debuffos
/// </summary>
[System.Serializable]
abstract public class FatherPwDbff : IState<Character>
{
    /// <summary>
    /// Funcion que es llamada cada frame del poder/buffo
    /// </summary>
    /// <param name="a">Parametro que recibe de forma automatica que te da al afectado</param>
    public Action<Character> on_Update;

    int _indexSpawnPool = -1;

    /// <summary>
    /// Funcion que es llamada cuando se gana el poder/buffo
    /// </summary>
    /// <param name="a">Parametro que recibe de forma automatica que te da al afectado</param>
    public abstract void OnEnterState(Character me);

    /// <summary>
    /// Funcion que es llamada cuando se pierde el poder/buffo
    /// </summary>
    /// <param name="a">Parametro que recibe de forma automatica que te da al afectado</param>
    public abstract void OnExitState(Character me);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="param"></param>
    public void OnStayState(Character me)
    {
        on_Update(me);
    }

    public FatherPwDbff()
    {
        NameClassToIndexCategory();
    }

    int NameClassToIndexCategory()
    {
        if (_indexSpawnPool > 0)
        {
            return _indexSpawnPool;
        }

        string nameClass = GetType().FullName;

        nameClass = nameClass.Substring(0, nameClass.IndexOf('_'));

        _indexSpawnPool = PoolObjects.SrchInCategory(nameClass);

        return _indexSpawnPool;
    }

    protected Vector2Int SchPowerObject(string name)
    {
        return PoolObjects.SrchInCategory(NameClassToIndexCategory(), name);
    }

    protected GameObject SpawnPowerObject(string name, Vector3 pos, Quaternion angle, Transform parent = null)
    {
        return PoolObjects.SpawnPoolObject(PoolObjects.SrchInCategory(NameClassToIndexCategory(), name), pos, angle, parent);
    }

    protected GameObject SpawnPowerObject(Vector2Int axis, Vector3 pos, Vector3 angle)
    {
        return PoolObjects.SpawnPoolObject(axis, pos, Quaternion.Euler(angle));
    }

    protected GameObject SpawnPowerObject(Vector2Int axis, Vector3 pos)
    {
        return PoolObjects.SpawnPoolObject(axis, pos, Quaternion.identity);
    }

    protected GameObject SpawnPowerObject(Vector2Int axis, Vector3 pos, Quaternion angle, Transform parent = null)
    {
        return PoolObjects.SpawnPoolObject(axis, pos, angle, parent);
    }
}*/

//padre de los debuffos
/*
 abstract public class Debuff_FatherPwDbff : FatherPwDbff
{
    /// <summary>
    /// Referencia para del indice del objeto que contenga las particulas (prefab)
    /// </summary>
    public GameObject particles;

    /// <summary>
    /// Referencia para del indice del objeto que contenga las particulas (prefab)
    /// </summary>
    public string particlesString;

    /// <summary>
    /// referencia del timer interno del debuff
    /// </summary>
    public Timer timer;

    //public Debuff_FatherPwDbff(float dbffTimer, Character me, GameObject particlesName=null)

    /// <summary>
    /// debe ser llamada luego de crear la clase
    /// </summary>
    /// <param name="me"></param>
    public virtual void Instance(Character me)
    {
        timer = TimersManager.Create(6);

        OnEnterState(me);

        if(particlesString!=null && particlesString!="")
        {
            particles = SpawnPowerObject(particlesString, Vector3.zero, Quaternion.identity, me.transform);
            var noMainaux = particles.GetComponentInChildren<ParticleSystem>();

            noMainaux.Stop();

            var aux = noMainaux.main;

            aux.duration = (6 - aux.startLifetime.constant / aux.simulationSpeed) * aux.simulationSpeed;

            noMainaux.Play();

        }

    }


}
*/

//debuffo toxina
/*
 public class Toxine_Debuff : Debuff_FatherPwDbff
{
    float _toxineDameg;

    Timer toDamage;
    public override void OnEnterState(Character me)
    {
        on_Update = MyUpdate;
        _toxineDameg = 2;
        toDamage = TimersManager.Create(0.5f);

        particlesString = "ToxineParticle";
    }

    public override void OnExitState(Character me)
    {
        TimersManager.Destroy(toDamage);
    }

    void MyUpdate(Character me)
    {
        if (toDamage.Chck)
        {
            me.health.Substract(_toxineDameg);
            toDamage.Reset();
        }
    }


}
*/