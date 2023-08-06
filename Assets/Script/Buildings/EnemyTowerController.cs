using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTowerController : TurretController
{
    protected override void Config()
    {
        base.Config();
        MyAwakes += MyAwake;
        MyStarts = null;
    }
    void MyAwake()
    {
        LoadSystem.AddPostLoadCorutine(MyStart);
    }

    void MyStart()
    {
        for (int i = 0; i < turret.myStructure.kataCombos.Length; i++)
        {
            turret.SetKataCombo(i);
        }

        turret.interactuable = false;
    }

    protected override void DestroyTurret()
    {
        base.DestroyTurret();
        transform.GetChild(0).SetActiveGameObject(false);
        turret.visible = false;
    }

}
