using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Detect<T>
{
    [Tooltip("Radio de deteccion")]
    public float radius;

    public float distance;

    [Tooltip("Layer de deteccion")]
    public LayerMask layerMask;

    /// <summary>
    /// numero maximo de objetos detectados
    /// </summary>
    public int maxDetects = -1;

    /// <summary>
    /// numero minimo de objetos detectados
    /// </summary>
    public int minDetects = 0;

    public float diameter => radius * 2;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="chck">Criterio de busqueda</param>
    /// <returns></returns>
    public List<T> Area(Vector2 pos, int min, int max, System.Func<T, bool> chck, float radius)
    {
        var aux = Physics2D.OverlapCircleAll(pos, radius, layerMask);

        List<T> damageables = new List<T>();

        if (min > aux.Length)
            return damageables;
        
        foreach (var item in aux)
        {
            var components = item.GetComponents<T>();

            foreach (var comp in components)
            {
                if (chck(comp))
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="chck">Criterio de busqueda</param>
    /// <returns></returns>
    public List<T> Area(Vector2 pos, System.Func<T, bool> chck)
    {
        return Area(pos, minDetects , maxDetects, chck, radius);
    }

    /// <summary>
    /// Ray que devuelve el transform
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="dir"></param>
    /// <param name="distance"></param>
    /// <returns></returns>
    public Transform[] RayTransform(Vector2 pos, Vector2 dir, float distance = -1)
    {
        return RayTransform(pos, dir, minDetects, maxDetects, distance);
    }

    public Transform[] RayTransform(Vector2 pos, Vector2 dir, int min, int max , float distance = -1)
    {
        var aux = Physics2D.RaycastAll(pos, dir, distance < 0 ? this.distance : distance, layerMask);

        Debug.DrawRay(pos, dir, Color.red, 1);

        if (aux.Length > min)
        {
            List<Transform> tr = new List<Transform>();

            foreach (var item in aux)
            {
                Add(tr, item.transform, pos);
            }

            if(max>0)
                for (int i = 1; i <= (tr.Count - max); i++)
                {
                    tr.RemoveAt(tr.Count - i);
                }

            Debug.Log("cantidad detectada: " + tr.Count);

            return tr.ToArray();
        }

        return null;
    }

    /// <summary>
    /// Ray que devuelve el tipo de la clase
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="dir"></param>
    /// <param name="distance"></param>
    /// <returns></returns>
    public T[] Ray(Vector2 pos, Vector2 dir, float distance = -1)
    {
        return Ray(pos, dir, (entity) => true, distance);
    }

    public T[] Ray(Vector2 pos, Vector2 dir, System.Func<T, bool> chck, float distance = -1)
    {
        List<T> result = new List<T>();

        var tr = RayTransform(pos, dir, distance);

        foreach (var item in tr)
        {
            var aux = item.GetComponents<T>();

            foreach (var tType in aux)
            {
                if (chck(tType))
                    Add(result, tType, pos);
            }
        }

        if (result.Count > 0)
            return result.ToArray();
        else
            return null;
    }

    protected virtual void Add(List<T> list, T add, Vector3 pos)
    {
        list.Add(add);
    }

    protected virtual void Add(List<Transform> list, Transform add, Vector3 pos)
    {
        list.Add(add);
    }
}


[System.Serializable]
public class DetectSort<T> : Detect<T> where T : Component
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="caster"></param>
    /// <param name="chck">Si cumple con el criterio de busqueda</param>
    /// <returns></returns>

    public T[] AreaWithRay(Transform caster, System.Func<T, bool> chck, float radius)
    {
        List<T> damageables = new List<T>(Area(caster.position, 0, 0, chck, radius));

        for (int i = damageables.Count - 1; i >= 0; i--)
        {
            var posDamageable = damageables[i].transform.position.Vect3To2();

            var pos = caster.position.Vect3To2();

            var aux = RayTransform(posDamageable, (pos - posDamageable), 0, 0, (pos - posDamageable).magnitude);

            if (aux != null  )//si colisiono significa q no tengo vision directa
            {
                bool chckCaster = true;

                for (int ii = 0; ii < Mathf.Clamp(aux.Length,0,2); ii++)//Para 
                {
                    if(aux[ii] != caster)
                    {
                        chckCaster = false;
                    }
                }

                if(chckCaster)
                    damageables.RemoveAt(i);
            }
        }

        if (maxDetects > 0)
        {
            if (damageables.Count < maxDetects)
                maxDetects = damageables.Count;

            var aux = damageables.Count;

            for (int i = 0; i < (aux - maxDetects); i++)
            {
                damageables.RemoveAt(aux - 1 - (1*i));
            }
        }

        return damageables.ToArray();
    }

    public T[] AreaWithRay(Transform caster, System.Func<T, bool> chck)
    {
        return AreaWithRay(caster, chck, radius);
    }

    protected override void Add(List<T> list, T add, Vector3 pos)
    {
        InternalAdd(list, add, pos);
    }

    protected override void Add(List<Transform> list, Transform add, Vector3 pos)
    {
        InternalAdd(list, add, pos);
    }

    void InternalAdd<Generic>(List<Generic> list, Generic add, Vector3 pos) where Generic : Component
    {
        CompareDist<Generic> compareDist = new CompareDist<Generic>();

        compareDist.me = pos;

        //busqueda binaria para determinar el candidato a comparar mas cercano
        int cmp, max = list.Count-1, min = 0, searchIndex = 0;

        while (min <= max) 
        {
            searchIndex = ((max - min) / 2) + min;

            cmp = compareDist.Compare(add, list[searchIndex]);

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
        if(list.Count>0)
        {
            searchIndex =  compareDist.Compare(add, list[searchIndex]) + searchIndex;
        }

        list.AddOrInsert(add, searchIndex);

        /*
        string aux = "";

        foreach (var item in list)
        {
            aux += (item.transform.position - pos).magnitude + "-";
        }

        Debug.Log(aux);
        */
    }
}


public class CompareDist<T> : IComparer<T> where T: Component
{
    public Vector3 me;

    public int Compare(T x, T y)
    {
        if ((x.transform.position - me).magnitude > (y.transform.position - me).magnitude)
            return 1;
        else
            return 0;
    }
}
