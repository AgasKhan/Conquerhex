using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region parent
public abstract class DetectParent<T> where T : class
{
    protected const int cantidad = 50;

    [Tooltip("Radio maximo de deteccion")]
    public float maxRadius;

    [Tooltip("Radio minimo de deteccion")]
    public float minRadius;

    [Tooltip("si arranca por el mas lejano, en vez del mas cercano")]
    public bool inverse;

    [Tooltip("Producto punto utilizado para el calculo del cono")]
    public float dot = 1;

    [SerializeField]
    protected int _maxDetects;

    [SerializeField]
    protected int _minDetects;

    protected CompareDist<Component> compareDist = new CompareDist<Component>();

    protected Transform[] transformsBuffer = new Transform[cantidad];

    protected int length;

    /// <summary>
    /// diametro
    /// </summary>
    public float diameter => maxRadius * 2;

    /// <summary>
    /// numero maximo de objetos detectados
    /// </summary>
    public int maxDetects => _maxDetects;

    /// <summary>
    /// numero minimo de objetos detectados
    /// </summary>
    public int minDetects => _minDetects;

    public abstract List<T> Area(Vector3 pos, int min, int max, System.Func<T, bool> chck, float minRadius, float maxRadius);

    public abstract List<T> Cone(Vector3 pos, Vector3 dir, int min, int max, System.Func<T, bool> chck, float minRadius, float maxRadius, float dot);

    public abstract List<T> Ray(Vector3 pos, Vector3 dir, System.Func<T, bool> chck, int min, int max, float distance = -1);

    public abstract void RayTransform(Vector3 pos, Vector3 dir, System.Func<Transform, bool> chck, int min, int max, float distance = -1);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="chck">Criterio de busqueda</param>
    /// <returns></returns>
    public List<T> Area(Vector3 pos, System.Func<T, bool> chck)
    {
        return Area(pos, minDetects, maxDetects, chck, minRadius, maxRadius);
    }

    public List<T> Cone(Vector3 pos, Vector3 dir, System.Func<T, bool> chck)
    {
        return Cone(pos, dir, minDetects, maxDetects, chck, minRadius, maxRadius, dot);
    }

    /// <summary>
    /// Ray que devuelve el transform
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="dir"></param>
    /// <param name="distance"></param>
    /// <returns></returns>
    public void RayTransform(Vector3 pos, Vector3 dir, System.Func<Transform, bool> chck, float distance = -1)
    {
        RayTransform(pos, dir, chck, minDetects, maxDetects, distance);
    }

    /// <summary>
    /// Ray que devuelve el tipo de la clase
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="dir"></param>
    /// <param name="distance"></param>
    /// <returns></returns>
    public List<T> Ray(Vector3 pos, Vector3 dir, float distance = -1)
    {
        return Ray(pos, dir, (entity) => true, minDetects, maxDetects,distance);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="caster"></param>
    /// <param name="chck">Si cumple con el criterio de busqueda</param>
    /// <returns></returns>

    public List<T> ConeWithRay(Transform caster, Vector3 dir, System.Func<T, bool> chck, int maxDetects, float minRadius, float maxRadius, float dot)
    {
        return WithRay(caster, Cone(caster.position, dir, 0, 0, chck, minRadius, maxRadius, dot), maxDetects);
    }

    public List<T> ConeWithRay(Transform caster, Vector3 dir, System.Func<T, bool> chck)
    {
        return ConeWithRay(caster, dir, chck, maxDetects, minRadius, maxRadius, dot);
    }

    public List<T> ConeWithRay(Transform caster, System.Func<T, bool> chck)
    {
        return ConeWithRay(caster, caster.right, chck, maxDetects, minRadius, maxRadius, dot);
    }

    public List<T> AreaWithRay(Transform caster, System.Func<T, bool> chck, int maxDetects, float minRadius, float maxRadius)
    {
        return WithRay(caster, Area(caster.transform.position, minDetects, 0, chck, minRadius, maxRadius), maxDetects);
    }

    public List<T> AreaWithRay(Transform caster, System.Func<T, bool> chck)
    {
        return AreaWithRay(caster, chck, maxDetects, minRadius, maxRadius);
    }



    protected virtual void Add(List<T> list, T add, Vector3 pos)
    {
        if (list.Contains(add))
            return;

        InternalAdd(list, add as Component, pos);
    }

    protected virtual void Add(List<RaycastHit2D> list, RaycastHit2D add, Vector3 pos)
    {
        if (list.Contains(add))
            return;

        list.Add(add);
    }

    void InternalAdd(List<T> list, Component add, Vector3 pos)
    {
        if (add == null)
            return;

        compareDist.inverse = inverse;

        compareDist.me = pos;

        //busqueda binaria para determinar el candidato a comparar mas cercano
        int cmp, max = list.Count - 1, min = 0, searchIndex = 0;

        while (min <= max)
        {
            searchIndex = ((max - min) / 2) + min;

            cmp = compareDist.Compare(add, list[searchIndex] as Component);

            if (cmp > 0)
            {
                min = searchIndex + 1;
            }
            else
            {
                max = searchIndex - 1;
            }
        }

        //una vez que encontre el lugar mas cercano para comparar ahora me fijo, si va antes o despues
        if (list.Count > 0)
        {
            searchIndex = compareDist.Compare(add, list[searchIndex] as Component) + searchIndex;
        }

        list.AddOrInsert(add as T, searchIndex);
    }

    /// <summary>
    /// Remueve de la lista aquellos que no tienen vision directa con el caster
    /// </summary>
    /// <param name="caster"></param>
    /// <param name="damageables"></param>
    /// <param name="maxDetects"></param>
    /// <returns></returns>
    protected List<T> WithRay(Transform caster, List<T> damageables, int maxDetects)
    {
        var pos = caster.position.Vect3To2();

        for (int i = damageables.Count - 1; i >= 0; i--)
        {
            var posDamageable = (damageables[i] as Component).transform.position.Vect3To2();

            RayTransform(posDamageable, (pos - posDamageable), (tr) => true, 0, 0, (pos - posDamageable).magnitude);

            if (length > 0)//si colisiono significa q no tengo vision directa
            {
                bool chckCaster = true;

                //Para chequear si en algunos de los primeros esta el caster
                for (int ii = 0; ii < Mathf.Clamp(length, 0, 2); ii++)
                {
                    if (transformsBuffer[ii].transform == caster)
                    {
                        chckCaster = false;
                    }
                }

                if (chckCaster)
                    damageables.RemoveAt(i);
            }
        }

        if (maxDetects > 0)
        {
            var count = damageables.Count;

            if (maxDetects > count)
            {
                maxDetects = count;
            }

            for (int i = 0; i < (count - maxDetects); i++)
            {
                damageables.RemoveAt(count - (1 + 1 * i));
            }
        }

        return damageables;
    }
}

#endregion

[System.Serializable]
public class Detect3D<T> : DetectParent<T> where T : class
{
    [SerializeField]
    LayerMask layerMask;

    List<T> results = new List<T>();

    Collider[] buffer = new Collider[cantidad];

    RaycastHit[] auxRaycastHit = new RaycastHit[cantidad];

    public override List<T> Area(Vector3 position, int min, int max, System.Func<T, bool> chck, float minRadius, float maxRadius)
    {
        results.Clear();

        length = Physics.OverlapCapsuleNonAlloc(position.Vect3Copy_Z(-10), position.Vect3Copy_Z(10), maxRadius, buffer, layerMask);

        for (int i = 0; i < length; i++)
        {
            if(!buffer[i].IsInRadius(position, minRadius) && buffer[i].TryGetComponent(out T obj) && chck(obj))
            {
                Add(results, obj, position);
            }

            if(max>0 && results.Count-1>=max)
            {
                return results;
            }
        }

        if (results.Count < min)
        {
            results.Clear();
        }

        //lento
        /*
        foreach (var item in hexagoneToSearch().AllChildEntities)
        {
            foreach (var transform in item.container.carlitos)
            {
                if (item.IsInRadius(transform, maxRadius) && !item.IsInRadius(transform, minRadius) && item.TryGetInContainer<T>(out var aux) && chck(aux))
                {
                    Add(results, aux, position);
                    break;
                }
            }
        }
        */
        return results;
    }

    public override List<T> Cone(Vector3 pos, Vector3 dir, int min, int max, System.Func<T, bool> chck, float minRadius, float maxRadius, float dot)
    {
        dir.Normalize();

        var copyChck = chck;

        chck = (T obj) =>
          {
              var posDamageable = (obj as Component).transform.position;

              var vecDot = Vector2.Dot((posDamageable - pos).normalized.Vect3To2(), dir.Vect3To2());

              //Debug.Log($"MyPos: {pos} - {(results[i] as Component).name} pos: {posDamageable} - dir: {dir} - normalize {(posDamageable - pos).normalized} - {dot} > {vecDot}={dot > vecDot}");

              return copyChck(obj) && dot > vecDot;
          };

        Area(pos, min, max, chck, minRadius, maxRadius);

        return results;
    }

    public override List<T> Ray(Vector3 pos, Vector3 dir, System.Func<T, bool> chck, int min, int max, float distance = -1)
    {
        results.Clear();

        RayTransform(pos, dir, (tr) => 
        {
            bool b = tr.TryGetComponent<T>(out var toAdd) && chck(toAdd);

            if(b)
                Add(results, toAdd, pos);

            return b;
        }, min, max, distance);

        if (length < min)
            results.Clear();

        return results;
    }

    public override void RayTransform(Vector3 pos, Vector3 dir, System.Func<Transform, bool> chck, int min, int max, float distance = -1)
    {
        length = Physics.RaycastNonAlloc(pos, dir, auxRaycastHit, distance > 0 ? distance : float.PositiveInfinity, layerMask);

        int j = 0;

        for (int i = 0; i < length; i++)
        {
            if(chck(auxRaycastHit[i].transform))
            {
                transformsBuffer[j] = auxRaycastHit[i].transform;
                j++;
            }
            if(max > 0 && j >=max)
            {
                length = j;
                return;
            }
        }

        if (min > j)
        {
            length = 0;
            return;
        }
        
        length = j;
    }

}

#region old


[System.Serializable]
public class Detect<T> : DetectParent<T> where T : class
{
    public ContactFilter2D contactFilter2D;

    List<T> results = new List<T>();

    List<Collider2D> auxCollider = new List<Collider2D>();

    List<RaycastHit2D> auxRaycastHit = new List<RaycastHit2D>();

    public override List<T> Area(Vector3 pos, int min, int max, System.Func<T, bool> chck, float minRadius, float maxRadius)
    {
        results.Clear();

        length = Physics2D.OverlapCircle(pos, maxRadius, contactFilter2D, auxCollider);

        if (min > length)
            return results;

        for (int i = 0; i < length; i++)
        {
            if (auxCollider[i].TryGetComponent<T>(out var toAdd) && !auxCollider[i].IsInRadius(pos, minRadius) && chck(toAdd))
                Add(results, toAdd, pos);
        }

        return results;
    }

    public override List<T> Cone(Vector3 pos, Vector3 dir, int min, int max, System.Func<T, bool> chck, float minRadius, float maxRadius, float dot)
    {
        Area(pos, min, max, chck, minRadius, maxRadius);

        dir.Normalize();

        for (int i = results.Count - 1; i >= 0; i--)
        {
            var posDamageable = (results[i] as Component).transform.position;

            var vecDot = Vector2.Dot((posDamageable - pos).normalized.Vect3To2(), dir.Vect3To2());

            //Debug.Log($"MyPos: {pos} - {(results[i] as Component).name} pos: {posDamageable} - dir: {dir} - normalize {(posDamageable - pos).normalized} - {dot} > {vecDot}={dot > vecDot}");

            if (dot > vecDot)
            {
                results.RemoveAt(i);
            }
        }

        return results;
    }

    public override List<T> Ray(Vector3 pos, Vector3 dir, System.Func<T, bool> chck, int min, int max, float distance = -1)
    {
        results.Clear();

        RayTransform(pos, dir, (tr) =>
        {
            bool b = tr.TryGetComponent<T>(out var toAdd) && chck(toAdd);

            if (b)
                Add(results, toAdd, pos);

            return b;
        }, min, max, distance);

        if (length < min)
            results.Clear();

        return results;
    }

    public override void RayTransform(Vector3 pos, Vector3 dir, System.Func<Transform, bool> chck, int min, int max, float distance = -1)
    {
        length = Physics2D.Raycast(pos, dir, contactFilter2D, auxRaycastHit, distance > 0 ? distance : float.PositiveInfinity);

        int j = 0;

        for (int i = 0; i < length; i++)
        {
            if (chck(auxRaycastHit[i].transform))
            {
                transformsBuffer[j] = auxRaycastHit[i].transform;
                j++;
            }
            if (max>0 && j >= max)
            {
                length = j;
                return;
            }
        }

        if (min > j)
        {
            length = 0;
            return;
        }

        length = j;
    }

}

#endregion

public class CompareDist<T> : IComparer<T> where T : Component
{
    public Vector3 me;

    public bool inverse = false;

    public int Compare(T x, T y)
    {
        var aux = (x.transform.position - me).magnitude > (y.transform.position - me).magnitude;

        if (inverse)
            aux = !aux;

        return System.Convert.ToInt32(aux);
    }
}


/*

[System.Serializable]
public class DetectAlloc<T> : DetectParent<T> where T : class
{
    [Tooltip("Layer de deteccion")]
    public LayerMask layerMask;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="chck">Criterio de busqueda</param>
    /// <returns></returns>
    public override List<T> Area(Vector3 pos, int min, int max, System.Func<T, bool> chck, float minRadius, float maxRadius)
    {
        var aux = Physics2D.OverlapCircleAll(pos, maxRadius, layerMask);

        List<T> damageables = new List<T>();

        if (min > aux.Length)
            return damageables;

        foreach (var item in aux)
        {
            var components = item.GetComponents<T>();

            foreach (var comp in components)
            {
                if (chck(comp) && (comp as Component).IsInRadius(pos,minRadius))
                    Add(damageables, comp, pos);
            }

            if (max > 0)
                for (int i = 1; i < damageables.Count - max; i++)
                {
                    damageables.RemoveAt(damageables.Count - i);
                }
        }

        return damageables;
    }

    public override List<T> Cone(Vector3 pos, Vector3 dir, int min, int max, System.Func<T, bool> chck, float minRadius, float maxRadius, float dot)
    {
        var damageables = Area(pos, 0, 0, chck, minRadius, maxRadius);

        dir.Normalize();

        for (int i = damageables.Count - 1; i >= 0; i--)
        {
            var posDamageable = (damageables[i] as Component).transform.position;

            if (dot > Vector2.Dot((posDamageable - pos).normalized.Vect3To2(), dir.Vect3To2()))
            {
                damageables.RemoveAt(i);
            }
        }

                
        dir *= radius;
        
        Debug.DrawRay(pos, dir, Color.red);

        Debug.DrawRay(pos, Quaternion.Euler(0, 0, Mathf.Acos(dot) * Mathf.Rad2Deg) * dir, Color.blue);

        Debug.DrawRay(pos, Quaternion.Euler(0, 0, -Mathf.Acos(dot) * Mathf.Rad2Deg) * dir, Color.blue);
        

        return damageables;
    }


    public override List<RaycastHit2D> RayTransform(Vector3 pos, Vector3 dir, System.Func<Transform, bool> chck, int min, int max, float distance = -1)
    {
        var aux = Physics2D.RaycastAll(pos, dir, distance < 0 ? this.maxRadius : distance, layerMask);

        Debug.DrawRay(pos, dir, Color.red, 1);

        if (aux.Length > min)
        {
            List<RaycastHit2D> tr = new List<RaycastHit2D>();

            //string str = "";

            foreach (var item in aux)
            {
                if (chck(item.transform))
                {
                    Add(tr, item, pos);
                    //str += item.transform.name + "- ";
                }
            }

            //Debug.Log(str);

            if (max > 0)
                for (int i = 1; i <= (tr.Count - max); i++)
                {
                    tr.RemoveAt(tr.Count - i);
                }

            return tr;
        }

        return null;
    }



    public override List<T> Ray(Vector3 pos, Vector3 dir, System.Func<T, bool> chck, float distance = -1)
    {
        List<T> result = new List<T>();

        var tr = RayTransform(pos, dir, (tr) => true, distance);

        foreach (var item in tr)
        {
            var aux = item.collider.GetComponents<T>();

            foreach (var tType in aux)
            {
                if (chck(tType))
                    Add(result, tType, pos);
            }
        }

        if (result.Count > 0)
            return result;
        else
            return null;
    }

}
*/