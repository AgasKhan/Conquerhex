using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecolectableItem : StaticEntity
{
    /*
    [SerializeField]
    Detect<Entity> detect;
    */

    //bool recolect;

    TimedCompleteAction recolect;

    StaticEntity referenceToTravel;

    protected override void Config()
    {
        base.Config();

        MyAwakes += MyAwake;
    }

    void MyAwake()
    {
        recolect = TimersManager.LerpInTime(transform.position, ()=> referenceToTravel.transform.position, 1, Vector3.Slerp, (pos) => transform.position = pos);

        recolect.AddToEnd(() =>
        {
            referenceToTravel.AddAllItems(this);
            gameObject.SetActive(false);
            Debug.Log("me recoge: " + referenceToTravel.name);

        }).Stop().SetUnscaled(false).current = 0;
    }

    public void Recolect(StaticEntity entity)
    {
        if (!recolect.Chck)
            return;

        Debug.Log("me quiere recoger: " + entity.name);

        referenceToTravel = entity;

        recolect.Reset();  
    }


    /*
    private void Update()
    {
        var depredadores = detect.Area(transform.position, (algo) => { return Team.hervivoro == algo.team; });
        Debug.Log("depredadores " + depredadores.Count);
        foreach (var depredador in depredadores)
        {
            var aux = depredador.GetComponent<IABoid>();
            aux.steerings["frutas"].targets.Remove(this);
            Debug.Log("depredador " + depredador.name);
            gameObject.SetActive(false);
        }
    }
    */

}
