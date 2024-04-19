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

    /// <summary>
    /// Configura y muestra un t�tulo, texto y una imagen opcional que se mostrar� en la ventana
    /// </summary>
    /// <param name="titulo"></param>
    /// <param name="text"></param>
    /// <param name="sprite"></param>
    /// <returns></returns>
    public virtual PopUp SetWindow(string titulo, string text, Sprite sprite = null)
    {
        detailsWindow.SetTexts(titulo, text).SetImage(sprite);
        buttonFactory.content.gameObject.SetActive(false);
        return this;
    }

    /// <summary>
    /// Agrega un bot�n al PopUp y toma como par�metros el texto del bot�n y el nombre del bot�n
    /// </summary>
    /// <param name="text"></param>
    /// <param name="buttonName"></param>
    /// <returns></returns>
    public PopUp AddButton(string text, string buttonName)
    {
        buttonFactory.content.gameObject.SetActive(true);
        buttonFactory.Create(text, buttonName, null);
        return this;
    }

    /// <summary>
    /// Agrega un bot�n al PopUp y toma como par�metros el texto del bot�n y una acci�n cuando se ejecute el bot�n
    /// </summary>
    /// <param name="text"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public PopUp AddButton(string text, UnityEngine.Events.UnityAction action)
    {
        buttonFactory.content.gameObject.SetActive(true);
        buttonFactory.Create(text, "", action);
        return this;
    }

    private void OnEnable()
    {
        GameManager.instance.Menu(true);
    }

    private void OnDisable()
    {
        buttonFactory.DestroyAll();
    }
}
