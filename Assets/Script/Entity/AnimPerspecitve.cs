using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimPerspecitve : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField]
    Material shadowMaterial;

    [SerializeField]
    Material transparentMaterial;

    public bool shadow;

    public bool animator;

    [Header("Shadow Config")]
    public Vector2 dirVector;
    public Color colorShadow;
    public string sortingLayer;

    [SerializeField]
    SpriteRenderer originalSprite;

    SpriteRenderer shadowSprite;

    private void Awake()
    {
        originalSprite = GetComponentInChildren<SpriteRenderer>();
        var aux = GetComponentInChildren<Animator>();

        originalSprite.sortingOrder = Mathf.RoundToInt(transform.position.y * -100);

        originalSprite.material = transparentMaterial;


        if (aux!=null)
            aux.enabled = animator;
    }

    private void UpdateTransparent(params object[] param)
    {
        if (!gameObject.activeSelf)
            return;

        Vector3 posPlayer = (Vector3)param[0];

        if(posPlayer.y>transform.position.y)
        {
            originalSprite.material.SetInt("_transparent", 1);
        }
        else
        {
            originalSprite.material.SetInt("_transparent", 0);
        }
    }

    void CreateShadow()
    {
        shadowSprite = Instantiate(originalSprite, transform);

        Destroy(shadowSprite.GetComponent<Animator>());

        shadowSprite.material = shadowMaterial;

        originalSprite.RegisterSpriteChangeCallback(UpdateShadowSprite);

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

    private void OnEnable()
    {
        transform.rotation = MainCamera.instance.transform.GetChild(0).rotation;

        EventManager.events.SearchOrCreate<EventGeneric>(EnumPlayer.move).action += UpdateTransparent;

        if (!shadow)
            return;

        if(shadowSprite==null)
            CreateShadow();

        UpdateShadow();
    }

    private void OnDisable()
    {
        EventManager.events.SearchOrCreate<EventGeneric>(EnumPlayer.move).action -= UpdateTransparent;
    }

    IEnumerator UpdatePostFrame(System.Action action)
    {
        yield return null;

        action();
    }
}
