using UnityEngine;
using UnityEngine.Events;

public class WeaponEquip : MonoBehaviour
{
    [SerializeField]
    GameObject weapon;

    [SerializeField]
    Vector3 offset;

    [SerializeField]
    UnityEvent onSpawn;

    [SerializeField]
    UnityEvent onDespawn;

    public WeaponEquip Create(Vector3 position, Quaternion rotation ,Transform parent)
    {
        var aux = Instantiate(this);
        
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

    public void Spawn()
    {
        onSpawn?.Invoke();
        weapon.SetActive(true);
    }

    public void Despawn()
    {
        onDespawn?.Invoke();
        weapon.SetActive(false);
    }

    public override bool Equals(object other)
    {
        if (other is WeaponEquip weapon)
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

        Gizmos.DrawWireSphere(transform.position+ transform.TransformDirection(offset), 0.1f);
    }
}
