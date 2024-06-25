using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PopUp : MonoBehaviour
{
    [SerializeField]
    protected DetailsWindow detailsWindow;

    [SerializeField]
    protected ButtonFactory buttonFactory;

    protected RectTransform rectTransform;

    /// <summary>
    /// Configura y muestra un título, texto y una imagen opcional que se mostrará en la ventana
    /// </summary>
    /// <param name="titulo"></param>
    /// <param name="text"></param>
    /// <param name="sprite"></param>
    /// <returns></returns>
    public virtual PopUp SetWindow(string titulo, string text, Sprite sprite = null)
    {
        detailsWindow.SetTexts(titulo, text).SetImage(sprite);
        buttonFactory.content.gameObject.SetActive(false);

        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);

        return this;
    }

    /// <summary>
    /// Agrega un botón al PopUp y toma como parámetros el texto del botón y el nombre del botón
    /// </summary>
    /// <param name="text"></param>
    /// <param name="buttonName"></param>
    /// <returns></returns>
    public PopUp AddButton(string text, string buttonName)
    {
        buttonFactory.content.gameObject.SetActive(true);
        buttonFactory.Create(text, buttonName, null);

        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);

        return this;
    }

    /// <summary>
    /// Agrega un botón al PopUp y toma como parámetros el texto del botón y una acción cuando se ejecute el botón
    /// </summary>
    /// <param name="text"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public PopUp AddButton(string text, UnityEngine.Events.UnityAction action)
    {
        buttonFactory.content.gameObject.SetActive(true);
        buttonFactory.Create(text, "", action);

        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);

        return this;
    }

    private void OnDisable()
    {
        buttonFactory.DestroyAll();
    }

    private void OnEnable()
    {
        GameManager.instance.Menu(true);
    }

    private void Awake()
    {
        rectTransform = (RectTransform)transform;
    }
}
