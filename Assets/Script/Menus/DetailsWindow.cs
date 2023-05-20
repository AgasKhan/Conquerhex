using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DetailsWindow : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI myTitle;

    [SerializeField]
    TextMeshProUGUI myDescription;

    [SerializeField]
    Image previewImage;

    public Transform buttonsGrid;

    Pictionarys<string, Button> myButtons = new Pictionarys<string, Button>();

    /*
    [SerializeField]
    Button myButton;

    [SerializeField]
    TextMeshProUGUI myButtonText;
    
    [SerializeField]
    Scrollbar scrollbar;
    */

    //--------------------------------------------
    [SerializeField]
    GameObject buttonPrefab;
    //--------------------------------------------


    private void Awake()
    {
        //gameObject.SetActive(false);

        /*
        for (int i = 0; i < buttonsGrid.childCount; i++)
        {
            var aux = buttonsGrid.GetChild(i).GetComponentInChildren<Button>();
            myButtons.Add(aux.name, aux);
        }
        */

    }
    public void ShowOrHide(bool condition)
    {
        gameObject.SetActive(condition);
    }


    public void ModifyTexts(DoubleString d)
    {
        myTitle.text = d.superior;
        myDescription.text = d.inferior;

        //Utilitys.LerpInTime(() => instance.scrollbar.value, 1, 0.3f, Mathf.Lerp, (save) => { instance.scrollbar.value = save; });

    }
    public void ModifyTexts(string title, string description)
    {
        myTitle.text = title;
        myDescription.text = description;

        //Utilitys.LerpInTime(() => instance.scrollbar.value, 1, 0.3f, Mathf.Lerp, (save) => { instance.scrollbar.value = save; });

    }

    public void ActiveButtons(bool value)
    {
        for (int i = 0; i < buttonsGrid.childCount; i++)
        {
            buttonsGrid.GetChild(i).gameObject.SetActive(value);
        }

        buttonsGrid.parent.GetChild(0).gameObject.SetActive(value);
    }

    public void GenerateButtons(DoubleString[] d)
    {
        ActiveButtons(true);
        /*
        for (i = 0; i < d.Length; i++)
        {
            var aux = instance.buttoncitos[i];
            aux.cost = d[i].superior + " pts";
            aux.improvement = d[i].inferior;

            aux.ChangeColor(i + 1, d.Length);
        }
        for (; i < instance.buttoncitos.Length; i++)
        {
            instance.buttoncitos[i].gameObject.SetActive(false);
        }*/
    }


    /*
    public static void HideMyButton(bool interact)
    {
        instance.myButton.gameObject.SetActive(!interact);
    }

    public static void SetMyButton(System.Action myAction, bool interact, string text)
    {
        HideMyButton(false);

        instance.myButtonText.text = text;
        instance.myButton.interactable = interact;
        instance.myButton.onClick.RemoveAllListeners();
        instance.myButton.onClick.AddListener(() =>
        {
            myAction();
            //MenuManager.instance.ClickAccept();
        }
        );

        var size = instance.myButton.GetComponent<RectTransform>();

        TimersManager.Create(0.1f,
            () =>
            {
                size.sizeDelta = new Vector2(instance.myButtonText.GetComponent<RectTransform>().sizeDelta.x + 30, size.sizeDelta.y);
            }
        );

    }
    public static void DeactiveLevelButton()
    {
        instance.myButton.interactable = false;
    }
    */

    public void PreviewImage(bool active, Sprite sprite = null)
    {
        previewImage.gameObject.SetActive(active);

        if (sprite != null)
            previewImage.sprite = sprite;
    }

    public void SetWindow(Sprite sprite, DoubleString ds)
    {
        PreviewImage(true, sprite);
        ModifyTexts(ds);
    }

    public void SetWindow(Sprite sprite, string title, string description)
    {
        PreviewImage(true, sprite);
        ModifyTexts(title, description);
    }

    public void SetWindow(string title, string description)
    {
        ModifyTexts(title, description);
    }

    //--------------------------------------------------------
    public void CreateButtons(List<string> buttonsNames)
    {
        for (int i = 0; i < buttonsNames.Count; i++)
        {
            var aux = Instantiate(buttonPrefab, buttonsGrid);
            var button = aux.GetComponentInChildren<Button>();

            button.name = buttonsNames[i];
        }
    }

    string[] storeButtonsNames;

    public void CreateStoreButton(string itemName, string buttonName)
    {
        var father = Instantiate(buttonPrefab, buttonsGrid);
        father.name = itemName;

        var button = father.GetComponentInChildren<Button>();
        button.name = buttonName;

        var text = button.GetComponentInChildren<TextMeshProUGUI>();
        text.text = itemName;
        
    }
    //--------------------------------------------------------

    public void HideSingleButton(string buttonName)
    {
        if (myButtons.ContainsKey(buttonName))
            myButtons[buttonName].transform.parent.gameObject.SetActive(false);
        else
            Debug.Log("No se encontro el boton: " + buttonName);
    }
    public void ShowSingleButton(string buttonName)
    {
        if (myButtons.ContainsKey(buttonName))
            myButtons[buttonName].transform.parent.gameObject.SetActive(true);
        else
            Debug.Log("No se encontro el boton: " + buttonName);
    }
    public void ShowOrHideButton(string buttonName, bool state)
    {
        if (myButtons.ContainsKey(buttonName))
            myButtons[buttonName].transform.parent.gameObject.SetActive(state);
        else
            Debug.Log("No se encontro el boton: " + buttonName);
    }


    public void RefreshButton(string name)
    {
        var aux = buttonsGrid.GetChild(0).GetComponentInChildren<Button>();

        if (aux == null)
        {
            Debug.Log("No se encontro el boton a refrescar");
            return;
        }

        buttonsGrid.GetChild(0).name = name;

        if (BaseData.playerInventory.Contains(name + "Recipe") && BaseData.currentWeapon != name)
            aux.interactable = true;
        else
            aux.interactable = false;
    }

    public void EnableButton ()
    {
        buttonsGrid.GetChild(0).GetComponentInChildren<Button>().interactable = true;
    }

}
