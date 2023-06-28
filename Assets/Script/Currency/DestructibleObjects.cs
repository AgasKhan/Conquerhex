using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleObjects : StaticEntity
{
    [SerializeField]
    StructureBase _structure;
    protected override Damage[] vulnerabilities => _structure.vulnerabilities;

    protected override void Config()
    {
        base.Config();

        MyAwakes += MyAwake;
    }

    private void MyAwake()
    {
        LoadSystem.AddPostLoadCorutine(InitDestructibleObjs);
    }

    void InitDestructibleObjs()
    {
        health.noLife += Health_noLife;

        health.Init(_structure.life, _structure.regen);
    }

    private void Health_noLife()
    {
        gameObject.SetActive(false);
    }
}
