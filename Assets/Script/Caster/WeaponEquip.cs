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

    public WeaponEquip Create(Vector3 localPosition, Transform parent)
    {
        var aux = Instantiate(this);
        aux.transform.SetParent(parent, false);
        aux.transform.localPosition = localPosition + offset;
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(transform.position+offset,0.1f);
    }
}
