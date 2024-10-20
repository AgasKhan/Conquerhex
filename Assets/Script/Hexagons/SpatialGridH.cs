using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class SpatialGridH : ParentSpatialGrid<SpatialNodeH>
{
    public bool showIsTransitable = false;

    [SerializeField] 
    private LayerMask _layerMaskBake;
    
    [ContextMenu("Bake")]
    public void Bake()
    {
        for (int x = 0; x < buckets.GetLength(0); x++)
        {
            for (int y = 0; y <  buckets.GetLength(1); y++)
            {
                Vector3 center = GetPositionInWorld((x, y));
                
                center.y += -10;

                buckets[x, y].isTransitable = !Physics.BoxCast(center,
                    new Vector3(cellHeight/3, cellHeight/3, cellHeight/3), 
                    Vector3.up, 
                    Quaternion.identity, 
                    20,
                    _layerMaskBake
                    ,QueryTriggerInteraction.Ignore);
            }
        }
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        
        if(!showIsTransitable || buckets==null)
            return;
        
        Gizmos.color = Color.red;
        
        for (int x = 0; x < buckets.GetLength(0); x++)
        {
            for (int y = 0; y <  buckets.GetLength(1); y++)
            {
                buckets[x, y].index = new int2(x, y);

                buckets[x, y].parent = this;
                
                if (!buckets[x, y].isTransitable)
                {
                    Vector3 center = GetPositionInWorld((x, y));
                    
                    Vector3 originButtom = center + new Vector3( - cellWidth / 2, xz ? 0:  - cellHeight / 2, xz ? - cellHeight / 2 : 0);
                    Vector3 finalButtom = center + new Vector3( + cellWidth / 2, xz ? 0 :  + cellHeight / 2, xz ? + cellHeight / 2 : 0);
                    
                    Vector3 originTop = center + new Vector3( - cellWidth / 2, xz ? 0 : + cellHeight / 2, xz ?  + cellHeight / 2 : 0 );
                    Vector3 finalTop = center + new Vector3(+ cellWidth / 2, xz ? 0:  - cellHeight / 2, xz ?  - cellHeight / 2 : 0 );

                    Gizmos.DrawLine(originButtom,finalButtom);
                    Gizmos.DrawLine(originTop,finalTop);
                    
                    //Gizmos.DrawWireSphere(center,cellWidth/2);
                }
            }
        }
        
        for (int x = 0; x < buckets.GetLength(0); x++)
        {
            for (int y = 0; y <  buckets.GetLength(1); y++)
            {
                if (x <  buckets.GetLength(0) -1 && buckets[x + 1, y].isTransitable)
                    buckets[x, y].neightbors.c1 = new int2(x+1, y);
                else
                    buckets[x, y].neightbors.c1 = new int2(-1, -1);
                
                if (y <  buckets.GetLength(1) -1 && buckets[x, y + 1].isTransitable)
                    buckets[x, y].neightbors.c1 = new int2(x, y + 1);
                else
                    buckets[x, y].neightbors.c1 = new int2(-1, -1);
                
                if (x > 0 && buckets[x - 1, y].isTransitable)
                    buckets[x, y].neightbors.c1 = new int2(x-1, y);
                else
                    buckets[x, y].neightbors.c1 = new int2(-1, -1);
                
                if (y > 0 && buckets[x, y - 1].isTransitable)
                    buckets[x, y].neightbors.c1 = new int2(x, y-1);
                else
                    buckets[x, y].neightbors.c1 = new int2(-1, -1);
            }
        }
    }
}

public struct SpatialNodeH : ISpatialNode<SpatialNodeH>
{
    public HashSet<IGridEntity> content { get; private set; }

    public bool isTransitable;

    public int2 index;

    public SpatialGridH parent;

    public int2x4 neightbors;
    
    public IEnumerator<IGridEntity> GetEnumerator()
    {
        return content.GetEnumerator();
    }
    
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
    
    public SpatialNodeH Create()
    {
        return new SpatialNodeH() { content = new HashSet<IGridEntity>() };
    }
}