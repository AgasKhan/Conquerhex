using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubMenus : MonoBehaviour
{
    
    [Header("Prefabs")]

    [SerializeField]
    RectTransform sectionPrefab;

    [SerializeField]
    ManagerComponentMenu componentMenu;

    [Header("Elementos")]
    [SerializeField]
    public ButtonFactory navbar;

    [SerializeField]
    RectTransform body;

    [SerializeField]
    float width;

    [SerializeField]
    float margin;

    [SerializeField]
    float subdivisions;

    int maxDivision;

    Stack<RectTransform> secctions = new Stack<RectTransform>();

    RectTransform lastSection
    {
        get
        {
            if (secctions.TryPeek(out var result))
                return result;
            else
                return null;
        }
        set
        {
            secctions.Push(value);
        }
    }    

    private void Awake()
    {
        var aux = width - margin * 7;

        subdivisions = aux / 6;

        CreateSection(0, 4);

            AddComponent<DetailsWindow>().SetTexts("Hola", "Muuundo");

        CreateSection(4, 8);

            CreateChildrenSection<HorizontalLayoutGroup>(); //de ejemplo

                AddComponent<DetailsWindow>().SetTexts("Hola" + " otra cosa", "SEGUNDA seccion").SetAlignment(TMPro.TextAlignmentOptions.Justified, TMPro.TextAlignmentOptions.TopRight);

                AddComponent<DetailsWindow>().SetTexts("Segunda parte", "DERECHA").SetAlignment(TMPro.TextAlignmentOptions.Justified, TMPro.TextAlignmentOptions.TopRight);

            FatherSection();

            AddComponent<DetailsWindow>().SetTexts("Hola", "SEGUNDA seccion segundo componente");
    }

    public SubMenus CreateSection(int comienzo, int final)
    {
        if(final> maxDivision)
        {
            maxDivision = final;

            var width = Width(maxDivision) + margin * 2;

            if (width < Screen.width)
                width = Screen.width;

            body.sizeDelta = new Vector2(width, body.sizeDelta.y);
        }

        secctions.Clear();

        lastSection = SetWidth(Instantiate(sectionPrefab, body), comienzo, final);

        return this;
    }

    public SubMenus FatherSection()
    {
        secctions.Pop();

        return this;
    }

    public T AddComponent<T>() where T : MonoBehaviour
    {
        return componentMenu.CreateComponent<T>(lastSection);
    }

    public T CreateChildrenSection<T>() where T : LayoutGroup
    {
        var result = componentMenu.CreateComponent<T>(lastSection);

        lastSection = result.GetComponent<RectTransform>();

        return result;
    }

    public void ClearBody()
    {
        for (int i = 0; i < body.childCount; i++)
        {
            Destroy(body.GetChild(i).gameObject);
        }

        body.sizeDelta = new Vector2(width, body.sizeDelta.y);
    }

    public SubMenus AddNavBarButton(string text, string buttonName)
    {
        navbar.Create(text, buttonName, null);
        return this;
    }

    public SubMenus AddNavBarButton(string text, UnityEngine.Events.UnityAction action)
    {
        navbar.Create(text, "", action);
        return this;
    }

    RectTransform SetWidth(RectTransform rect, int comienzo, int final)
    {
        var x = comienzo * subdivisions + (comienzo + 1) * margin;

        rect.position = new Vector3(x, rect.position.y, rect.position.z);

        rect.sizeDelta = new Vector2(Width(final- comienzo), rect.sizeDelta.y);

        return rect;
    }

    float Width(int w)
    {
       return w * subdivisions + (w - 1) * margin;
    }
}

[System.Serializable]
public class ManagerComponentMenu
{
    [SerializeField]
    Component[] components;

    public T SearchComponent<T>() where T : Component
    {
        foreach (var item in components)
        {
            if (item.GetType() == typeof(T))
            {
                return (T)item;
            }
        }
        return default;
    }

    public T CreateComponent<T>(Transform parent) where T : Component
    {
        return Object.Instantiate(SearchComponent<T>(), parent);
    }
}

[System.Serializable]
public class ManagerModulesMenu
{
    [SerializeField]
    public RectTransform container;

    [SerializeField]
    ManagerComponentMenu componentMenu;

    public T ObtainMenu<T>(bool view) where T : MonoBehaviour
    {
        container.gameObject.SetActive(true);
        return componentMenu.SearchComponent<T>().SetActiveGameObject(view);
    }

    public void CloseMenus()
    {
        container.gameObject.SetActive(false);
    }
}

/// <summary>
/// Version estatica para crear menus
/// </summary>
public static class StaticCreateSubMenu
{
    static SubMenus subMenus => MenuManager.instance.modulesMenu.ObtainMenu<SubMenus>(false);

    static public void CreateBody(System.Action<SubMenus> action)
    {
        subMenus.ClearBody();

        Create(action);
    }

    static public void CreateNavBar(System.Action<SubMenus> action)
    {
        subMenus.navbar.DestroyAll();

        Create(action);
    }

    static void Create(System.Action<SubMenus> action)
    {
        //subMenu.ClearBody();

        action(subMenus);

        subMenus.SetActiveGameObject(true);
    }
}



/// <summary>
/// Version por objetos para crear menus
/// </summary>
public abstract class CreateSubMenu
{
    protected SubMenus subMenu;

    public System.Action<SubMenus> action;

    public CreateSubMenu(System.Action<SubMenus> action)
    {
        subMenu = MenuManager.instance.modulesMenu.ObtainMenu<SubMenus>(false);

        this.action = action;
    }

    public virtual void Create()
    {
        //subMenu.ClearBody();

        action(subMenu);

        subMenu.SetActiveGameObject(true);
    }
}

public class CreateBodySubMenu : CreateSubMenu
{
    public CreateBodySubMenu(System.Action<SubMenus> action) : base(action)
    {
    }

    public override void Create()
    {
        subMenu.ClearBody();
        base.Create();
    }
}

public class CreateNavBarSubMenu : CreateSubMenu
{
    public CreateNavBarSubMenu(System.Action<SubMenus> action) : base(action)
    {
    }

    public override void Create()
    {
        subMenu.navbar.DestroyAll();
        base.Create();
    }
}