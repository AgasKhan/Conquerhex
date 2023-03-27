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
            //menu.eventListVoid.Remove(b.name);
            return;
        }
        DebugPrint.Warning("No contiene uno de los componentes esperados: " + g.name);
    }
    void Start()
    {
        menu = MenuManager.instance;
    }


}