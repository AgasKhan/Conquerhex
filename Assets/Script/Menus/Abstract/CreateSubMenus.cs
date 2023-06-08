using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Version por objetos para crear menus
/// </summary>
public abstract class CreateSubMenu : Init
{
    static SubMenus staticSubMenu => MenuManager.instance.modulesMenu.ObtainMenu<SubMenus>();

    static public void CreateBody(System.Action<SubMenus> action)
    {
        staticSubMenu.ClearBody();

        Create(action);
    }

    static public void CreateNavBar(System.Action<SubMenus> action)
    {
        staticSubMenu.navbar.DestroyAll();

        Create(action);
    }

    static void Create(System.Action<SubMenus> action)
    {
        staticSubMenu.SetActiveGameObject(true);
        action(staticSubMenu);
    }

    [SerializeField]
    protected SubMenus subMenu;

    public virtual void Create()
    {
        subMenu.SetActiveGameObject(true);
        InternalCreate();
    }
    public void Init(params object[] param)
    {
        if (subMenu == null)
            subMenu = MenuManager.instance.modulesMenu.ObtainMenu<SubMenus>();
    }

    protected abstract void InternalCreate();


}

public abstract class CreateBodySubMenu : CreateSubMenu
{
    public override void Create()
    {
        subMenu.ClearBody();
        base.Create();
    }
}

public abstract class CreateNavBarSubMenu : CreateSubMenu
{
    public override void Create()
    {
        subMenu.navbar.DestroyAll();
        base.Create();
    }
}
