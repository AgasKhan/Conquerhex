using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOnOff : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    SpriteRenderer sprite;

    [SerializeField]
    Color fadeColorOn;

    [SerializeField]
    Color fadeColorOff;

    TimedAction offTimer;

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
        offTimer = TimersManager.Create(fadeOff, () => gameObject.SetActive(false), false);

        onTimer = Utilitys.LerpInTime(sprite.color, fadeColorOn, fadeOn, Color.Lerp, (fadecolor) => sprite.color = fadecolor).SetDestroy(false);
    }

    public void On()
    {
        gameObject.SetActive(true);
    }

    public void Off()
    {
        Utilitys.LerpInTime(sprite.color, fadeColorOff, fadeOff, Color.Lerp, (fadecolor) => sprite.color = fadecolor);
        offTimer.Reset();
    }

    private void OnEnable()
    {
        transform.localScale = Vector3.one;
        sprite.color = fadeColorOff;
        onTimer.Reset();
    }
}
