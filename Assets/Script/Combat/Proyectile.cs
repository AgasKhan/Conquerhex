using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(MoveRb))]
abstract public class Proyectile : MonoBehaviour
{
    //public Character owner;

    //public MoveRb MoveRb;

    public Danio damage;

    /*
    protected void AplicateDebuff(Character ch)
    {
        if (damage.debuffList == null)
            return;

        foreach (var debuff in damage.debuffList)
        {
            ch.AddDebuff(debuff);
        }

        damage.debuffList=null;
    }
   
    */

    protected void CasterObject()
    {
        if (damage.objectSpawner != null)
        {
            foreach (var item in damage.objectSpawner)
            {
                PoolObjects.SpawnPoolObject(item, transform.position, Quaternion.identity);
            }
        }
        damage.objectSpawner = null;
    }

    protected void AplicateAction(Collider other)
    {
        damage.actions?.Invoke(other);
        damage.actions = null;
    }

    protected virtual void OnEnter(Collider other)
    {
        var aux = other.GetComponents<IOnProyectileEnter>();

        AplicateAction(other);

        if (aux!=null && aux.Length>0)
        {
            foreach (var item in aux)
            {
                OnDamage(item);
            }
        }
        else
            FailDamage();
    }
    protected virtual void OnExit(GameObject go)
    {
        var aux = go.GetComponents<IOnProyectileExit>();

        if(aux!=null)
            foreach (var item in aux)
                item.ProyectileExit();
            
    }

    /// <summary>
    /// Funcion por defecto de danio
    /// </summary>
    /// <param name="damaged"></param>
    protected virtual void OnDamage(IOnProyectileEnter damaged)
    {
        damaged.ProyectileEnter(damage);
    }


    /// <summary>
    /// Funcion complementaria de danio que no sera llamada por defecto
    /// </summary>
    /// <typeparam name="T">Tipo de conversion que se buscara realizar</typeparam>
    /// <param name="damaged">Interfaz que representa la funcion que se desea ejecutar cuando colisiona con un proyectil</param>
    /// <returns>devolvera verdadero en caso de que se realice con exito la conversion</returns>
    protected virtual bool OnDamaged<T>(IOnProyectileEnter damaged, out T OUT)
    {
        if (damaged is T)
        {
            T aux = ((T)damaged);

            OUT = aux;

            return true;
        }

        OUT = default;

        return false;
    }

    protected virtual void FailDamage()
    {

    }

    public virtual void Throw(Danio dmg, Vector3 dir, float multiply)
    {
        damage = dmg;
        //MoveRb.Dash(dir, multiply);
    }

    private void OnTriggerEnter(Collider other)
    {
        /*
        if (!other.CompareTag(tag) && !other.CompareTag(owner.tag))
        {
            OnEnter(other);
        }
        */
    }

    private void OnTriggerExit(Collider other)
    {
        /*
        if (!other.CompareTag(tag) && !other.CompareTag(owner.tag))
        {
            OnExit(other.gameObject);
        }
        */
    }
}

public interface IOnProyectileEnter
{
    void ProyectileEnter(Danio damage);
}

public interface IOnProyectileExit
{
    void ProyectileExit();
}

public struct Danio
{
    public float amount;
    public System.Type[] debuffList;
    public System.Action<Collider> actions;
    public Vector2Int[] objectSpawner;
    public Vector3 velocity;

    /*
    public void SetWithCharacter(Character chr)
    {
        amount = chr.damage;
        debuffList = new System.Type[0];


        if (chr.debuffToAplicate != null)
        {
            debuffList = new System.Type[chr.debuffToAplicate.Count];
            chr.debuffToAplicate.CopyTo(debuffList);
        }

        if (chr.ActionOnDamage != null)
        {
            actions = chr.ActionOnDamage;
        }

        if (chr.ObjectSpawnOnDamage != null)
        {
            objectSpawner = new Vector2Int[chr.ObjectSpawnOnDamage.Count];
            chr.ObjectSpawnOnDamage.CopyTo(objectSpawner);
        }

        chr.ObjectSpawnOnDamage.Clear();
        chr.ActionOnDamage = null;
    }
    */

}