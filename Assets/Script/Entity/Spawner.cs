using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField]
    protected GameObject[] objects;

    protected GameObject spawneado;

    protected virtual void Awake()
    {
        LoadSystem.AddPostLoadCorutine(()=> {

            int aux = Random.Range(0, objects.Length);
            spawneado = Instantiate(objects[aux], transform.position, transform.rotation);

            spawneado.GetComponent<Init>()?.Init();
            spawneado.transform.SetParent(transform.parent);

            var rend = GetComponentInChildren<SpriteRenderer>();

            rend.sortingOrder = Mathf.RoundToInt(transform.position.y * -100);
        });
    }
}
