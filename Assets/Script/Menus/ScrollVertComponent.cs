using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollVertComponent : ContentRectTransform
{
    public List<Item> listItems = new List<Item>();

    public Transform content;

    ButtonA prefab;

    public void GenerateButtonsList()
    {
        for (int i = 0; i < listItems.Count; i++)
        {
            foreach (var item in listItems)
            {
                item.GetAmounts(out int actual, out int max);
                prefab.SetButtonA(item.nameDisplay, item.image, actual + " / " + max, null);

                Instantiate(prefab, content);
            }

        }
    }

    public Component GenerateComp(Component comp)
    {
        return Instantiate(comp, content);
    }

    public EventsCall GenerateButton(EventsCall obj)
    {
        return Instantiate(obj, content);
    }
}
