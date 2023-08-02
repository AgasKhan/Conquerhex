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

        respawn = TimersManager.Create(10, Respawn).SetLoop(true);
    }

    void Respawn()
    {
        if(!spawneado.activeSelf)
        {
            spawneado.transform.parent = transform.parent;

            spawneado.transform.SetPositionAndRotation(transform.position, transform.rotation);

            spawneado.SetActive(true);
        }
    }
}
