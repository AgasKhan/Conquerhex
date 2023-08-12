using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderTextureHex : MonoBehaviour
{
    [SerializeField]
    Material materialRend;

    [SerializeField]
    Material effect;

    [SerializeField]
    RenderTexture texture;

    [SerializeField]
    int orderInLayer = -4999;

    [SerializeField]
    string renderLayer = "Default";

    [SerializeField]
    string effectLayer = "Cielo";

    [SerializeField]
    public Camera cameraRelated;
    /*
    [SerializeField]
    Vector2 waves;

    [SerializeField]
    float overrideColor;
    */

    [SerializeField]
    Renderer rend;

    [SerializeField]
    Renderer rendCielo;

    void Awake()
    {
        rend.material = materialRend;

        cameraRelated.targetTexture = texture;

        rend.material.SetTexture("_Emission", texture);

        rendCielo.material = effect;

        rendCielo.sortingLayerName = effectLayer;
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
            cameraRelated.SetActiveGameObject(true);
        }
        else if (cameraRelated.isActiveAndEnabled)
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
            cameraRelated.SetActiveGameObject(false);
        }
    }
}
