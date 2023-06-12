using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListScrollVert : MonoBehaviour
{
    [SerializeField]
    List<ButtonA> ListButtonsA = new List<ButtonA>();

    [SerializeField]
    RectTransform content;



    public ListScrollVert CreateConfigured(ButtonA buttonA)
    {
        AddButtonA(buttonA);
        return this;
    }

    public ListScrollVert CreateDefault()
    {
        foreach (var buttonA in ListButtonsA)
        {
            AddButtonA(buttonA);
        }
        return this;
    }

    public void AddButtonA(ButtonA buttonA)
    {
        ButtonA newButtonA = Instantiate(buttonA, content);
        ListButtonsA.Add(newButtonA);
    }



}
