using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class HardcodeoArte : MonoBehaviour
{
    [SerializeField]
    VisualEffect visualEffect;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.K) && visualEffect.isActiveAndEnabled)
        {
            visualEffect.SetActiveGameObject(false);
            
            if (Input.GetKey(KeyCode.K))
            {
                visualEffect.SetBool("SpawnDespawn", true);
                visualEffect.SetActiveGameObject(true);
            }
        }
    }
}

