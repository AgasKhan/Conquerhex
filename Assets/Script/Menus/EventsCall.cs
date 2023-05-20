using UnityEngine;
using UnityEngine.UI;


public class EventsCall : MonoBehaviour
{
    [SerializeField]
    public Image image;

    public Button button
    {
        get
        {
            if(_button==null)
            {
                _button = GetComponentInChildren<Button>();
            }

            return _button;
        }
    }

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

    [SerializeField]
    Button _button;

    public void Event(GameObject g)
    {
        print("\tAccediendo: " + g.name);

        if(_button==null)
            if (!g.TryGetComponent(out _button))
            {
                DebugPrint.Warning("No contiene uno de los componentes esperados: " + g.name);
                return; 
            }

        _button.onClick.SetPersistentListenerState(0, UnityEngine.Events.UnityEventCallState.Off);

        _button.Event();
        //menu.eventListVoid[b.name](b.gameObject);
        _button.onClick.Invoke();

        //menu.eventListVoid.Remove(b.name);
    }
}