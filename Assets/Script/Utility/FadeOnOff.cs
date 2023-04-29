using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOnOff : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    SpriteRenderer sprite;

    [SerializeField]
    public Color areaColor;

    [SerializeField]
    public Color attackColor;

    [SerializeReference]
    TimedAction offTimer;

    [SerializeReference]
    TimedAction onTimer;

    [SerializeField]
    float fadeOn;

    [SerializeField]
    float fadeOff;

    public Color color
    {
        get => sprite.color;
        set => sprite.color = value;
    }

    private void Awake()
    {
        offTimer = TimersManager.LerpInTime(1f, 0, fadeOff, Mathf.Lerp, (fadecolor) => sprite.color = new Color(attackColor.r, attackColor.g, attackColor.b, fadecolor));

        offTimer.AddToEnd(() => gameObject.SetActive(false));

        onTimer = TimersManager.LerpInTime(0f, 1, fadeOn, Mathf.Lerp, (fadecolor) => sprite.color = new Color(areaColor.r, areaColor.g, areaColor.b, fadecolor));
    }

    public FadeOnOff On()
    {
        gameObject.SetActive(true);

        return this;
    }

    public FadeOnOff Off()
    {
        offTimer.Reset();

        return this;
    }

    private void OnEnable()
    {
        sprite.color = new Color(areaColor.r, areaColor.g, areaColor.b, 0);

        onTimer.Reset();
    }

    
}
