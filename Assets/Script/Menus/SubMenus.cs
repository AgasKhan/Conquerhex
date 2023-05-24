using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubMenus : MonoBehaviour
{
    [SerializeField]
    GameObject navbar;

    [SerializeField]
    MonoBehaviour[] prefabs;

    [SerializeField]
    RectTransform sectionPrefab;

    [SerializeField]
    RectTransform horizontalContainer;

    [SerializeField]
    float margin;

    [SerializeField]
    float subdivisions;

    private void Awake()
    {
        var aux = Screen.width - margin*7;

        subdivisions = aux / 6;

        CreateSection(0,4);

        CreateSection(4, 8);
    }

    public T CreateModule<T>(Transform parent) where T : MonoBehaviour
    {
        foreach (var item in prefabs)
        {
            if(item is T)
            {
                return Instantiate((T)item, parent);
            }
        }
        return default;
    }

    public RectTransform CreateSection(int comienzo, int final)
    {
        return SetWidth(Instantiate(sectionPrefab, horizontalContainer), comienzo, final);
    }

    public RectTransform SetWidth(RectTransform rect, int comienzo, int final)
    {
        var x = comienzo * subdivisions + (comienzo + 1) * margin;

        var width = (final - comienzo) * subdivisions + (final - comienzo - 1) * margin;

        rect.position = new Vector3(x, rect.position.y, rect.position.z);

        rect.sizeDelta = new Vector2(width, rect.sizeDelta.y);

        return rect;
    }

}
