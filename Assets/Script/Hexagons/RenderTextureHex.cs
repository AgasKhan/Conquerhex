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

    [SerializeField]
    Renderer rend;

    [SerializeField]
    Renderer rendCielo;

    Collider2D col;

    bool auxBool;

    void Awake()
    {
        rend.material = materialRend;

        cameraRelated.targetTexture = texture;

        rend.material.SetTexture("_Emission", texture);

        rendCielo.material = effect;

        rendCielo.sortingLayerName = effectLayer;


        var aux = MainCamera.instance.perspective ? 0 : 1;
        rend.material.SetInt("_DeActiveEffect", aux);
        rend.sortingOrder = orderInLayer;
        rend.sortingLayerName = renderLayer;

        col = GetComponent<Collider2D>();

        MyOnBecameInvisible();

       
    }

    private void LateUpdate()
    {
        auxBool = false;

        for (int i = 0; i < MainCamera.instance.points.Length; i++)
        {
            if (col.OverlapPoint(MainCamera.instance.points[i]))
            {
                auxBool = true;
                break;
            }
        }

        if (auxBool)
        {
            MyOnBecameVisible();
        }
        else
        {
            MyOnBecameInvisible();
        }
    }

    void MyOnBecameVisible()
    {
        cameraRelated.SetActiveGameObject(true);
        rendCielo.SetActiveGameObject(true);
    }

    void MyOnBecameInvisible()
    {
        cameraRelated.SetActiveGameObject(false);
        rendCielo.SetActiveGameObject(false);
    }

}
