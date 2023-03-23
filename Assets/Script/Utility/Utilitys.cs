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



    #region interfaz


    static public IEnumerable<T> FindObjsWithInterface<T>()
    {
        return GameObject.FindObjectsOfType<MonoBehaviour>().OfType<T>();
    }


    #endregion


    #region lerps

    static public CompleteRoutine LerpInTime<T>(T original, T final, float seconds, System.Func<T, T, float, T> Lerp, System.Action<T> save)
    {
        return LerpInTime(() => original, () => final, seconds, Lerp, save);
    }

    static public CompleteRoutine LerpInTime<T>(T original, System.Func<T> final, float seconds, System.Func<T, T, float, T> Lerp, System.Action<T> save)
    {
        return LerpInTime(() => original, final, seconds, Lerp, save);
    }

    static public CompleteRoutine LerpInTime<T>(System.Func<T> original, T final, float seconds, System.Func<T, T, float, T> Lerp, System.Action<T> save)
    {
        return LerpInTime(original, () => final, seconds, Lerp, save);
    }


    static public CompleteRoutine LerpInTime<T>(System.Func<T> original, System.Func<T> final, float seconds, System.Func<T, T, float, T> Lerp, System.Action<T> save)
    {
        Timer tim = new Timer(seconds);

        System.Action

        update = () =>
        {
            save(Lerp(original(), final(), tim.Percentage()));
        }
        ,

        end = () =>
        {
            save(final());
        };

        tim = TimersManager.Create(seconds, update, end, true, true);

        return (CompleteRoutine)tim;
    }

    static public CompleteRoutine LerpWithCompare<T>(T original, T final, float velocity, System.Func<T, T, float, T> Lerp, System.Func<T, T, bool> compare, System.Action<T> save)
    {
        Timer tim = new Timer(1);

        System.Action

        update = () =>
        {
            original = Lerp(original, final, Time.deltaTime * velocity);
            save(original);
            if (compare(original, final))
                tim.Set(0);
            else
                tim.Reset();
        }
        ,
        end = () =>
        {
            save(final);

        };

        tim = TimersManager.Create(1, update, end, true, true);

        return (CompleteRoutine)tim;
    }

    #endregion

    /*
    /// <summary>
    /// remueve de una lista de FatherPwDbff y ejecuta el off correspondiente
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="pwDbffs"></param>
    /// <param name="index">indice a eliminar</param>
    public static void RemoveOff<T>(this List<T> pwDbffs, int index, Character me) where T : FatherPwDbff
    {
        if (pwDbffs.Count <= index)
            return;

        var aux = pwDbffs[index];

        pwDbffs.RemoveAt(index);

        aux?.OnExitState(me);
    }
    */

    public static void AddSingleAction(ref System.Action thisAction, System.Action action)
    {
        foreach (var item in thisAction.GetInvocationList())
        {
            if (item.Method.Name == action.Method.Name)
                return;
        }

        thisAction += action;
    }
}

