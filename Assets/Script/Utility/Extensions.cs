using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Internal;


public static class Extensions
{

    #region Tags

    /*
    static public void AddTags(this GameObject g, params Tag[] t)
    {
        Tags.AddTags(g, t);
    }

    static public void RemoveTags(this GameObject g, params Tag[] t)
    {
        Tags.RemoveTags(g, t);
    }
    
    /// <summary>
    /// Debuelve true en caso de que todas las tags coincidan
    /// </summary>
    static public bool CompareTags(this GameObject g, params Tag[] t)
    {
        return Tags.ChckAll(g, t);
    }


    /// <summary>
    /// devuelve true en caso de que una tag coincida
    /// </summary>
    static public bool CompareOneTags(this GameObject g, params Tag[] t)
    {
        return Tags.ChckOne(g, t);
    }

    static public GameObject[] FindWithTags(this GameObject g, params Tag[] t)
    {
        return Tags.Find(t);
    }


    static public void AddTags(this GameObject g, params string[] t)
    {
        Tags.AddTags(g, t);
    }

    static public void RemoveTags(this GameObject g, params string[] t)
    {
        Tags.RemoveTags(g, t);
    }

    /// <summary>
    /// Debuelve true en caso de que todas las tags coincidan
    /// </summary>
    /// <param name="g"></param>
    /// <param name="t"></param>
    /// <returns></returns>
    static public bool CompareTags(this GameObject g, params string[] t)
    {
        return Tags.ChckAll(g, t);
    }

    /// <summary>
    /// devuelve true en caso de que una tag coincida
    /// </summary>
    /// <param name="g"></param>
    /// <param name="t"></param>
    /// <returns></returns>
    static public bool CompareOneTags(this GameObject g, params string[] t)
    {
        return Tags.ChckOne(g, t);
    }

    /// <summary>
    /// buscar todos los objetos con dichas tags
    /// </summary>
    /// <param name="g"></param>
    /// <param name="t"></param>
    /// <returns></returns>
    static public GameObject[] FindWithTags(this GameObject g, params string[] t)
    {
        return Tags.Find(t);
    }

    */

    #endregion

    #region Vectors

    /// <summary>
    /// devuelve una magnitud aproximada del vector donde el valor de este es determinado por la mayor de todas sus proyecciones
    /// muy util para el input del movimiento
    /// </summary>
    /// <param name="v"></param>
    /// <returns></returns>
    static public float AproxMagnitude(this Vector2 v)
    {
        var n1 = Mathf.Abs(v.x);
        var n2 = Mathf.Abs(v.y);

        return n1 > n2 ? n1 : n2;
    }

    /// <summary>
    /// devuelve una magnitud aproximada del vector donde el valor de este es determinado por la mayor de todas sus proyecciones
    /// muy util para el input del movimiento
    /// </summary>
    /// <param name="v"></param>
    /// <returns></returns>
    static public float AproxMagnitude(this Vector3 v)
    {
        var n1 = Mathf.Abs(v.z);

        return (AproxMagnitude(v.Vect3To2()) > n1 ? AproxMagnitude(v.Vect3To2()) : n1);
    }

    /// <summary>
    /// Quita el parametro z del vector 3 y devuelve un vector 2
    /// </summary>
    /// <param name="v">Vector que modifica</param>
    /// <returns></returns>
    static public Vector2 Vect3To2(this Vector3 v)
    {
        return new Vector2(v.x,v.y);
    }

    static public Vector3 Vec2to3(this Vector2 v, float z)
    {
        return new Vector3(v.x, v.y, z);
    }
    #endregion

    #region string

    static public string RichText(this string s, string tag, string valor="")
    {
        if(valor!="")
            return "<"+tag+"="+valor+">"+s+"</"+tag+">" ;
        else
            return "<" + tag + ">" + s + "</" + tag + ">";
    }

    #endregion


    #region eventos botones

    
    public static void Event(this Button b)
    {
        var menu = MenuManager.instance;
        

        b.onClick.RemoveAllListeners();

        UnityEngine.Events.UnityAction unityAction;

        if(b.TryGetComponent(out LogicActive logicActive))
        {
            unityAction = () => logicActive.Activate(b);
        }
        else
        {
            unityAction = () =>
            {
                menu.eventListVoid[b.name](b.gameObject);
            };
        }

        //UnityEventTools.RemovePersistentListener(b.onClick, 0);
        b.onClick.AddListener(unityAction);
        //menu.eventListVoid[b.name](b.gameObject);

        DebugPrint.Log("\tboton configurando");
    }

    public static void SlotEvent(this Slot s)
    {
        var menu = MenuManager.instance;

        s.onAcceptDrop += menu.eventListVoid[s.name];

        DebugPrint.Log("\tSlot" + s.name + "configurando");
    }

    /*
    public static void Event(this Slider s)
    {
        var menu = MenuManager.instance;
        

        s.onValueChanged.RemoveAllListeners();

        //UnityEventTools.RemovePersistentListener(s.onValueChanged, 0);
        s.onValueChanged.AddListener(
            (float f) => 
            { 
                menu.eventListFloat[s.name](s.gameObject, f); 
            }
        );
        //menu.eventListFloat[s.name](s.gameObject, s.value);

        DebugPrint.Log("\tslider configurado");
    }
    */
    #endregion


}
