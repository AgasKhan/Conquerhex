using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReSpawner : Spawner
{
    [SerializeField]
    bool automaticRespawn = true;

    // Start is called before the first frame update
    Timer respawn;
    protected override void Awake()
    {
        base.Awake();

        autoDestroy = false;
    }

    public override void Init()
    {
        base.Init();

        if (spawneado != null)
        {
            respawn = TimersManager.Create(10, Respawn).Stop();

            if (spawneado.TryGetComponent(out Entity entity) && automaticRespawn)
            {
                entity.health.death += () => respawn.Reset();
            }
        }
    }
    public void TryRespawn()
    {
        if (spawneado != null)
        {
            Respawn();
        }
        else
        {
            Init();
        }
    }

    public void Respawn()
    {
        if(!spawneado.activeSelf && isActiveAndEnabled)
        {
            spawneado.transform.parent = transform.parent;

            spawneado.transform.SetPositionAndRotation(transform.position, transform.rotation);

            spawneado.SetActive(true);

            if(spawneado.TryGetComponent(out Entity entity))
            {
                entity.health.Revive();
            }
        }
    }
}
