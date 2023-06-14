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
    ///  Borra el contenido del cuerpo de SubMenus y luego llama al método Create con una acción proporcionada como argumento.
    /// </summary>
    /// <param name="action"></param>
    static public void CreateBody(System.Action<SubMenus> action)
    {
        staticSubMenu.ClearBody();

        Create(action);
    }

    /// <summary>
    ///  Elimina todos los elementos de la barra de navegación del menú SubMenus y luego llama al método Create con una acción proporcionada como argumento.
    /// </summary>
    /// <param name="action"></param>
    static public void CreateNavBar(System.Action<SubMenus> action)
    {
        staticSubMenu.navbar.DestroyAll();

        Create(action);
    }

    /// <summary>
    /// Establece el objeto del menú SubMenus como activo y luego ejecuta la acción proporcionada en el menú.
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
    /// establece el objeto del menú subMenu como activo y luego llama al método InternalCreate.
    /// </summary>
    public virtual void Create()
    {
        subMenu.CreateTitle("");
        subMenu.SetActiveGameObject(true);
        InternalCreate();
    }

    /// <summary>
    /// Init inicializa la instancia del menú subMenu si no ha sido asignada previamente.
    /// </summary>
    /// <param name="param"></param>
    public void Init(params object[] param)
    {
        if (subMenu == null)
            subMenu = MenuManager.instance.modulesMenu.ObtainMenu<SubMenus>();
    }
    /// <summary>
    /// Define la lógica específica para crear el contenido del menú.
    /// </summary>
    protected abstract void InternalCreate();
}

public abstract class CreateBodySubMenu : CreateSubMenu
{
    /// <summary>
    /// Sobrescribe el método Create de la clase base CreateSubMenu. Limpia el contenido del cuerpo del menú subMenu y luego llama al método Create de la clase base.
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
    /// Sobrescribe el método Create de la clase base CreateSubMenu. Elimina todos los elementos de la barra de navegación del menú subMenu y luego llama al método Create de la clase base.
    /// </summary>
    public override void Create()
    {
        subMenu.navbar.DestroyAll();
        base.Create();
    }
}
