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

    public Texture2D positionsPCache, normalsPCache;
    public int countPCache;

    /*
    #if UNITY_EDITOR

    [SerializeField]
    UnityEditor.Experimental.VFX.Utility.PointCacheAsset pointCache;

    private void OnValidate()
    {
        if(pointCache!=null)
        {
            positionsPCache = pointCache.surfaces[0];
            normalsPCache = pointCache.surfaces[1];
            countPCache = pointCache.PointCount;
        }
    }

    #endif
    */
    public WeaponEquip Create(Vector3 position, Quaternion rotation, Transform parent)
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

    public bool Spawn()
    {
        if(!weapon.activeSelf)
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

        Gizmos.DrawWireSphere(transform.position + transform.TransformDirection(offset), 0.1f);
    }
}
