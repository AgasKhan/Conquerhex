using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderTexture : MonoBehaviour
{
    [SerializeField]
    Material material;

    [SerializeField]
    Texture texture;

    [SerializeField]
    public GameObject cameraRelated;
    /*
    [SerializeField]
    Vector2 waves;

    [SerializeField]
    float overrideColor;
    */

    SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        spriteRenderer.material = material;

        spriteRenderer.material.SetTexture("_Emission", texture);
        /*
          spriteRenderer.material.SetVector("_Waves", waves);
          spriteRenderer.material.SetFloat("_OverrideColor", overrideColor);
        */
    }

    private void OnEnable()
    {
        var aux = MainCamera.instance.perspective ? 1 : 0;
        spriteRenderer.material.SetInt("_DeActiveEffect", aux);
    }

    private void LateUpdate()
    {
        if (spriteRenderer.isVisible)
        {
            cameraRelated.SetActive(true);
        }
        else if (cameraRelated.activeSelf)
        {
            //cameraRelated.SetActive(false);
            StartCoroutine(RetardedOf());
        }
    }

    IEnumerator RetardedOf()
    {
        yield return null;
        if (!spriteRenderer.isVisible)
        {
            cameraRelated.SetActive(false);
        }
    }
}
