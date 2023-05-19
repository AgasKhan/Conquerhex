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

    [SerializeReference]
    TimedAction attackTimer;

    [SerializeReference]
    TimedAction noAttackTimer;

    [SerializeField]
    float fadeOn;

    [SerializeField]
    float fadeOff;

    [SerializeField]
    float fadeAttack;

    [SerializeField]
    float fadeNoAttack;

    public Color color
    {
        get => sprite.color;
        set => sprite.color = value;
    }

    private void Awake()
    {
        offTimer = TimersManager.LerpInTime(1f, 0, fadeOff, Mathf.Lerp, (fadecolor) => sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, fadecolor));

        offTimer.AddToEnd(() => gameObject.SetActive(false));

        onTimer = TimersManager.LerpInTime(0f, 1, fadeOn, Mathf.Lerp, (fadecolor) => sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, fadecolor));

        attackTimer = TimersManager.LerpInTime(() => sprite.color, attackColor, fadeAttack, Color.Lerp, (fadecolor) => sprite.color = new Color(fadecolor.r, fadecolor.g, fadecolor.b, sprite.color.a));

        noAttackTimer = TimersManager.LerpInTime(() => sprite.color, areaColor, fadeNoAttack, Color.Lerp, (fadecolor) => sprite.color = new Color(fadecolor.r, fadecolor.g, fadecolor.b, sprite.color.a));

        attackTimer.AddToEnd(()=> NoAttack());
    }

    public FadeOnOff On()
    {
        gameObject.SetActive(true);

        return this;
    }

    public FadeOnOff Attack()
    {
        attackTimer.Reset();
        return this;
    }

    FadeOnOff NoAttack()
    {
        noAttackTimer.Reset();
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
