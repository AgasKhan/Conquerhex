using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour, Init
{
    [SerializeField]
    protected Pictionarys<GameObject, int> objects;

    protected GameObject spawneado;

    [SerializeField]
    protected bool setTeam;
    [SerializeField]
    protected Team team;

    [SerializeField]
    Transform patrolParent;
    [SerializeField]
    bool patrolReverse;

    [SerializeField]
    protected int percentageToSpawn=100;

    [SerializeField]
    protected bool autoDestroy = true;

    protected virtual void Awake()
    {
        LoadSystem.AddPostLoadCorutine(LoadCorutine);
    }

    void LoadCorutine()
    {
        Init();
    }

    public virtual void Init()
    {
        if (autoDestroy)
            Destroy(gameObject);
        else
            GetComponent<SpriteRenderer>().enabled = false;

        if (percentageToSpawn < Random.Range(0, 100))
            return;

        var prefabSelected = objects.RandomPic();

        spawneado = Instantiate(prefabSelected, transform.position, transform.rotation);

        spawneado.transform.SetParent(transform.parent);

        if (spawneado.TryGetComponent(out PatrolLibrary.IGetPatrol patrolReturn))
        {
            var patrol = patrolReturn.GetPatrol();
            patrol.patrolParent = patrolParent;
            patrol.reverse = patrolReverse;
        }

        if (spawneado.TryGetComponent(out Entity entity))
        {
            if (setTeam)
                entity.SetTeam(team);
        }
        else
        {
            if (spawneado.TryGetComponent(out MoveAbstract move))
            {
                var hex = spawneado.GetComponentInParent<Hexagone>(true);
                if (hex != null)
                    move.Teleport(hex, 0);
            }
        }

        spawneado.GetComponent<Init>()?.Init();
        /*
        var rend = GetComponentInChildren<SpriteRenderer>();

        rend.sortingOrder = Mathf.RoundToInt(transform.position.y * -100);
        */

    }
}
