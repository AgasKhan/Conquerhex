using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public static class Utilitys
{

    /// <summary>
    /// 
    /// </summary>
    /// <param name="v"></param>
    /// <param name="forward"></param>
    /// <param name="axis"></param>
    /// <param name="clockwise"></param>
    /// <returns></returns>
    static public float AngleOffAroundAxis(Vector3 dir, Vector3 forward, Vector3 axis, bool clockwise = true)
    {
        Vector3 right;
        if (clockwise)
        {
            right = Vector3.Cross(forward, axis);
            forward = Vector3.Cross(axis, right);
        }
        else
        {
            right = Vector3.Cross(axis, forward);
            forward = Vector3.Cross(right, axis);
        }
        return Mathf.Atan2(Vector3.Dot(dir, right), Vector3.Dot(dir, forward)) * Mathf.Rad2Deg;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dir"></param>
    /// <param name="angle">devuelve el angulo entre los vectores</param>
    /// <param name="from"></param>
    /// <param name="axis"></param>
    /// <param name="MyRot"></param>
    /// <returns>retorna la diferencia absoluta entre el angulo y la rotacion del objeto</returns>
    static public float DeltaAngle(Vector3 dir, Vector3 from, out float angle, Vector3 axis, Quaternion MyRot)
    {
        angle = AngleOffAroundAxis(from, dir, axis);

        angle = angle < 0 ? 360 + angle : angle;

        float rest = 0;

        for (int i = 0; i < 3; i++)
        {
            if (axis[i] != 0)
                rest = MyRot.eulerAngles[i];
        }

        return Mathf.Abs(angle - rest);
    }

    /// <summary>
    /// Compara la direccion ingresada con el forward del objeto, y devuelve su angulo en el eje Y
    /// </summary>
    /// <param name="dir">direccion a comparar</param>
    /// <param name="angle">flotante donde deveolvera el angulo de rotacion</param>
    /// <returns>retorna la diferencia absoluta entre el angulo y rotacion del objeto (sirve para calcular el cono)</returns>
    static public float DeltaAngleY(Vector3 dir, out float angle, Quaternion MyRot)
    {
        return DeltaAngle(dir, Vector3.forward, out angle,  Vector3.up, MyRot);
    }


    /// <summary>
    /// devuelve la diferencia real entre angulos de 2 vectores <br/> OJO QUE ESTA FUNCION SOLO SIRVE CON VECTOR2
    /// </summary>
    /// <param name="vec1"></param>
    /// <param name="vec2"></param>
    /// <returns></returns>
    public static float DifAngulosVectores(Vector2 vec1, Vector2 vec2)
    {
        float angle = Vector2.SignedAngle(vec1, vec2);

        return angle < 0 ? 360 + angle : angle;
    }


    /// <summary>
    /// Calcula un vector 2 en base al angulo y su magnitud
    /// </summary>
    /// <param name="x"></param>
    /// <param name="m"></param>
    /// <returns></returns>
    public static Vector2 VecFromDegs(float x, float m = 1)
    {
        x *= Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(x),Mathf.Sin(x))*m;
    }

    public static float DotCalculate(float angle)
    {
        return angle == 360 ? -1 : Vector2.Dot(Vector2.right, Quaternion.Euler(0, 0, angle) * Vector2.right);
    }

    #region interfaz


    static public IEnumerable<T> FindObjsWithInterface<T>()
    {
        return GameObject.FindObjectsOfType<MonoBehaviour>().OfType<T>();
    }

    static public void InitAll(IEnumerable<Init> init)
    {
        foreach (var item in init)
        {
            item.Init();
        }
    }
    #endregion

    public static IEnumerable<T> VoidEnumerable<T>()
    {
        return new T[0];
    }

    public static void DrawArrowRay(Vector3 position, Vector3 dir)
    {
        Gizmos.DrawRay(position, dir);

        Gizmos.DrawRay((position + dir), Quaternion.Euler(0, 0, 30) * (dir*-1)/10);

        Gizmos.DrawRay((position + dir), Quaternion.Euler(0, 0, -30) * (dir * -1)/10);

        Gizmos.DrawRay((position + dir), Quaternion.Euler(30, 0, 0) * (dir * -1) / 10);

        Gizmos.DrawRay((position + dir), Quaternion.Euler(-30, 0, 0) * (dir * -1) / 10);
    }

    public static void DrawArrowLine(Vector3 position, Vector3 to)
    {
        DrawArrowRay(position, (to-position));
    }
}

