using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecretKey : MonoBehaviour
{
    public GameObject myObject;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Comma))
            myObject.SetActive(true);
    }
}
