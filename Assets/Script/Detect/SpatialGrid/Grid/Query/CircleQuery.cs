using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleQuery : MonoBehaviour, IQuery
{
    [SerializeField] SpatialGrid targetGrid = null;
    [SerializeField] float radious = 5;


    public IEnumerable<IGridEntity> Query()
    {
        Vector3 aabbFrom = transform.position + targetGrid.aabbFrom * radious;
        Vector3 aabbTo = transform.position + targetGrid.aabbTo * radious;

        return targetGrid.Query(aabbFrom, aabbTo, x => (x - transform.position).sqrMagnitude <= radious  * radious);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(Vector3.Scale(transform.position, targetGrid.aabbTo) , radious);
    }
}
