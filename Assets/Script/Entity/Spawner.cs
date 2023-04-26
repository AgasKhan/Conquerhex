using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField]
    GameObject obj;

    void Awake()
    {
        obj = Instantiate(obj, transform.position, transform.rotation, transform.parent);

        obj.GetComponent<Init>()?.Init();

        Destroy(this);
    }
}
