using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DetailsWindow : MonoBehaviour
{
    public static DetailsWindow instance;

    [SerializeField]
    TextMeshProUGUI myTitle;

    [SerializeField]
    TextMeshProUGUI myDescription;

    [SerializeField]
    GameObject buttonsGrid;

    [SerializeField]
    Button myButton;

    [SerializeField]
    TextMeshProUGUI myButtonText;

    [SerializeField]
    Image previewImage;

    [SerializeField]
    Scrollbar scrollbar;

    //--------------------------------------------
    [SerializeField]
    Button backButton;

    [SerializeField]
    GameObject buttonPrefab;
    //--------------------------------------------


    private void Awake()
    {
        instance = this;
        //gameObject.SetActive(false);
    }
    public void ShowOrHide(bool condition)
    {
        this.gameObject.SetActive(condition);
    }


    public static void ModifyTexts(DoubleString d)
    {
        instance.myTitle.text = d.superior;
        instance.myDescription.text = d.inferior;

        //Utilitys.LerpInTime(() => instance.scrollbar.value, 1, 0.3f, Mathf.Lerp, (save) => { instance.scrollbar.value = save; });

    }
    public static void ModifyTexts(string title, string description)
    {
        instance.myTitle.text = title;
        instance.myDescription.text = description;

        //Utilitys.LerpInTime(() => instance.scrollbar.value, 1, 0.3f, Mathf.Lerp, (save) => { instance.scrollbar.value = save; });

    }

    public static void ActiveButtons(bool value)
    {
        for (int i = 0; i < instance.buttonsGrid.transform.childCount; i++)
        {
            instance.buttonsGrid.transform.GetChild(i).gameObject.SetActive(value);
        }

        instance.buttonsGrid.transform.parent.GetChild(0).gameObject.SetActive(value);
    }

    public static void GenerateButtons(DoubleString[] d)
    {
        ActiveButtons(true);
        int i;
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


    static public void PreviewImage(bool active, Sprite sprite = null)
    {
        instance.previewImage.gameObject.SetActive(active);

        if (sprite != null)
            instance.previewImage.sprite = sprite;
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
            var aux = Instantiate(buttonPrefab, buttonsGrid.transform);
            var button = aux.GetComponentInChildren<Button>();

            button.name = buttonsNames[i];
        }
    }
    //--------------------------------------------------------
}
