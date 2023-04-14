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
    SpriteRenderer sprite;


    private void OnEnable()
    {
        transform.rotation = MainCamera.instance.transform.GetChild(0).rotation;

        if (!shadow)
            return;


        if(sprite==null)
        {
            sprite = GetComponentInChildren<SpriteRenderer>();

            sprite = Instantiate(sprite, transform);

            sprite.material = material;
        }
                

        sprite.material.SetVector("_Vector2", dirVector);

        sprite.material.SetColor("_Color", colorShadow);

        sprite.sortingLayerName = sortingLayer;
    }
}
