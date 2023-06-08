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

    public T ObtainMenu<T>() where T : MonoBehaviour
    {
        container.gameObject.SetActive(true);
        return componentMenu.SearchComponent<T>();
    }

    public void CloseMenus()
    {
        container.gameObject.SetActive(false);
    }
}


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

[System.Serializable]
public class ShowControll : CreateBodySubMenu
{
    [SerializeField]
    Sprite sprite;

    [SerializeField]
    string text;

    protected override void InternalCreate()
    {
       
        subMenu.CreateSection(1, 5);

            subMenu.AddComponent<DetailsWindow>().SetImage(sprite);


        subMenu.CreateSection(5, 8);

            subMenu.AddComponent<DetailsWindow>().SetTexts("Titulo", text);
    }


    public void Create(Sprite sprite, string text)
    {
        this.sprite = sprite;
        this.text = text;

        Create();
    }
}