using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SubMenus : MonoBehaviour
{

    [Header("Elementos")]
    [SerializeField]
    ManagerComponentMenu componentMenu;

    [SerializeField]
    public ButtonFactory navbar;

    [SerializeField]
    TextMeshProUGUI title;

    public LayoutGroup lastSectionLayputGroup;

    [SerializeField]
    CanvasGroup canvasGroup;

    [Header("Prefabs")]

    [SerializeField]
    RectTransform sectionPrefab;

    [SerializeField]
    RectTransform body;

    [Header("Configuracion")]

    [SerializeField]
    FadeOnOff fadeMenu;

    [SerializeField]
    float width;

    [SerializeField]
    float margin;

    [SerializeField]
    float subdivisions;

    int maxDivision;

    public TextAnchor lastSectionAlign
    {
        set => lastSectionLayputGroup.childAlignment = value;
        get => lastSectionLayputGroup.childAlignment;
    }

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

            lastSectionLayputGroup = lastSection.GetComponent<LayoutGroup>();
        }
    }    

    private void Awake()
    {
        var aux = width - margin * 7;

        subdivisions = aux / 6;

        fadeMenu.alphas += FadeMenu_alphas;

        fadeMenu.Init();
    }

    private void FadeMenu_alphas(float obj)
    {
        //canvasGroup.alpha = obj;
    }

    private void OnEnable()
    {
        fadeMenu.FadeOn();
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
        lastSectionLayputGroup = lastSection.GetComponent<LayoutGroup>();
        return this;
    }

    public T AddComponent<T>() where T : MonoBehaviour
    {           
        
        var aux = componentMenu.CreateComponent<T>(lastSection);
        StartCoroutine(RetardedOn(lastSection.gameObject));
        return aux;
    }

    public T CreateChildrenSection<T>() where T : UnityEngine.EventSystems.UIBehaviour
    {
        var result = componentMenu.CreateComponent<T>(lastSection);

        lastSection = result.GetComponent<ContentRectTransform>().rectTransform;

        return result;
    }

    public void ClearBody()
    {
        for (int i = 0; i < body.childCount; i++)
        {
            Destroy(body.GetChild(i).gameObject);
        }

        maxDivision = 0;

        body.sizeDelta = new Vector2(width, body.sizeDelta.y);
    }

    public SubMenus AddNavBarButton(string text, string buttonName)
    {
        return AddNavbarButton(text, buttonName, null);
    }

    public SubMenus AddNavBarButton(string text, UnityEngine.Events.UnityAction action)
    {
        return AddNavbarButton(text, "", null);
    }

    SubMenus AddNavbarButton(string text, string buttonName, UnityEngine.Events.UnityAction action)
    {
        action += () => title.text = text;

        navbar.Create(text, buttonName, action);

        if(navbar.eventsCalls.Count == 1)
            title.text = text;

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

    //la magia potagia, para refrezcar lo creado
    IEnumerator RetardedOn(GameObject gameObject)
    {
        yield return new WaitForEndOfFrame();

        gameObject.SetActive(false);

        yield return null;

        gameObject.SetActive(true);
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
