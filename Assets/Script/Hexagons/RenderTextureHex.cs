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

    bool _auxBool;

    bool _enabled;

    bool Enabled 
    {
        set
        {
            if (_enabled == value)
                return;

            if (_auxBool)
            {
                MyOnBecameVisible();
            }
            else
            {
                MyOnBecameInvisible();
            }

            _enabled = value;
        }
    }

    public Hexagone hexagone;

    public int lado;

    public void SetRender(Hexagone hexagone, int lado)
    {
        this.hexagone = hexagone;

        this.lado = lado;

        transform.position = HexagonsManager.AbsSidePosHex(hexagone.ladosArray[HexagonsManager.LadoOpuesto(lado)].transform.position, lado, transform.position.z, 2);

        //cameraRelated.transform.position = HexagonsManager.AbsSidePosHex(hexagone.ladosArray[HexagonsManager.LadoOpuesto(lado)].transform.position, lado, cameraRelated.transform.position.z, 2);

        
        cameraRelated.transform.position = new Vector3(
            hexagone.transform.position.x, 
            hexagone.transform.position.y, 
            cameraRelated.transform.position.z);
        
    }


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
        _auxBool = false;

        /*

        for (int i = 0; i < MainCamera.instance.points.Length; i++)
        {
            if (col.OverlapPoint(MainCamera.instance.points[i]))
            {
                //hexagone.SectionView[HexagonsManager.CalcEdge(MainCamera.instance.points[i] - transform.position, 90)].Active=lado;

                _auxBool = true;
            }
        }

        */
        Enabled = _auxBool;
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
