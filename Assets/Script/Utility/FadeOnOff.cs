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

    TimedAction routine;

    [SerializeField]
    float fadeOn;

    [SerializeField]
    float fadeOff;

    private void Awake()
    {
        routine = TimersManager.Create(fadeOff, () => gameObject.SetActive(false), false);
    }

    public void On()
    {
        gameObject.SetActive(true);
    }

    public void Off()
    {
        Utilitys.LerpInTime(sprite.color, fadeColorOff, fadeOff, Color.Lerp, (fadecolor) => sprite.color = fadecolor);
        routine.Reset();
    }

    private void OnEnable()
    {
        sprite.color = fadeColorOff;
        Utilitys.LerpInTime(sprite.color, fadeColorOn, fadeOn, Color.Lerp, (fadecolor) => sprite.color = fadecolor);
    }
}
