using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeColorAttack : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    Renderer sprite;

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

    [SerializeField]
    Transform areaFeedback;

    [SerializeField]
    TMPro.TextMeshProUGUI text;

    [SerializeField]
    CircularTextMeshPro textCircular;

    [SerializeField]
    float velocityRotation = 5;

    float internalDot;

    string _area;

    string _angle;
    string area
    {
        set
        {
            _area = value;
            text.text = $"{_area}\n{_angle}";
        }
    }
    string angle
    {
        set
        {
            _angle = value;
            text.text = $"{_area}\n{_angle}";
        }
    }

    //[SerializeField]
    //UnityEngine.Rendering.Universal.Light2D light2D;

    public Color color
    {
        get => text.color;
        set
        {
            sprite.material.SetColor("_Color", value);
            text.color = value;
        }
    } 

    private void Awake()
    {
        //lightColor = light2D.color;

        fadeOnOff.alphas += FadeMenu_alphas;

        fadeOnOff.Init();

        attackTimer = TimersManager.Create(() => color, attackColor, fadeAttack, Color.Lerp, (fadecolor) => color = new Color(fadecolor.r, fadecolor.g, fadecolor.b, color.a));

        noAttackTimer = TimersManager.Create(() => color, areaColor, fadeNoAttack, Color.Lerp, (fadecolor) => color = new Color(fadecolor.r, fadecolor.g, fadecolor.b, color.a));

        attackTimer.AddToEnd(()=> NoAttack());
    }

    private void Update()
    {
        text.transform.rotation *= Quaternion.Euler(0,0, velocityRotation*Time.deltaTime);
    }

    private void FadeMenu_alphas(float obj)
    {
        color = color.ChangeAlphaCopy(obj);
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

    public FadeColorAttack Angle(float angle)
    {
        if (internalDot == angle)
            return this;

        sprite.material.SetFloat("_Angle", angle);
        this.angle = "Angle: " + angle.ToStringFixed(0) + "º";
        return this;
    }

    public FadeColorAttack InternalArea(float area)
    {
        sprite.material.SetFloat("_InternalArea", area * transform.localScale.x);

        return this;
    }

    public FadeColorAttack Direction(Vector3 dir)
    {
        areaFeedback.localRotation = Quaternion.Euler(0,0,Utilitys.DifAngulosVectores(Vector2.up, dir.Vect3To2XZ()));

        return this;
    }

    public FadeColorAttack Area(out float number)
    {
        number = areaFeedback.localScale.x;
        return this;
    }

    public FadeColorAttack Area(float max, float min=0)
    {
        areaFeedback.localScale = Vector3.one * max;

        textCircular.Radius = max ;

        area = "MaxRadius: " + max.ToStringFixed();
        InternalArea(min);

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
        Angle(360);
        color = areaColor.ChangeAlphaCopy(0);
        fadeOnOff.end -= FadeMenu_end;
        fadeOnOff.FadeOn().Set(fadeOn);

        text.transform.rotation *= Quaternion.Euler(0, 0, Random.Range(0,360));
    }

    private void OnDisable()
    {
        if(!transform.parent?.gameObject.activeSelf ?? false)
        {
            gameObject.SetActive(false);
        }
    }
}
