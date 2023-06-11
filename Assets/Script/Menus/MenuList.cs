using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MenuList : PopUp
{

    List<DoubleString> defaultOptions = new List<DoubleString>();

    /// <summary>
    ///  configura y sobrescribe el m�todo SetWindow de la Clase PopUp
    /// </summary>
    /// <param name="titulo"></param>
    /// <param name="text"></param>
    /// <param name="sprite"></param>
    /// <returns></returns>
    public override PopUp SetWindow(string titulo, string text, Sprite sprite = null)
    {
        // activa el objeto padre del componente detailsWindow lo que hace visible la ventana del men�.
        detailsWindow.transform.parent.gameObject.SetActive(true);

        // llama al m�todo SetWindow de la clase base PopUp y pasa los mismos par�metros que se recibieron en este m�todo
        return base.SetWindow(titulo, text, sprite);
    }

    /// <summary>
    /// Permite crear y configurar el men� con los botones personalizados.
    /// Toma una serie de variable DoubleString como par�metros y devuelve una instancia de MenuList.
    /// </summary>
    /// <param name="stringActions"></param>
    /// <returns></returns>
    public MenuList CreateConfigured(params DoubleString[] stringActions)
    {
        foreach (var item in stringActions)
        {
            AddButton(item.superior, item.inferior);
        }

        return this;
    }

    /// <summary>
    /// crea y configura el men� con las opciones predeterminadas guardadas en defaultOptions
    /// </summary>
    /// <returns></returns>
    public MenuList CreateDefault()
    {
        return CreateConfigured(defaultOptions.ToArray()); ;
    }

  

    private void Awake()
    {
        //Agrega a la lista eventsCalls de buttonFactory todos los componentes EventsCall encontrados en los hijos del objeto content de buttonFactory.
        //Con esto podremos acceder a los botones y eventos asociados.
        buttonFactory.eventsCalls.AddRange(buttonFactory.content.GetComponentsInChildren<EventsCall>());

        //Para cada objeto EventsCall en buttonFactory.eventsCalls, se llama al m�todo Event() del bot�n asociado.
        foreach (var item in buttonFactory.eventsCalls)
        {
            item.button.Event();

            //Se agrega un nuevo objeto DoubleString a la lista defaultOptions con el texto y el nombre del bot�n del objeto EventsCall actual.
            defaultOptions.Add(new DoubleString(item.textButton.text, item.button.name));
        }
        buttonFactory.DestroyAll();
    }


    private void OnEnable()
    {
        detailsWindow.transform.parent.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        buttonFactory.DestroyAll();
    }
}