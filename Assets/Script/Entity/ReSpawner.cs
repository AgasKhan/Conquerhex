using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReSpawner : Spawner
{
    // Start is called before the first frame update
    Timer respawn;
    protected override void Awake()
    {
        base.Awake();

        autoDestroy = false;
    }

    protected override void LoadCorutine()
    {
        base.LoadCorutine();

        if (spawneado != null)
        {
            respawn = TimersManager.Create(10, Respawn).SetLoop(true);

            if (spawneado.TryGetComponent(out Entity entity))
            { 
                entity.health.death += () => respawn.Reset();         
            }
        }
            
    }

    void Respawn()
    {
        if(!spawneado.activeSelf && isActiveAndEnabled)
        {
            spawneado.transform.parent = transform.parent;

            spawneado.transform.SetPositionAndRotation(transform.position, transform.rotation);

            spawneado.SetActive(true);

            if(spawneado.TryGetComponent(out Entity entity))
            {
                JsonUtility.FromJsonOverwrite(jsonGenerated, entity);
            }
        }
    }
}
