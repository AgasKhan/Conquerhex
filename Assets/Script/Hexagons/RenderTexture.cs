using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderTexture : MonoBehaviour
{

    [SerializeField]
    Material material;

    [SerializeField]
    Texture texture;

    /*
    [SerializeField]
    Vector2 waves;

    [SerializeField]
    float overrideColor;
    */
    void Awake()
    {
        var sprite = GetComponent<SpriteRenderer>();

        sprite.material = material;

        sprite.material.SetTexture("_Emission", texture);
      /*
        sprite.material.SetVector("_Waves", waves);
        sprite.material.SetFloat("_OverrideColor", overrideColor);
      */
    }

    
}
