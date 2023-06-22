using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransparentMaterial : MonoBehaviour
{
    [SerializeField]
    protected Renderer originalSprite;

    [SerializeField]
    protected Material transparentMaterial;

    [SerializeField]
    Texture mainTexture;

    protected virtual void Awake()
    {
        //originalSprite = GetComponentInChildren<Renderer>();
        originalSprite.material = transparentMaterial;
        
        if(mainTexture!=null)
            originalSprite.material.SetTexture("_MainTex", mainTexture);
    }

    protected virtual void OnEnable()
    {
        EventManager.events.SearchOrCreate<EventGeneric>(EnumPlayer.move).action += UpdateTransparent;
    }

    private void OnDisable()
    {
        EventManager.events.SearchOrCreate<EventGeneric>(EnumPlayer.move).action -= UpdateTransparent;
    }

    private void UpdateTransparent(params object[] param)
    {
        if (!gameObject.activeSelf)
            return;

        Vector3 posPlayer = (Vector3)param[0];

        if (posPlayer.y > transform.position.y)
        {
            originalSprite.material.SetInt("_transparent", 1);
        }
        else
        {
            originalSprite.material.SetInt("_transparent", 0);
        }
    }
}
