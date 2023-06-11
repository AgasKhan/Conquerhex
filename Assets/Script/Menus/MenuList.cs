using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MenuList : PopUp
{

    List<DoubleString> defaultOptions = new List<DoubleString>();

    /// <summary>
    ///  configura y sobrescribe el método SetWindow de la Clase PopUp
    /// </summary>
    /// <param name="titulo"></param>
    /// <param name="text"></param>
    /// <param name="sprite"></param>
    /// <returns></returns>
    public override PopUp SetWindow(string titulo, string text, Sprite sprite = null)
    {
        // activa el objeto padre del componente detailsWindow lo que hace visible la ventana del menú.
        detailsWindow.transform.parent.gameObject.SetActive(true);

        // llama al método SetWindow de la clase base PopUp y pasa los mismos parámetros que se recibieron en este método
        return base.SetWindow(titulo, text, sprite);
    }

    /// <summary>
    /// Permite crear y configurar el menú con los botones personalizados.
    /// Toma una serie de variable DoubleString como parámetros y devuelve una instancia de MenuList.
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
    /// crea y configura el menú con las opciones predeterminadas guardadas en defaultOptions
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

        //Para cada objeto EventsCall en buttonFactory.eventsCalls, se llama al método Event() del botón asociado.
        foreach (var item in buttonFactory.eventsCalls)
        {
            item.button.Event();

            //Se agrega un nuevo objeto DoubleString a la lista defaultOptions con el texto y el nombre del botón del objeto EventsCall actual.
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