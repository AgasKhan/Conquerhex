using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField]
    protected Pictionarys<GameObject, int> objects;

    protected GameObject spawneado;

    Transform patrolParent;

    [SerializeField]
    protected int percentageToSpawn=100;

    [SerializeField]
    bool autoDestroy=true;

    protected virtual void Awake()
    {
        LoadSystem.AddPostLoadCorutine(()=> {

            if (autoDestroy)
                Destroy(gameObject);

            if (percentageToSpawn < Random.Range(0, 100))
                return;

            spawneado = Instantiate(objects.RandomPic(), transform.position, transform.rotation);

            spawneado.GetComponent<Init>()?.Init();

            spawneado.transform.SetParent(transform.parent);

            if (spawneado.TryGetComponent(out IGetPatrol patrolReturn))
                patrolReturn.GetPatrol().patrolParent = patrolParent;

            var rend = GetComponentInChildren<SpriteRenderer>();

            rend.sortingOrder = Mathf.RoundToInt(transform.position.y * -100);
        });
    }
}
