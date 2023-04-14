using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimPerspecitve : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField]
    Material material;

    public bool shadow;

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
    }

    void CreateShadow()
    {
        shadowSprite = Instantiate(originalSprite, transform);

        shadowSprite.material = material;

        originalSprite.RegisterSpriteChangeCallback(UpdateShadowSprite);
    }

    void UpdateShadowSprite(SpriteRenderer sprite)
    {
        shadowSprite.sprite = sprite.sprite;
        shadowSprite.flipX = sprite.flipX;
    }

    void UpdateShadow()
    {
        shadowSprite.material.SetVector("_Vector2", dirVector);

        shadowSprite.material.SetColor("_Color", colorShadow);

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
