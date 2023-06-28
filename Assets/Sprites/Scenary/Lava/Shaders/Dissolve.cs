using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dissolve : MonoBehaviour
{
    [SerializeField] private Material material;

    private float dissolveAmount;
    private bool isDissolve;

    private void Update()
    {
        if (isDissolve)
        {
            dissolveAmount = Mathf.Clamp01(dissolveAmount + Time.deltaTime);
            material.SetFloat("_Dissolve", dissolveAmount);
        }
        else
        {
            dissolveAmount = Mathf.Clamp01(dissolveAmount - Time.deltaTime);
            material.SetFloat("_Dissolve", dissolveAmount);
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            isDissolve = true;
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            isDissolve = false;
        }
    }

}
