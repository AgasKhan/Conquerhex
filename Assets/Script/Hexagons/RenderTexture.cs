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
    int orderInLayer = -4999;

    [SerializeField]
    string renderLayer = "Default";

    [SerializeField]
    public GameObject cameraRelated;
    /*
    [SerializeField]
    Vector2 waves;

    [SerializeField]
    float overrideColor;
    */

    Renderer rend;

    void Awake()
    {
        rend = GetComponentInChildren<Renderer>();

        rend.material = material;

        rend.material.SetTexture("_Emission", texture);
        /*
          spriteRenderer.material.SetVector("_Waves", waves);
          spriteRenderer.material.SetFloat("_OverrideColor", overrideColor);
        */
    }

    private void OnEnable()
    {
        var aux = MainCamera.instance.perspective ? 0 : 1;
        rend.material.SetInt("_DeActiveEffect", aux);
        rend.sortingOrder = orderInLayer;
        rend.sortingLayerName = renderLayer;
    }

    private void LateUpdate()
    {
        if (rend.isVisible)
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
        if (!rend.isVisible)
        {
            cameraRelated.SetActive(false);
        }
    }
}
