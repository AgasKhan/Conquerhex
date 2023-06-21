using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Detect<T> where T : class
{
    [Tooltip("Radio de deteccion")]
    public float radius;

    public float distance;

    [Tooltip("Layer de deteccion")]
    public LayerMask layerMask;

    [SerializeField]
    int _maxDetects;

    [SerializeField]
    int _minDetects;


    [Tooltip("si arranca por el mas lejano, en vez del mas cercano")]
    public bool inverse;

    [Tooltip("Producto punto utilizado para el calculo del cono")]
    public float dot = 1;

    /// <summary>
    /// numero maximo de objetos detectados
    /// </summary>
    public int maxDetects => _maxDetects;

    /// <summary>
    /// numero minimo de objetos detectados
    /// </summary>
    public int minDetects => _minDetects;

    /// <summary>
    /// diametro
    /// </summary>
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

    public List<T> Cone(Vector2 pos, Vector2 dir, int min, int max, System.Func<T, bool> chck, float radius, float dot)
    {
        var damageables = Area(pos, 0, 0, chck, radius);

        for (int i = damageables.Count - 1; i >= 0; i--)
        {
            var posDamageable = (damageables[i] as Component).transform.position.Vect3To2();

            if (dot > Vector2.Dot((posDamageable - pos).normalized, dir))
            {
                damageables.RemoveAt(i);
                continue;
            }
        }

        return damageables;
    }

    public List<T> Cone(Vector2 pos, Vector2 dir,System.Func<T, bool> chck)
    {
        return Cone(pos,dir,minDetects, maxDetects, chck, radius, dot);
    }

 
    public Transform[] RayTransform(Vector2 pos, Vector2 dir, int min, int max , float distance = -1)
    {
        var aux = Physics2D.RaycastAll(pos, dir, distance < 0 ? this.distance : distance, layerMask);

        Debug.DrawRay(pos, dir, Color.red, 1);

        if (aux.Length > min)
        {
            List<Transform> tr = new List<Transform>();

            string str = "";

            foreach (var item in aux)
            {
                str += item.transform.name +"- ";
                Add(tr, item.transform, pos);
            }

            Debug.Log(str);

            if(max>0)
                for (int i = 1; i <= (tr.Count - max); i++)
                {
                    tr.RemoveAt(tr.Count - i);
                }

            return tr.ToArray();
        }

        return null;
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




    public List<T> Ray(Vector2 pos, Vector2 dir, System.Func<T, bool> chck, float distance = -1)
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
            return result;
        else
            return null;
    }

    /// <summary>
    /// Ray que devuelve el tipo de la clase
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="dir"></param>
    /// <param name="distance"></param>
    /// <returns></returns>
    public List<T> Ray(Vector2 pos, Vector2 dir, float distance = -1)
    {
        return Ray(pos, dir, (entity) => true, distance);
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="caster"></param>
    /// <param name="chck">Si cumple con el criterio de busqueda</param>
    /// <returns></returns>

    public List<T> ConeWithRay(Transform caster, Vector2 dir, System.Func<T, bool> chck, int maxDetects, float radius, float dot)
    {
        return WithRay(caster, Cone(caster.position, dir, 0, 0, chck, radius, dot), maxDetects);
    }

    public List<T> ConeWithRay(Transform caster, System.Func<T, bool> chck)
    {
        return ConeWithRay(caster, caster.right, chck, maxDetects, radius, dot);
    }

    public List<T> AreaWithRay(Transform caster, System.Func<T, bool> chck, int maxDetects, float radius)
    {
        return WithRay(caster, Area(caster.transform.position, minDetects, 0, chck, radius), maxDetects);
    }

    public List<T> AreaWithRay(Transform caster, System.Func<T, bool> chck)
    {
        return AreaWithRay(caster, chck, maxDetects, radius);
    }

    /// <summary>
    /// Remueve de la lista aquellos que no tienen vision directa con el caster
    /// </summary>
    /// <param name="caster"></param>
    /// <param name="damageables"></param>
    /// <param name="maxDetects"></param>
    /// <returns></returns>
    List<T> WithRay(Transform caster, List<T> damageables, int maxDetects)
    {
        var pos = caster.position.Vect3To2();

        for (int i = damageables.Count - 1; i >= 0; i--)
        {
            var posDamageable = (damageables[i] as Component).transform.position.Vect3To2();

            var aux = RayTransform(posDamageable, (pos - posDamageable), 0, 0, (pos - posDamageable).magnitude);

            if (aux != null && aux.Length > 0)//si colisiono significa q no tengo vision directa
            {
                bool chckCaster = true;

                //Para chequear si en algunos de los primeros esta el player
                for (int ii = 0; ii < Mathf.Clamp(aux.Length, 0, 2); ii++)
                {
                    if (aux[ii] == caster)
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

    protected virtual void Add(List<T> list, T add, Vector3 pos)
    {
        InternalAdd(list, add as Component, pos);
    }

    protected virtual void Add(List<Transform> list, Transform add, Vector3 pos)
    {
        list.Add(add);
    }

    void InternalAdd(List<T> list, Component add, Vector3 pos)
    {
        CompareDist<Component> compareDist = new CompareDist<Component>();

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
}

public class CompareDist<T> : IComparer<T> where T: Component
{
    public Vector3 me;

    public bool inverse=false;

    public int Compare(T x, T y)
    {
        var aux = (x.transform.position - me).magnitude > (y.transform.position - me).magnitude;

        if (inverse)
            aux = !aux;

        return System.Convert.ToInt32(aux);
    }
}
