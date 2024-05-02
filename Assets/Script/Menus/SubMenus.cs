using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SubMenus : MonoBehaviour
{
    [Header("Elementos")]
    [SerializeField]
    public ManagerComponentMenu componentMenu;

    [SerializeField]
    public ButtonFactory navbar;

    [SerializeField]
    TextMeshProUGUI title;

    public LayoutGroup lastSectionLayoutGroup;

    public event System.Action OnClose;

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
    float fixedWidth;

    [SerializeField]
    float fixedValue;

    [SerializeField]
    float margin;

    [SerializeField]
    float subdivisions;

    [SerializeField]
    string titleString;

    int maxDivision;

    public TextAnchor lastSectionAlign
    {
        set => lastSectionLayoutGroup.childAlignment = value;
        get => lastSectionLayoutGroup.childAlignment;
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

            lastSectionLayoutGroup = lastSection.GetComponent<LayoutGroup>();
        }
    }    


    public SubMenus CreateTitle(string title)
    {
        titleString = title;

        this.title.text = titleString;

        if (navbar.eventsCalls.Count >= 1)
            this.title.text += "/" + navbar.eventsCalls[0].textButton.text;
            
        return this;
    }

    public SubMenus CreateSection(int comienzo, int final)
    {
        if(final> maxDivision)
        {
            maxDivision = final;

            var width = Width(maxDivision) + margin * 2;

            if (width < fixedWidth)
                width = fixedWidth;

            body.sizeDelta = new Vector2(width, body.sizeDelta.y);
        }

        secctions.Clear();

        lastSection = SetWidth(Instantiate(sectionPrefab, body), comienzo, final);

        return this;
    }

    public SubMenus FatherSection()
    {
        secctions.Pop();
        lastSectionLayoutGroup = lastSection.GetComponent<LayoutGroup>();
        return this;
    }

    public T AddComponent<T>() where T : MonoBehaviour
    {      
        var aux = componentMenu.CreateComponent<T>(lastSection);
        RetardedOn((_bool)=> lastSectionLayoutGroup?.SetActive(_bool));
        return aux;
    }

    public T CreateChildrenSection<T>() where T : MonoBehaviour
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

        body.sizeDelta = new Vector2(fixedWidth, body.sizeDelta.y);
    }

    public SubMenus AddNavBarButton(string text, string buttonName)
    {
        return AddNavbarButton(text, buttonName, null);
    }

    public SubMenus AddNavBarButton(string text, UnityEngine.Events.UnityAction action)
    {
        return AddNavbarButton(text, "", action);
    }

    public void TriggerOnClose()
    {
        OnClose?.Invoke();
    }

    public SubMenus RetardedOn(System.Action<bool> retardedOrder)
    {
        StartCoroutine(RetardedOnCoroutine(retardedOrder));

        return this;
    }

    public void ExitSubmenu()
    {
        //Debug.Log("Execute ExitSubMenu");
        transform.SetActiveGameObject(false);
    }

    private void Awake()
    {
        fadeMenu.alphas += FadeMenu_alphas;

        fadeMenu.Init();
    }

    private void FadeMenu_alphas(float obj)
    {
        canvasGroup.alpha = obj;
    }

    private void OnDisable()
    {
        GameManager.instance.Menu(false);
    }

    private void OnEnable()
    {
        //fadeMenu.FadeOn();

        //que numero multiplicado x mi screen da 1920

        //Screen.width * x = 1920

        //Screen.width = 1920 / x

        //1 / Screen.width =  x / 1920

        //1920/ Screen.width = x

        float relacion = (float)Screen.width / Screen.height;

        fixedWidth = relacion * width / (16 / 9f);

        fixedValue = fixedWidth / Screen.width;

        var aux = fixedWidth - margin * 7;

        subdivisions = aux / 6;

        //Debug.Log("Execute OnEnable");
        GameManager.instance.Menu(true);
    }

    

    SubMenus AddNavbarButton(string text, string buttonName, UnityEngine.Events.UnityAction action)
    {
        UnityEngine.Events.UnityAction aux = action; 

        action = () => SetText(text);

        action += aux;

        navbar.Create(text, buttonName, action);

        
        SetText(navbar.eventsCalls[0].textButton.text);
        

        return this;
    }

    RectTransform SetWidth(RectTransform rect, int comienzo, int final)
    {
        var x = comienzo * subdivisions + (comienzo + 1) * margin;

        rect.position = new Vector3(x / fixedValue, rect.position.y, rect.position.z) ;

        rect.sizeDelta = new Vector2(Width(final- comienzo), rect.sizeDelta.y);

        return rect;
    }

    float Width(int w)
    {
       return (w * subdivisions + (w - 1) * margin);
    }

    void SetText(string str)
    {
        if (titleString != "")
            title.text = titleString + "/" + str;
        else
            title.text = str;
    }

    //la magia potagia, para refrezcar lo creado
    IEnumerator RetardedOnCoroutine(System.Action<bool> retardedOrder)
    {
        yield return null;
        yield return null;

        retardedOrder?.Invoke(false);
        retardedOrder?.Invoke(true);
        /*
        yield return new WaitForEndOfFrame();

        gameObject.SetActive(false);
        
        yield return null;

        gameObject.SetActive(true);

        yield return new WaitForEndOfFrame();

        retardedOrder?.Invoke(false);
        retardedOrder?.Invoke(true);
        */
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


/*
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
    float widthScale;

    [SerializeField]
    float widthSimple;

    [SerializeField]
    float margin;

    [SerializeField]
    float subdivisions;

    int maxDivision;

    System.Action postConstruct;

    public TextAnchor lastSectionAlign
    {
        set => postConstruct+= ()=> lastSectionLayputGroup.childAlignment = value;
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
        fadeMenu.alphas += FadeMenu_alphas;

        fadeMenu.Init();
    }

    private void FadeMenu_alphas(float obj)
    {
        canvasGroup.alpha = obj;
    }

    private void OnEnable()
    {
        StartCoroutine(RetardedAwake());
        fadeMenu.FadeOn();
    }

    public SubMenus CreateSection(int comienzo, int final)
    {
        postConstruct += () => _CreateSection(comienzo, final);
        return this;
    }

    public SubMenus FatherSection()
    {
        postConstruct += () => _FatherSection();
        return this;
    }

    public T AddComponent<T>() where T : MonoBehaviour
    {
        var aux = componentMenu.CreateComponent<T>(body);

        postConstruct += () => _AddComponent(aux.transform);

        return aux;
    }

    public T CreateChildrenSection<T>() where T : UnityEngine.EventSystems.UIBehaviour
    {
        var result = componentMenu.CreateComponent<T>(body);

        lastSection = result.GetComponent<ContentRectTransform>().rectTransform;

        postConstruct += () => _AddComponent(result.transform);

        return result;
    }

    public void ClearBody()
    {
        //postConstruct += () => _ClearBody();
    }

    void _CreateSection(int comienzo, int final)
    {
        if(final> maxDivision)
        {
            maxDivision = final;

            var width = Width(maxDivision) + margin * 2;

            if (width < this.width)
                width = this.width;

            body.sizeDelta = new Vector2(width, body.sizeDelta.y);
        }

        secctions.Clear();

        lastSection = SetWidth(Instantiate(sectionPrefab, body), comienzo, final);
    }

    void _FatherSection()
    {
        secctions.Pop();
        lastSectionLayputGroup = lastSection.GetComponent<LayoutGroup>();
    }

    void _AddComponent(Transform tr)
    {
        tr.SetParent(lastSection);
        StartCoroutine(RetardedOn(lastSection.gameObject));
    }

    void _ClearBody()
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

    IEnumerator RetardedAwake()
    {
        yield return new WaitForEndOfFrame();

        float realacion = (float)Screen.width / Screen.height;

        widthScale = realacion * width / (16 / 9f);

        widthSimple = (Screen.height * 1920 / 1080);

       
        //16:9
        //4:3
        //16:10

        //16:9 --- 1920
        //16:10 ---

        //si para 1080 --- 1920
        //para screen.height ------ screen.height*1920/1080

        var aux = width - margin * 7;

        subdivisions = aux / 6;

        postConstruct?.Invoke();

        postConstruct = null;
    }
}
 
 
 */