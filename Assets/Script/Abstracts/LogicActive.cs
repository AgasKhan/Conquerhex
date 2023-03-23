using System.Collections;
using System.Collections.Generic;
using UnityEngine;
abstract public class LogicActive : MonoBehaviour
{

    void ErrorActivate<T>(params T[] genericParams)
    {
        string warning = "Funcion activate no sobre escrita\nTipo: "+typeof(T)+"\nParametros:";

        foreach (var item in genericParams)
        {
            warning += "\n"+item;
        }

        Debug.LogWarning(warning);
    }



    /// <summary>
    /// Funcion por defecto con la idea de simplificar todos aquellos scripts que se centran en ejecutar una funcion
    /// </summary>
    virtual public void Activate()
    {
        ErrorActivate<string>();
    }


    /// <summary>
    /// Funcion por defecto con la idea de simplificar todos aquellos scripts que se centran en ejecutar una funcion
    /// </summary>
    /// <param name="genericParams">Params del tipo definido</param>
    virtual public void Activate(params float[] genericParams)
    {
        ErrorActivate<float>(genericParams);
    }


    /// <summary>
    /// Funcion por defecto con la idea de simplificar todos aquellos scripts que se centran en ejecutar una funcion
    /// </summary>
    /// <param name="genericParams">Params del tipo definido</param>
    virtual public void Activate(params bool[] genericParams)
    {
        ErrorActivate<bool>(genericParams);
    }

    /// <summary>
    /// Funcion por defecto con la idea de simplificar todos aquellos scripts que se centran en ejecutar una funcion
    /// </summary>
    /// <param name="genericParams">Params del tipo definido</param>
    virtual public void Activate(params string[] genericParams)
    {
        ErrorActivate<string>(genericParams);
    }
}



