using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EventsCall : MonoBehaviour
{
    [SerializeField]
    public Image image;

    public TextMeshProUGUI textButton;

    public Button button;

    public bool empty=true;

    public FadeOnOff fadeMenu;



    public event UnityEngine.Events.UnityAction listeners
    {
        add
        {
            button.onClick.AddListener(value);
        }

        remove
        {
            button.onClick.RemoveListener(value);
        }
    }

    private void Awake()
    {
        fadeMenu.alphas += Text_alphas;
        fadeMenu.Init();
    }

    private void Text_alphas(float obj)
    {
        textButton.color = textButton.color.ChangeAlphaCopy(obj);
        image.color = image.color.ChangeAlphaCopy(obj);
    }


    private void OnEnable()
    {
        fadeMenu.FadeOn();
    }

    /// <summary>
    /// Imprime un mensaje en la consola que muestra el nombre del objeto
    /// </summary>
    /// <param name="g"></param>
    public void Event(GameObject g)
    {
        print("\tAccediendo: " + g.name);

        /*
        if(button == null)
            if (!g.TryGetComponent(out button))
            {
                DebugPrint.Warning("No contiene uno de los componentes esperados: " + g.name);
                return; 
            }
        */

        button.onClick.SetPersistentListenerState(0, UnityEngine.Events.UnityEventCallState.Off);

        button.Event(empty);
        //menu.eventListVoid[b.name](b.gameObject);
        button.onClick.Invoke();

        //menu.eventListVoid.Remove(b.name);
    }


    public EventsCall Clone(string text, UnityEngine.Events.UnityAction action, string buttonName, Transform content)
    {
        //Crea una instancia de EventsCall usando Instantiate y establece su posici�n en content
        var aux = Instantiate(this, content);

        //asigno el texto al campo textButton.text
        aux.textButton.text = text;

        // Si el nombre del bot�n no est� vac�o, establece el nombre del bot�n en buttonName
        // Si el nombre del bot�n est� vac�o, cambia el estado del primer listener del evento onClick del bot�n a Off
        if (buttonName != "")
            aux.button.name = buttonName;
        else
        {
            aux.button.onClick.SetPersistentListenerState(0, UnityEngine.Events.UnityEventCallState.Off);
        }

        //Si la acci�n no es nula, agrega la acci�n al evento listeners y establece empty en false
        if (action != null)
        {
            aux.listeners += action;
            aux.empty = false;
        }

        return aux;
    }

    private void OnDestroy()
    {
        fadeMenu.Stop();
    }
}