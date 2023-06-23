using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeColorAttack : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    SpriteRenderer sprite;

    [SerializeField]
    public Color areaColor;

    [SerializeField]
    public Color attackColor;

    /*
    [SerializeReference]
    TimedAction offTimer;

    [SerializeReference]
    TimedAction onTimer;
    */

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

    [SerializeField]
    FadeOnOff fadeOnOff;

    [SerializeField]
    UnityEngine.Rendering.Universal.Light2D light2D;

    public Color color
    {
        get => sprite.color;
        set => sprite.color = value;
    } 

    private void Awake()
    {
        /*
        offTimer = TimersManager.LerpInTime(1f, 0, fadeOff, Mathf.Lerp, (fadecolor) => sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, fadecolor));
        onTimer =  TimersManager.LerpInTime(0f, 1, fadeOn,  Mathf.Lerp, (fadecolor) => sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, fadecolor));
        offTimer.AddToEnd(() => gameObject.SetActive(false));
        */

        fadeOnOff.alphas += FadeMenu_alphas;

        fadeOnOff.Init();

        attackTimer = TimersManager.LerpInTime(() => sprite.color, attackColor, fadeAttack, Color.Lerp, (fadecolor) => sprite.color = new Color(fadecolor.r, fadecolor.g, fadecolor.b, sprite.color.a));

        noAttackTimer = TimersManager.LerpInTime(() => sprite.color, areaColor, fadeNoAttack, Color.Lerp, (fadecolor) => sprite.color = new Color(fadecolor.r, fadecolor.g, fadecolor.b, sprite.color.a));

        attackTimer.AddToEnd(()=> NoAttack());
    }

    private void FadeMenu_alphas(float obj)
    {
        sprite.color = sprite.color.ChangeAlphaCopy(obj);
        light2D.color = attackColor.ChangeAlphaCopy(obj/2);
    }

    public FadeColorAttack On()
    {
        gameObject.SetActive(true);

        return this;
    }

    public FadeColorAttack Attack()
    {
        attackTimer.Reset();
        return this;
    }

    public FadeColorAttack Off()
    {
        fadeOnOff.end += FadeMenu_end;
        fadeOnOff.FadeOff().Set(fadeOff);

        return this;
    }

    public FadeColorAttack Area(out float number)
    {
        number = transform.localScale.x;
        return this;
    }

    public FadeColorAttack Area(float number)
    {
        transform.localScale = Vector3.one * 2 * number;
        light2D.pointLightOuterRadius = number;
        return this;
    }

    FadeColorAttack NoAttack()
    {
        noAttackTimer.Reset();
        return this;
    }

    private void FadeMenu_end()
    {
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        sprite.color = areaColor.ChangeAlphaCopy(0);
        fadeOnOff.end -= FadeMenu_end;
        fadeOnOff.FadeOn().Set(fadeOn);
    }
}
