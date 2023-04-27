using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Detect<T>
{
    public float radius;

    public float distance;

    public LayerMask layerMask;

    public int maxDetects = -1;

    public int minDetects = 1;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="chck">Criterio de busqueda</param>
    /// <returns></returns>
    public T[] Area(Vector2 pos, System.Func<T, bool> chck)
    {
        var aux = Physics2D.OverlapCircleAll(pos, radius, layerMask);

        List<T> damageables = new List<T>();

        foreach (var item in aux)
        {
            var components = item.GetComponents<T>();

            foreach (var comp in components)
            {
                if (chck(comp))
                    damageables.Add(comp);
            }
        }

        return damageables.ToArray();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="caster"></param>
    /// <param name="chck">Si cumple con el criterio de busqueda</param>
    /// <param name="chckTr">compara si es el casteador para ver si tenemos vision directa</param>
    /// <returns></returns>
    public T[] AreaWithRay(Vector2 pos, Vector2 caster, System.Func<T, bool> chck, System.Func<Transform, bool> chckTr)
    {
        List<T> damageables = new List<T>(Area(pos, chck));

        for (int i = damageables.Count; i >= 0; i--)
        {
            var aux = RayTransform(pos, (caster - pos), (caster - pos).magnitude);

            if (aux != null && aux.Length > 0 && !chckTr(aux[minDetects]))
            {
                damageables.RemoveAt(i);
            }
        }

        return damageables.ToArray();
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
        var aux = Physics2D.RaycastAll(pos, dir, distance < 0 ? this.distance : distance, layerMask);

        Debug.DrawRay(pos, dir, Color.red, 1);

        if (aux.Length >= minDetects)
        {
            List<Transform> tr = new List<Transform>();

            Transform[] result = new Transform[maxDetects > 0 ? maxDetects : tr.ToArray().Length - minDetects - 1];

            foreach (var item in aux)
            {
                tr.Add(item.transform);
            }

            System.Array.ConstrainedCopy(tr.ToArray(), minDetects - 1, result, 0, result.Length);

            return result;
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
                    result.Add(tType);
            }
        }

        if (result.Count > 0)
            return result.ToArray();
        else
            return null;
    }


}
