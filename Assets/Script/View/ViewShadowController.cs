using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewShadowController : MonoBehaviour, ViewObjectModel.IViewController
{
    [SerializeField]
    bool _shadow = true;

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

    ViewObjectModel viewObjectModel;

    SpriteRenderer originalSpriteRenderer => (SpriteRenderer)viewObjectModel.originalRender;

    public Sprite sprite { get => originalSpriteRenderer.sprite ; set => originalSpriteRenderer.sprite = value; }

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
                    originalSpriteRenderer.UnregisterSpriteChangeCallback(UpdateShadowSprite);
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

    public void OnEnterState(ViewObjectModel param)
    {
        viewObjectModel = param;

        anim = GetComponentInChildren<Animator>();

        animator = animator;

        shadow = true;
    }

    public void OnStayState(ViewObjectModel param)
    {
        throw new System.NotImplementedException();
    }

    public void OnExitState(ViewObjectModel param)
    {
        throw new System.NotImplementedException();
    }

    void CreateShadow()
    {
        shadowSprite = Instantiate(originalSpriteRenderer, transform);

        Destroy(shadowSprite.GetComponent<Animator>());

        shadowSprite.material = shadowMaterial;

        originalSpriteRenderer.RegisterSpriteChangeCallback(UpdateShadowSprite);

        shadowSprite.gameObject.transform.rotation = Quaternion.identity;

        shadowSprite.maskInteraction = SpriteMaskInteraction.None;

        shadowSprite.sortingLayerName = sortingLayer;
    }

    void UpdateShadowSprite(SpriteRenderer sprite)
    {
        shadowSprite.sprite = sprite.sprite;
        shadowSprite.flipX = sprite.flipX;
    }

    Bounds UpdateBounds(Bounds bounds)
    {
        //return new Bounds(transform.TransformPoint(bounds.center), bounds.size * shadowSprite.transform.lossyScale.x);

        return new Bounds(bounds.center, bounds.size * originalSpriteRenderer.transform.lossyScale.x * 10);
    }

    void UpdateShadow()
    {
        shadowSprite.material.SetVector("_Vector2", Vector2.one);

        shadowSprite.material.SetColor("_Color", colorShadow);

        shadowSprite.ResetLocalBounds();

        shadowSprite.localBounds = UpdateBounds(shadowSprite.localBounds);
    }

    private void OnDrawGizmosSelected()
    {
        if(shadowSprite!=null)
            Gizmos.DrawWireCube(shadowSprite.localBounds.center + transform.position, shadowSprite.localBounds.size);
    }

}