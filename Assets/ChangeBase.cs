using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeBase : MonoBehaviour
{
    // Start is called before the first frame update

    SpriteRenderer sprite;

    [SerializeField]
    Material material;

    [SerializeField]
    RenderTexture render;
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();

 
 

        

        Debug.Log(sprite.materials.Length);
    }

    // Update is called once per frame
    void Update()
    {
        sprite.material.SetTexture("_BaseMap", render);

        sprite.material.SetTexture("_EmissionMap", render);
        

        //sprite.material.SetFloat("_EnableExternalAlpha", 1);

        //sprite.material.SetTexture("_AlphaTex", render);
    }
}
