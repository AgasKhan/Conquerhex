using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class LogicActive : MonoBehaviour
{
    void ErrorActivate<T>(params T[] genericParams)
    {
        string warning = "Funcion activate no sobre escrita\nTipo: "+nameof(T)+"\nParametros:";

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
    /*
    virtual public void Activate<C>(params C[] genericParams)
    {
        ErrorActivate(genericParams);
    }
    */
}

public abstract class LogicActive<T> : LogicActive
{
    public abstract void Activate(T genericParams);
    
    //protected abstract void InternalActivate(params T[] specificParam);
}

public abstract class InteractAction : LogicActive
{
    [HideInInspector]
    public InteractEntityComponent interactComp;
    protected CreateSubMenu subMenu;

    public virtual void InteractInit(InteractEntityComponent _interactComp)
    {
        interactComp = _interactComp;
    }

    public virtual void ShowMenu(Character character)//Recibe el customer
    {
        subMenu.Create(character);
    }
}
public abstract class InteractAction<T> : InteractAction
{
    public abstract void Activate(T genericParams);
}
