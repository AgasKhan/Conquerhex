using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class DisplayItem : LogicActive<UnityEngine.UI.Button>
{
    public IShowDetails myItem;

    public string specialAction = "";

    [HideInInspector]
    public DetailsWindow myDetailWindow;

    static Pictionarys<string, UnityEngine.UI.Image> alphaButtons = new Pictionarys<string, UnityEngine.UI.Image>();

    private void OnEnable()
    {
        SpecialButton(gameObject, "Controls");
    }

    void GetVariables()
    {
        myItem = GetScriptObject(transform.parent.name);
        myDetailWindow = Manager<DetailsWindow>.pic[transform.name];
    }

    IShowDetails GetScriptObject(string ObjectName)
    {
        if (Manager<ShowDetails>.pic.ContainsKey(ObjectName))
            return Manager<ShowDetails>.pic[transform.parent.name];
        else
            return Manager<ItemBase>.pic[transform.parent.name];
    }

    protected override void InternalActivate(params Button[] specificParam)
    {
        GetVariables();

        if (myItem == null || myDetailWindow == null)
        {
            Debug.LogWarning("No se encontro un parametro necesario");
            return;
        }

        myDetailWindow.SetTexts(myItem.nameDisplay, myItem.GetDetails().ToString("\n", "\n \n")).SetImage(myItem.image);

        SpecialButton(gameObject);
    }


    Image SpecialButton(GameObject g, string name = "")
    {

        if (!alphaButtons.TryGetValue(g.transform.parent.name, out var image))
        {
            image = g.GetComponent<UnityEngine.UI.Image>();

            alphaButtons.Add(g.transform.parent.name, image);
        }

        foreach (var button in alphaButtons)
        {

            if (button.value.transform.parent.name == name)
            {
                continue;
            }

            var tempColor = button.value.color;
            tempColor.a = 1f;

            button.value.color = tempColor;
        }

        if (name == "")
        {
            var tempColor2 = image.color;
            tempColor2.a = 0.5f;
            image.color = tempColor2;
        }

        return image;
    }


    private void OnDestroy()
    {
        alphaButtons.Clear();
    }
}