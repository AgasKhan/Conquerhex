using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ViewEquipElement<TChild> : MonoBehaviour where TChild : ViewEquipElement<TChild>
{
    [SerializeField]
    GameObject weapon;

    [SerializeField]
    Vector3 offset;

    [SerializeField]
    UnityEvent onSpawn;

    [SerializeField]
    UnityEvent onDespawn;

    public TChild Create(Vector3 position, Quaternion rotation, Transform parent) 
    {
        var aux = (TChild)Instantiate(this);

        aux.transform.SetParent(parent);
        aux.transform.position = position;
        aux.transform.rotation = rotation;
        aux.transform.position -= aux.transform.TransformDirection(offset);
        aux.weapon.SetActive(false);
        aux.name = name;
        return aux;
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }

    public bool Spawn()
    {
        if (!weapon.activeSelf)
        {
            onSpawn?.Invoke();
            weapon.SetActive(true);
            return true;
        }

        return false;
    }

    public bool Despawn()
    {
        if (weapon.activeSelf)
        {
            onDespawn?.Invoke();
            weapon.SetActive(false);
            return true;
        }

        return false;
    }

    public override bool Equals(object other)
    {
        if (other is ViewEquipWeapon weapon)
            return weapon.name.Equals(name);
        else
            return base.Equals(other);
    }

    public override int GetHashCode()
    {
        return name.GetHashCode();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(transform.position + transform.TransformDirection(offset), 0.1f);
    }
}
