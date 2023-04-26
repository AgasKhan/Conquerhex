using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField]
    GameObject obj;

    private void Awake()
    {
        LoadSystem.AddPostLoadCorutine(()=> {
            obj = Instantiate(obj, transform.position, transform.rotation);
            obj.GetComponent<Init>()?.Init();
            obj.transform.SetParent(transform.parent); 
        });
    }
}
