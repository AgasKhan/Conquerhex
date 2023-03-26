using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    #region VARIABLES

    [Header("Estadisticas")]
    [SerializeField] protected Damage damage;
    [SerializeField] protected float durability;
    [SerializeField] protected float velocity;

    [Header("audio")]
    public AudioClip audioClip;
    public AudioSource audioSource;

    [Header("Gizmos")]
    public Color color;
    [Range(0.1f, 2)]
    public float radius;
    [Range(-2, 2)]
    public float distance;
    [Range(0.1f, 2)]
    public float upCollider;
    #endregion

    #region FUNCIONES

    #region VIRTUALES
    public virtual void Attack()
    {
        Collider[] Auxiliar = DetectColliders();

       /* if (Auxiliar != null)
            foreach (var item in Auxiliar)
            {
                /* IReciveDamage entitys = item.GetComponent<IReciveDamage>();
                 if (entitys != null && item.name != name)
                     AttackPers(entitys);*/
            //}
    }

    public virtual void Durability()
    {
        durability -= 1;
        if (durability <= 0)
        {
            Destroy();
        }
    }

    public virtual void Destroy()
    {
        enabled = false;
    }

    public virtual void Drop()
    {

    }

    public virtual Collider[] DetectColliders()
    {
        return Physics.OverlapSphere(transform.position + Vector3.up * upCollider + transform.forward * distance, radius);
    }

    #endregion

    #region ABSTRACTAS
    public abstract void ButtomA();
    public abstract void ButtomB();
    public abstract void ButtomC();

    #endregion

    #endregion

    #region UNITY FUNCIONES

    private void OnDrawGizmos()
    {
        Gizmos.color = color;
        Gizmos.DrawSphere(transform.position + Vector3.up * upCollider + transform.forward * distance, radius);
    }

    #endregion
}

public abstract class Damage : MonoBehaviour
{
    float amount;
}

