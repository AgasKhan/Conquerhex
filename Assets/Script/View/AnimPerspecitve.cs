using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimPerspecitve : TransparentMaterial
{
    [SerializeField]
    bool _shadow;

    [SerializeField]
    bool _animator;

    [Header("Shadow Config")]
    public Vector2 dirVector;
    public Color colorShadow;
    public string sortingLayer;

    [SerializeField]
    Material shadowMaterial;

    SpriteRenderer shadowSprite;

    Animator anim;

    public Sprite sprite { get => ((SpriteRenderer)originalSprite).sprite ; set => ((SpriteRenderer)originalSprite).sprite = value; }

    public bool shadow
    {
        get => _shadow;
        set
        {
            if (shadowSprite == null)
            {
                if (value)
                {
                    CreateShadow();
                    UpdateShadow();
                }
            }
            else
            {
                if (value)
                {
                    UpdateShadow();
                }
                else
                {
                    StopAllCoroutines();
                    ((SpriteRenderer)originalSprite).UnregisterSpriteChangeCallback(UpdateShadowSprite);
                    Destroy(shadowSprite.gameObject);
                    shadowSprite = null;
                }
            }

            _shadow = value;
        }
    }

    public bool animator
    {
        get => _animator;

        set
        {
            _animator = value;
            if(anim!=null)
                anim.enabled = animator;
        }
    }

    override protected void Awake()
    {
        base.Awake();

        anim = GetComponentInChildren<Animator>();

        animator = animator;
    }

    public override TransparentMaterial CloneAndSuscribe()
    {
        AnimPerspecitve animPerspecitve = (AnimPerspecitve)base.CloneAndSuscribe();
        animPerspecitve.animator = false;
        return animPerspecitve;
    }

    void CreateShadow()
    {
        shadowSprite = Instantiate(originalSprite, transform) as SpriteRenderer;

        Destroy(shadowSprite.GetComponent<Animator>());

        shadowSprite.material = shadowMaterial;

        ((SpriteRenderer)originalSprite).RegisterSpriteChangeCallback(UpdateShadowSprite);

        shadowSprite.gameObject.transform.rotation = Quaternion.identity;

        shadowSprite.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;

        shadowSprite.sortingLayerName = sortingLayer;


        /*
        shadowSprite.renderingLayerMask = (uint)Mathf.Pow(2, Random.Range(1, 6));

        shadowSprite.sortingOrder = shadowOrder;

        shadowOrder += 2;


        var aux = UpdateBounds(shadowSprite.sprite.bounds);

         print(shadowSprite.bounds + " " + aux);

        shadowSprite.bounds = aux;

        */
    }



    void UpdateShadowSprite(SpriteRenderer sprite)
    {
        shadowSprite.sprite = sprite.sprite;
        shadowSprite.flipX = sprite.flipX;
    }

    Bounds UpdateBounds(Bounds bounds)
    {

        //return new Bounds(transform.TransformPoint(bounds.center), bounds.size * shadowSprite.transform.lossyScale.x);

        return new Bounds(bounds.center, bounds.size * shadowSprite.transform.lossyScale.x*10);
    }

    void UpdateShadow()
    {
        shadowSprite.material.SetVector("_Vector2", Vector2.one);

        shadowSprite.material.SetColor("_Color", colorShadow);

        StartCoroutine(UpdatePostFrame(() => shadowSprite.localBounds = UpdateBounds(shadowSprite.sprite.bounds)));   
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        if (MainCamera.instance.perspective)
            transform.rotation = MainCamera.instance.transform.GetChild(0).rotation;
        else
            transform.rotation = Quaternion.identity;

        shadow = shadow;
    }



    IEnumerator UpdatePostFrame(System.Action action)
    {
        yield return null;

        action();
    }
}
