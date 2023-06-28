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

    
    public static void Event(this Button b, bool remove = true)
    {
        var menu = MenuManager.instance;
        if(remove)
            b.onClick.RemoveAllListeners();

        UnityEngine.Events.UnityAction unityAction;

        if(b.TryGetComponent(out DisplayItem logicActive))
        {
            if (logicActive.specialAction != "")
            {
                unityAction = () =>
                {
                    logicActive.Activate(b);
                    menu.eventListVoid[logicActive.specialAction](b.gameObject);
                };
            }
            else
            {
                unityAction = () => logicActive.Activate(b);
            }
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

    static public string ToString(this Damage[] damages, string glue, string reglon)
    {
        var aux = "";
        foreach (var item in damages)
        {
            aux += item.typeInstance.GetType().Name + glue + item.amount + reglon;
        }

        return aux;
    }

    static public void AddOrInsert<T>(this List<T> list, T toAdd ,int insert)
    {
        if(insert<0 || insert>= list.Count)
        {
            list.Add(toAdd);
        }
        else
        {
            list.Insert(insert, toAdd);
        }
    }

    /// <summary>
    /// Retorna una copia del color con el alpha cambiado
    /// </summary>
    /// <param name="color"></param>
    /// <param name="alpha"></param>
    /// <returns></returns>
    static public Color ChangeAlphaCopy(this Color color, float alpha)
    {
        return new Color(color.r, color.g, color.b, alpha);
    }


    static public T SetActiveGameObject<T>(this T component, bool active) where T : Component
    {
        component.gameObject.SetActive(active);
        return component;
    }

    static public T SetActive<T>(this T mono, bool active) where T : MonoBehaviour
    {
        mono.enabled = active;
        return mono;
    }

    static public T RandomPic<T>(this Pictionarys<T,int> pictionaries, int levelMultiply = 0)
    {
        float acumTotal = 0;

        float acumPercentage = 0;

        float rng = Random.Range(0, 1f);

        levelMultiply = Mathf.Clamp(levelMultiply, 0, 1000);

        T lastItem = default;

        foreach (var item in pictionaries)
        {
            acumTotal += item.value;
        }

        float newAcumTotal = 0;

        if (levelMultiply != 0)
            foreach (var item in pictionaries)
            {
                newAcumTotal += item.value + Mathf.Pow(1.5f, (1 + (1 - (item.value / acumTotal)) * levelMultiply));
            }
        else
            newAcumTotal = acumTotal;

        foreach (var item in pictionaries)
        {
            acumPercentage += item.value / newAcumTotal;

            if (levelMultiply != 0)
                acumPercentage += (Mathf.Pow(1.5f, (1 + (1 - (item.value / acumTotal)) * levelMultiply))) / newAcumTotal;

            if (rng<= acumPercentage)
            {
                return item.key;
            }

            lastItem = item.key;
        }

        return lastItem;
    }
}
