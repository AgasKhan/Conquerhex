using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IAHunter : IABoid
{

    public override void OnEnterState(Character param)
    {

    }

    public override void OnExitState(Character param)
    {

    }

    public override void OnStayState(Character param)
    {

        //pendiente: necesito el area para que chequee el mas cercano + chequear que no interfiera con el area de detección del arrive
        var corderos = detectEnemy.Area(param.transform.position, (target) => { return Team.hervivoro == target.team; });

        //añado los corderos que estan en mi area de detección, si ya esta en la lista no se añade
        steerings["corderitos"].targets = corderos;


        foreach (var itemInPictionary in steerings)
        {
            for (int i = 0; i < itemInPictionary.value.Count; i++)
            {
                param.move.Acelerator(itemInPictionary.value[i]);
            }
        }
    }

}
