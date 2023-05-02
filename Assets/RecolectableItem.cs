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

    [SerializeField]
    ItemBase initialItems;
    [SerializeField]
    int amount;

    Timer recolect;

    StaticEntity referenceToTravel;

    public DropItem myDrop;

    protected override void Config()
    {
        base.Config();

        MyAwakes += MyAwake;
    }

    void MyAwake()
    {
        recolect = TimersManager.LerpInTime(() => transform.position, ()=> referenceToTravel.transform.position, 1, Vector3.Slerp, (pos) => transform.position = pos)
        .AddToEnd(() =>
        {
            referenceToTravel.AddAllItems(this);
            gameObject.SetActive(false);
            
        })
        .Stop();

        recolect.current = 0;

        AddOrSubstractItems(initialItems.nameDisplay, amount);
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
