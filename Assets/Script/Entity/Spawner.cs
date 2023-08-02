using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField]
    protected Pictionarys<GameObject, int> objects;

    protected GameObject spawneado;

    protected string jsonGenerated;

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

    protected virtual void LoadCorutine()
    {

        if (autoDestroy)
            Destroy(gameObject);

        if (percentageToSpawn < Random.Range(0, 100))
            return;

        var prefabSelected = objects.RandomPic();

        spawneado = Instantiate(prefabSelected, transform.position, transform.rotation);

        spawneado.transform.SetParent(transform.parent);

        spawneado.GetComponent<Init>()?.Init();            

        if (spawneado.TryGetComponent(out IGetPatrol patrolReturn))
        {
            var patrol = patrolReturn.GetPatrol();
            patrol.patrolParent = patrolParent;
            patrol.reverse = patrolReverse;
        }

        if (spawneado.TryGetComponent(out Entity entity))
        {
            if (setTeam)
                entity.SetTeam(team);

            jsonGenerated = JsonUtility.ToJson(entity);
        }

        if(spawneado.TryGetComponent(out MoveAbstract move))
        {
            var hex = spawneado.GetComponentInParent<Hexagone>();
            if(hex!=null)
                move.Teleport(hex,0);            
        }


            /*
            var rend = GetComponentInChildren<SpriteRenderer>();

            rend.sortingOrder = Mathf.RoundToInt(transform.position.y * -100);
            */

            
    }
}
