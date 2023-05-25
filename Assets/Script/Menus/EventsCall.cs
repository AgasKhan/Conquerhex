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

    public float durationAnim=0.3f;

    public float durationWait = 0.1f;

    public Timer timerOn;

    Timer textOn;

    Timer imageOn;

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
        textOn = TimersManager.LerpInTime(0f, 1, durationAnim, Mathf.Lerp, (save) => textButton.color = textButton.color.ChangeAlphaCopy(save)).SetUnscaled(true).Stop();

        imageOn = TimersManager.LerpInTime(0f, 1, durationAnim, Mathf.Lerp, (save) => image.color = image.color.ChangeAlphaCopy(save)).SetUnscaled(true).Stop();

        timerOn = TimersManager.Create(durationWait, () =>
        {
            textOn.Reset();

            imageOn.Reset();

        }).SetUnscaled(true).Stop();
    }

    private void OnEnable()
    {
        textButton.color = textButton.color.ChangeAlphaCopy(0);

        image.color = image.color.ChangeAlphaCopy(0);

        timerOn.Reset();
    }

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
        var aux = Instantiate(this, content);

        aux.textButton.text = text;

        if (buttonName != "")
            aux.button.name = buttonName;
        else
        {
            aux.button.onClick.SetPersistentListenerState(0, UnityEngine.Events.UnityEventCallState.Off);
        }

        if (action != null)
        {
            aux.listeners += action;
            aux.empty = false;
        }

        return aux;
    }

    private void OnDestroy()
    {
        timerOn.Stop();
        textOn.Stop();
        imageOn.Stop();
    }
}