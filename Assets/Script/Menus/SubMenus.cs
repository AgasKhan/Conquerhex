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