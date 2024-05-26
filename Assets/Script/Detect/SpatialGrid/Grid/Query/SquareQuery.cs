using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SquareQuery : MonoBehaviour, IQuery 
{

    public SpatialGrid             targetGrid;
    public float                   width    = 15f;
    public float                   height   = 30f;
    public IEnumerable<IGridEntity> selected = new List<IGridEntity>();

    Vector3 WidhHeight => targetGrid.AaBb(width, height);

    public IEnumerable<IGridEntity> Query() 
    {
        //posicion inicial --> esquina superior izquierda de la "caja"
        //posición final --> esquina inferior derecha de la "caja"
        //como funcion para filtrar le damos una que siempre devuelve true, para que no filtre nada.
        return targetGrid.Query(
                                transform.position + WidhHeight * -0.5f,
                                transform.position + WidhHeight * 0.5f,
                                x => true);
    }

    void OnDrawGizmos() 
    {
        if (targetGrid == null) return;

        //Flatten the sphere we're going to draw
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(Vector3.Scale(transform.position, targetGrid.aabbTo), WidhHeight);
    }
}