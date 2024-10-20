using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpatialGrid : ParentSpatialGrid<SpatialNode>
{
}

public struct SpatialNode : ISpatialNode<SpatialNode>
{
    public HashSet<IGridEntity> content { get; set; }
    
    public IEnumerator<IGridEntity> GetEnumerator()
    {
        return content.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public SpatialNode Create()
    {
        return new SpatialNode() {content = new HashSet<IGridEntity>()};
    }
}

public interface ISpatialNode<out TChild> : IEnumerable<IGridEntity> where TChild : ISpatialNode<TChild>
{
    public HashSet<IGridEntity> content { get; }

    public bool Add(IGridEntity gridEntity) => content.Add(gridEntity);
    
    public bool Remove(IGridEntity gridEntity) => content.Remove(gridEntity);

    public bool Contains(IGridEntity gridEntity) => content.Contains(gridEntity);

    public TChild Create();
}

public abstract class ParentSpatialGrid<TNode> : MonoBehaviour where TNode : ISpatialNode<TNode>, new()
{
    #region Variables
    [Tooltip("Verdadero para el plano xz, falso para el plano xy")]
    public bool xz;

    /// <summary>
    /// ancho de las celdas
    /// </summary>
    [SerializeField]
    public float cellWidth;

    /// <summary>
    /// alto de las celdas
    /// </summary>
    [SerializeField]
    public float cellHeight;

    [SerializeField]
    //cantidad de columnas (el "ancho" de la grilla)
    public int width;

    [SerializeField]
    //cantidad de filas (el "alto" de la grilla)
    public int height;

    [SerializeField]
    Vector3 offset;

    /// <summary>
    /// punto de inicio de la grilla en X
    /// </summary>
    public float x => transform.position.x + offset.x;

    /// <summary>
    /// punto de inicio de la grilla en Y
    /// </summary>
    public float y => transform.position.y + offset.y;

    /// <summary>
    /// punto de inicio de la grilla en z
    /// </summary>
    public float z => transform.position.z + offset.z;

    public Vector3 aabbFrom => xz ? aabbFromZ : aabbFromY;

    public Vector3 aabbTo => xz ? aabbToZ : aabbToY;


    //ultimas posiciones conocidas de los elementos, guardadas para comparación.
    protected Dictionary<IGridEntity, (int x, int y)> lastPositions = new();

    //los "contenedores"
    protected TNode[,] buckets;

    //el valor de posicion que tienen los elementos cuando no estan en la zona de la grilla.
    /*
     Const es implicitamente statica
     const tengo que ponerle el valor apenas la declaro, readonly puedo hacerlo en el constructor.
     Const solo sirve para tipos de dato primitivos.
     */
    readonly public (int x, int y) Outside = (-1, -1);

    //Una colección vacía a devolver en las queries si no hay nada que devolver
    readonly public IGridEntity[] Empty = new IGridEntity[0];



    readonly Vector3 aabbFromY = new Vector3(-1, -1, 0);

    readonly Vector3 aabbFromZ = new Vector3(-1, 0, -1);

    readonly Vector3 aabbToY = new Vector3(1, 1, 0);

    readonly Vector3 aabbToZ = new Vector3(1, 0, 1);

    #endregion

    #region Funciones

    public Vector3 AaBb(float first, float second)
    {
        Vector3 aux = new Vector3();

        aux.x = first;

        if (xz)
            aux.z = second;
        else
            aux.y = second;

        return aux;
    }

    public void Add(IGridEntity entity)
    {
        entity.OnMove += UpdateEntity;
        UpdateEntity(entity);
    }

    public void Remove(IGridEntity entity)
    {
        entity.OnMove -= UpdateEntity;

        UpdateEntity(entity);
        var currentPos = GetPositionInGrid(entity.Position);
        buckets[currentPos.Item1, currentPos.Item2].Remove(entity);
        lastPositions.Remove(entity);
    }

    public void UpdateEntity(IGridEntity entity)
    {
        var lastPos = lastPositions.ContainsKey(entity) ? lastPositions[entity] : Outside;
        var currentPos = GetPositionInGrid(entity.Position);

        //Misma posición, no necesito hacer nada
        if (lastPos.Equals(currentPos))
            return;

        //Lo "sacamos" de la posición anterior
        if (IsInsideGrid(lastPos))
        {
            buckets[lastPos.x, lastPos.y].Remove(entity);
        }

        //Lo "metemos" a la celda nueva, o lo sacamos si salio de la grilla
        if (IsInsideGrid(currentPos))
        {
            buckets[currentPos.x, currentPos.y].Add(entity);
            lastPositions[entity] = currentPos;
        }
        else
            lastPositions.Remove(entity);
    }

    public IEnumerable<IGridEntity> Query(Vector3 aabbFrom, Vector3 aabbTo, Func<Vector3, bool> filterByPosition)
    {
        Vector3 from;
        Vector3 to;

        if (xz)
        {
            from = new Vector3(Mathf.Min(aabbFrom.x, aabbTo.x), 0, Mathf.Min(aabbFrom.z, aabbTo.z));
            to = new Vector3(Mathf.Max(aabbFrom.x, aabbTo.x), 0, Mathf.Max(aabbFrom.z, aabbTo.z));
        }
        else
        {
            from = new Vector3(Mathf.Min(aabbFrom.x, aabbTo.x), Mathf.Min(aabbFrom.y, aabbTo.y), 0);
            to = new Vector3(Mathf.Max(aabbFrom.x, aabbTo.x), Mathf.Max(aabbFrom.y, aabbTo.y), 0);
        }

   
        var fromCoord = GetPositionInGrid(from);
        var toCoord = GetPositionInGrid(to);

        fromCoord = (Util.Clamp(fromCoord.x, 0, width), Util.Clamp(fromCoord.y, 0, height));
        toCoord = (Util.Clamp(toCoord.x, 0, width), Util.Clamp(toCoord.y, 0, height));

        if (!IsInsideGrid(fromCoord) && !IsInsideGrid(toCoord))
            return Empty;

        // Creamos tuplas de cada celda
        var cols = Util.Generate(fromCoord.Item1, x => x + 1)
                       .TakeWhile(n => n < width && n <= toCoord.Item1);

        var rows = Util.Generate(fromCoord.Item2, y => y + 1)
                       .TakeWhile(y => y < height && y <= toCoord.Item2);

        var cells = cols.SelectMany(col => rows.Select(row => (col, row)));

        // Iteramos las que queden dentro del criterio

        if (xz)
        {
            return cells
              .SelectMany(cell => buckets[cell.col, cell.row])
              .Where(e =>
                         from.x <= e.Position.x && e.Position.x <= to.x &&
                         from.z <= e.Position.z && e.Position.z <= to.z
                    )
              .Where(n => filterByPosition(n.Position));
        }
        else
        {
            return cells
              .SelectMany(cell => buckets[cell.col, cell.row])
              .Where(e =>
                         from.x <= e.Position.x && e.Position.x <= to.x &&
                         from.y <= e.Position.y && e.Position.y <= to.y
                    )
              .Where(n => filterByPosition(n.Position));
        }

        
    }

    public (int x, int y) GetPositionInGrid(Vector3 pos)
    {
        //quita la diferencia, divide segun las celdas y floorea por que las flores se la bancan
        return (
            Mathf.FloorToInt((pos.x - x) / cellWidth),
            Mathf.FloorToInt(((xz? pos.z -z : pos.y - y) ) / cellHeight)
            );
    }
    
    public Vector3 GetPositionInWorld((int x, int y) gridPos)
    {
        float worldX = gridPos.x * cellWidth + x + cellWidth/2;
        float worldY = xz ? y : gridPos.y * cellHeight + y + cellHeight/2;
        float worldZ = xz ? gridPos.y * cellHeight + z  + cellHeight/2: z;

        return new Vector3(worldX, worldY, worldZ);
    }

    public bool IsInsideGrid((int x, int y) position)
    {
        //si es menor a 0 o mayor a width o height, no esta dentro de la grilla
        return 0 <= position.x && position.x < width &&
               0 <= position.y && position.y < height;
    }

    
    
    void OnDestroy()
    {
        var ents = RecursiveWalker(transform).Select(n => n.GetComponent<IGridEntity>())
                                             .Where(n => n != null);

        foreach (var e in ents) e.OnMove -= UpdateEntity;
    }

    protected void Awake()
    {
        buckets = new TNode[width, height];

        //creamos todos los hashsets
        for (var i = 0; i < width; i++)
        {
            for (var j = 0; j < height; j++)
            {
                buckets[i, j] = (new TNode()).Create();
            }
        }

        var ents = RecursiveWalker(transform)
                  .Select(n => n.GetComponent<IGridEntity>())
                  .Where(n => n != null);

        foreach (var e in ents)
        {
            e.OnMove += UpdateEntity;
            UpdateEntity(e);
        }
    }

    #region GENERATORS

    private static IEnumerable<Transform> RecursiveWalker(Transform parent)
    {
        foreach (Transform child in parent)
        {
            foreach (Transform grandchild in RecursiveWalker(child))
                yield return grandchild;
            yield return child;
        }
    }

    #endregion

    #endregion

    #region GRAPHIC REPRESENTATION

    [Header("Representacion grafica")]

    public bool showGrid = true;
    public bool areGizmosShutDown;
    public bool activatedGrid;
    public bool showLogs = true;


    protected virtual void OnDrawGizmos()
    {
        if (!showGrid)
            return;

        IEnumerable<(Vector3, Vector3)> rows;

        IEnumerable<(Vector3, Vector3)> cols;

        if(xz)
        {
            rows = Util.Generate(z, curr => curr + cellHeight)
            .Select(row => (new Vector3(x, 0, row), new Vector3(x + cellWidth * width, 0, row)))
            .Take(height + 1);

            cols = Util.Generate(x, curr => curr + cellWidth)
                           .Select(col => (new Vector3(col, 0, z), new Vector3(col, 0, z + cellHeight * height)))
                           .Take(width + 1);
        }
        else
        {
            rows = Util.Generate(y, curr => curr + cellHeight)
                        .Select(row => (new Vector3(x, row, 0), new Vector3(x + cellWidth * width, row, 0)))
                        .Take(height + 1);

            cols = Util.Generate(x, curr => curr + cellWidth)
                           .Select(col => (new Vector3(col, y, 0), new Vector3(col, y + cellHeight * height, 0)))
                           .Take(width + 1);
        }

        var allLines = rows.Concat(cols);

        foreach (var elem in allLines)
        {
            Gizmos.DrawLine(elem.Item1, elem.Item2);
        }

        if (buckets == null || areGizmosShutDown)
            return;

        var originalCol = GUI.color;
        GUI.color = Color.red;
        if (!activatedGrid)
        {
            var allElems = new List<IGridEntity>();
            foreach (var elem in buckets)
                allElems = allElems.Concat(elem).ToList();

            int connections = 0;
            foreach (var entity in allElems)
            {
                foreach (var neighbour in allElems.Where(x => x != entity))
                {
                    Gizmos.DrawLine(entity.Position, neighbour.Position);
                    connections++;
                }

                if (showLogs)
                    Debug.Log("tengo " + connections + " conexiones por individuo");
                connections = 0;
            }
        }
        else
        {
            int connections = 0;
            foreach (var elem in buckets)
            {
                foreach (var ent in elem)
                {
                    foreach (var n in elem.Where(x => x != ent))
                    {
                        Gizmos.DrawLine(ent.Position, n.Position);
                        connections++;
                    }

                    if (showLogs)
                        Debug.Log("tengo " + connections + " conexiones por individuo");
                    connections = 0;
                }
            }
        }

        GUI.color = originalCol;
        showLogs = false;
    }

    #endregion
}
