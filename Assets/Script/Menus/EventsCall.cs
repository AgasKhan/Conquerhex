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
        fadeMenu.Stop();
    }
}


[System.Serializable]
public class FadeOnOff : Init
{
    public float durationAnim = 0.3f;

    public float durationWait = 0.1f;

    [SerializeReference]
    public Timer timerOn;

    public bool unscaled = true;

    Vector2 fades;

    Timer fadeOn;

    public event System.Action<float> alphas;
    public event System.Action end;

    public void Init(params object[] param)
    {
        fadeOn = TimersManager.LerpInTime(()=>fades.x, ()=>fades.y, durationAnim, Mathf.Lerp, alphas).AddToEnd(()=> end?.Invoke()).SetUnscaled(unscaled).Stop();

        timerOn = TimersManager.Create(durationWait, () =>
        {
            fadeOn.Reset();

        }).SetUnscaled(unscaled).Stop();
    }

    public Timer FadeOn()
    {
        return SetFade(0, 1);
    }

    public Timer FadeOff()
    {
        return SetFade(1, 0);
    }

    public Timer SetFade(float init, float end)
    {
        alphas?.Invoke(init);
        fades.x = init;
        fades.y = end;

        timerOn.Reset();

        return timerOn;
    }

    public void Stop()
    {
        timerOn.Stop();
        fadeOn.Stop();
    }
}