using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubMenus : MonoBehaviour
{
    [SerializeField]
    GameObject navbar;

    [SerializeField]
    RectTransform sectionPrefab;

    [SerializeField]
    ManagerComponentMenu componentMenu;

    [SerializeField]
    float margin;

    [SerializeField]
    float subdivisions;

    int maxDivision;

    private void Awake()
    {
        var aux = Screen.width - margin*7;

        subdivisions = aux / 6;

        CreateSection(0,4);

        CreateSection(4, 8);
    }

    public RectTransform CreateSection(int comienzo, int final)
    {
        if(final> maxDivision)
        {
            maxDivision = final;

            componentMenu.container.sizeDelta = new Vector2(Width(maxDivision) + margin*2, componentMenu.container.sizeDelta.y);
        }
            

        return SetWidth(Instantiate(sectionPrefab, componentMenu.container), comienzo, final);
    }

    public RectTransform SetWidth(RectTransform rect, int comienzo, int final)
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
    public RectTransform container;

    [SerializeField]
    MonoBehaviour[] components;

    public T SearchComponent<T>() where T : MonoBehaviour
    {
        foreach (var item in components)
        {
            if (item is T)
            {
                return (T)item;
            }
        }
        return default;
    }

    public T CreateComponent<T>(Transform parent) where T : MonoBehaviour
    {
        return Object.Instantiate(SearchComponent<T>(), parent);
    }
}

[System.Serializable]
public class ManagerModulesMenu
{
    [SerializeField]
    ManagerComponentMenu componentMenu;

    public T ObtainMenu<T>(bool view) where T : MonoBehaviour
    {
        componentMenu.container.gameObject.SetActive(true);
        return componentMenu.SearchComponent<T>().SetActiveGameObject(view);
    }

    public void CloseMenus()
    {
        componentMenu.container.gameObject.SetActive(false);
    }
}