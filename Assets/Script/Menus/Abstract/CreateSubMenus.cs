using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Version por objetos para crear menus
/// </summary>
public abstract class CreateSubMenu : Init
{
    static SubMenus staticSubMenu => MenuManager.instance.modulesMenu.ObtainMenu<SubMenus>();

    /// <summary>
    ///  Borra el contenido del cuerpo de SubMenus y luego llama al m�todo Create con una acci�n proporcionada como argumento.
    /// </summary>
    /// <param name="action"></param>
    static public void CreateBody(System.Action<SubMenus> action)
    {
        staticSubMenu.ClearBody();

        Create(action);
    }

    /// <summary>
    ///  Elimina todos los elementos de la barra de navegaci�n del men� SubMenus y luego llama al m�todo Create con una acci�n proporcionada como argumento.
    /// </summary>
    /// <param name="action"></param>
    static public void CreateNavBar(System.Action<SubMenus> action)
    {
        staticSubMenu.navbar.DestroyAll();

        Create(action);
    }

    /// <summary>
    /// Establece el objeto del men� SubMenus como activo y luego ejecuta la acci�n proporcionada en el men�.
    /// </summary>
    /// <param name="action"></param>
    static void Create(System.Action<SubMenus> action)
    {
        staticSubMenu.CreateTitle("");
        staticSubMenu.SetActiveGameObject(true);
        action(staticSubMenu);
    }


    
    /////////////////////////////////////////////////
    ///NO ESTATICA
    ////////////////////////////////////////////////
    


    [SerializeField]
    protected SubMenus subMenu;

    /// <summary>
    /// establece el objeto del men� subMenu como activo y luego llama al m�todo InternalCreate.
    /// </summary>
    public virtual void Create()
    {
        subMenu.CreateTitle("");
        subMenu.SetActiveGameObject(true);
        InternalCreate();
    }

    /// <summary>
    /// Init inicializa la instancia del men� subMenu si no ha sido asignada previamente.
    /// </summary>
    /// <param name="param"></param>
    public void Init(params object[] param)
    {
        if (subMenu == null)
            subMenu = MenuManager.instance.modulesMenu.ObtainMenu<SubMenus>();
    }
    /// <summary>
    /// Define la l�gica espec�fica para crear el contenido del men�.
    /// </summary>
    protected abstract void InternalCreate();
}

public abstract class CreateBodySubMenu : CreateSubMenu
{
    /// <summary>
    /// Sobrescribe el m�todo Create de la clase base CreateSubMenu. Limpia el contenido del cuerpo del men� subMenu y luego llama al m�todo Create de la clase base.
    /// </summary>
    public override void Create()
    {
        subMenu.ClearBody();
        base.Create();
    }
}

public abstract class CreateNavBarSubMenu : CreateSubMenu
{
    /// <summary>
    /// Sobrescribe el m�todo Create de la clase base CreateSubMenu. Elimina todos los elementos de la barra de navegaci�n del men� subMenu y luego llama al m�todo Create de la clase base.
    /// </summary>
    public override void Create()
    {
        subMenu.navbar.DestroyAll();
        base.Create();
    }
}
