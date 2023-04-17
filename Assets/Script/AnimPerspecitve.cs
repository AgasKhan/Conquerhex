using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimPerspecitve : MonoBehaviour
{
    // Start is called before the first frame update
    static int shadowOrder;

    [SerializeField]
    Material material;

    public bool shadow;

    public bool animator;

    [Header("Shadow Config")]
    public Vector2 dirVector;
    public Color colorShadow;
    public string sortingLayer;

    [SerializeField]
    SpriteRenderer originalSprite;

    SpriteRenderer shadowSprite;

    
    [SerializeField]
    float offsetScale=10;
    

    private void Awake()
    {
        originalSprite = GetComponentInChildren<SpriteRenderer>();
        var aux = GetComponentInChildren<Animator>();

        if(aux!=null)
            aux.enabled = animator;
    }

    void CreateShadow()
    {
        shadowSprite = Instantiate(originalSprite, transform);

        Destroy(shadowSprite.GetComponent<Animator>());

        shadowSprite.material = material;

        shadowSprite.transform.localScale = originalSprite.transform.localScale * offsetScale;

        shadowSprite.sortingOrder = shadowOrder;

        originalSprite.RegisterSpriteChangeCallback(UpdateShadowSprite);

        shadowOrder+=2;

        shadowSprite.renderingLayerMask = (uint)Random.Range(2,5);
    }

    void UpdateShadowSprite(SpriteRenderer sprite)
    {
        shadowSprite.sprite = sprite.sprite;
        shadowSprite.flipX = sprite.flipX;
    }

    void UpdateShadow()
    {
        //shadowSprite.material.SetVector("_Vector2", dirVector);

        //shadowSprite.material.SetColor("_Color", colorShadow);

        shadowSprite.sortingLayerName = sortingLayer;

        shadowSprite.gameObject.transform.rotation = Quaternion.identity;

        
    }

    private void OnEnable()
    {
        transform.rotation = MainCamera.instance.transform.GetChild(0).rotation;

        if (!shadow)
            return;

        if(shadowSprite==null)
            CreateShadow();

        UpdateShadow();
    }
}
