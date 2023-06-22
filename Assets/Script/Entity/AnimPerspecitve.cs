using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimPerspecitve : TransparentMaterial
{
    public bool shadow;

    public bool animator;

    [Header("Shadow Config")]
    public Vector2 dirVector;
    public Color colorShadow;
    public string sortingLayer;

    [SerializeField]
    Material shadowMaterial;


    SpriteRenderer shadowSprite;

    override protected void Awake()
    {
        base.Awake();

        var aux = GetComponentInChildren<Animator>();

        originalSprite.sortingOrder = Mathf.RoundToInt(transform.position.y * -100);

        if (aux!=null)
            aux.enabled = animator;
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

        if (!shadow)
            return;

        if(shadowSprite==null)
            CreateShadow();

        UpdateShadow();
    }

   

    IEnumerator UpdatePostFrame(System.Action action)
    {
        yield return null;

        action();
    }
}
