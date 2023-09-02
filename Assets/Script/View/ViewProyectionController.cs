using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewProyectionController : MonoBehaviour, ViewObjectModel.IViewController
{
    Transform[] proyections = new Transform[6];

    //ViewObjectModel view;

    public void OnEnterState(ViewObjectModel param)
    {
        //view = param;

        proyections = param.Proyections();

        LoadSystem.AddPostLoadCorutine(Proyection);
    }

    void Proyection()
    {
        GetComponentInParent<Hexagone>(true)?.SetProyections(transform, proyections, true);

        if (TryGetComponent<ViewEntityController>(out var viewController) && viewController.entity!=null)
            for (int i = 0; i < viewController.entity.carlitos.Length; i++)
            {
                proyections[i].transform.SetParent(viewController.entity.carlitos[i].transform);
                proyections[i].transform.localPosition = Vector3.zero;
            }
        else
            for (int i = 0; i < proyections.Length; i++)
            {
                proyections[i].transform.localScale = transform.parent.localScale;
            }
    }


    public void OnStayState(ViewObjectModel param)
    {
        throw new System.NotImplementedException();
    }

    public void OnExitState(ViewObjectModel param)
    {
        throw new System.NotImplementedException();
    }
    
}


