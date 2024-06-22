using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RetardedOn : MonoBehaviour
{
    [SerializeField]
    GameObject layoutGroup;

    private void Awake()
    {
        if (layoutGroup == null)
            layoutGroup = transform.gameObject;
    }

    private void OnEnable()
    {
        GameManager.RetardedOn((_bool) => layoutGroup.SetActive(_bool));
    }
}
