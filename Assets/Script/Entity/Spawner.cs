using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField]
    GameObject[] objects;

    private void Awake()
    {
        LoadSystem.AddPostLoadCorutine(()=> {

            int aux = Random.Range(0, objects.Length);
            var go = Instantiate(objects[aux], transform.position, transform.rotation);

            go.GetComponent<Init>()?.Init();
            go.transform.SetParent(transform.parent);
        });
    }
}
