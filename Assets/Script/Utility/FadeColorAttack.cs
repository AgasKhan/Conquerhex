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

    [SerializeField]
    public Color lightColor;

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

    //[SerializeField]
    //UnityEngine.Rendering.Universal.Light2D light2D;

    public Color color
    {
        get => sprite.color;
        set => sprite.color = value;
    } 

    private void Awake()
    {
        //lightColor = light2D.color;

        fadeOnOff.alphas += FadeMenu_alphas;

        fadeOnOff.Init();

        attackTimer = TimersManager.Create(() => sprite.color, attackColor, fadeAttack, Color.Lerp, (fadecolor) => sprite.color = new Color(fadecolor.r, fadecolor.g, fadecolor.b, sprite.color.a));

        noAttackTimer = TimersManager.Create(() => sprite.color, areaColor, fadeNoAttack, Color.Lerp, (fadecolor) => sprite.color = new Color(fadecolor.r, fadecolor.g, fadecolor.b, sprite.color.a));

        attackTimer.AddToEnd(()=> NoAttack());
    }

    private void FadeMenu_alphas(float obj)
    {
        sprite.color = sprite.color.ChangeAlphaCopy(obj);
        //light2D.color = lightColor.ChangeAlphaCopy(obj/2);
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

    public FadeColorAttack DotAngle(float dot)
    {
        sprite.material.SetFloat("_Dot", dot);

        return this;
    }

    public FadeColorAttack InternalArea(float area)
    {
        sprite.material.SetFloat("_InternalArea", area);

        return this;
    }

    public FadeColorAttack Direction(Vector2 dir)
    {
        transform.up = dir.Vec2to3(transform.up.z);

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
        //light2D.pointLightOuterRadius = number;
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
        DotAngle(-1);
        sprite.color = areaColor.ChangeAlphaCopy(0);
        fadeOnOff.end -= FadeMenu_end;
        fadeOnOff.FadeOn().Set(fadeOn);
    }
}
