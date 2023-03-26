using UnityEngine;
using UnityEngine.UI;


public class EventsCall : MonoBehaviour
{

    MenuManager menu;

    public void Event(GameObject g)
    {
        print("\tAccediendo: " + g.name);

        if (g.TryGetComponent(out Button b))
        {
            b.onClick.SetPersistentListenerState(0, UnityEngine.Events.UnityEventCallState.Off);
            b.Event();
            menu.eventListVoid[b.name](b.gameObject);
            menu.eventListVoid.Remove(b.name);
            return;
        }
        else if (g.TryGetComponent(out Slider s))
        {
            s.onValueChanged.SetPersistentListenerState(0, UnityEngine.Events.UnityEventCallState.Off);
            s.Event();
            menu.eventListFloat[s.name](s.gameObject, s.value);
            menu.eventListVoid.Remove(s.name);
            return;
        }

        DebugPrint.Warning("no contiene uno de los componentes esperados: " + g.name);
    }
    void Start()
    {
        menu = MenuManager.instance;
    }


}