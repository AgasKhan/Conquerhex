using System.Collections;
using UnityEngine;
using System.Collections.Generic;


public abstract class AbilityBase : FatherWeaponAbility<AbilityBase>
{
    [SerializeField]
    GameObject particles;

    protected void Attack(Vector2 direction, WeaponBase weapon)
    {
        //instacio particulas y bla bla
        InternalAttack(direction, weapon);
    }

    public abstract void ControllerDown(Vector2 dir, float button, WeaponBase weapon, bool cooldownEnd);
    public abstract void ControllerPressed(Vector2 dir, float button, WeaponBase weapon, bool cooldownEnd);
    public abstract void ControllerUp(Vector2 dir, float button, WeaponBase weapon, bool cooldownEnd);
    protected abstract void InternalAttack(Vector2 direction, WeaponBase weapon);
}

public class Ability : Init, IControllerDir
{
    public WeaponBase weapon;
    public AbilityBase abilityBase;
    Timer cooldown;

    public void Init()
    {
        cooldown = TimersManager.Create(abilityBase.velocity);
    }

    public void ControllerDown(Vector2 dir, float tim)
    {
        abilityBase.ControllerDown(dir, tim, weapon, cooldown.Chck);
    }

    public void ControllerPressed(Vector2 dir, float tim)
    {
        abilityBase.ControllerPressed(dir, tim, weapon, cooldown.Chck);
    }

    public void ControllerUp(Vector2 dir, float tim)
    {
        abilityBase.ControllerUp(dir, tim, weapon, cooldown.Chck);
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