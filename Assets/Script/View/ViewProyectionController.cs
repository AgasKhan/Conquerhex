using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewProyectionController : MonoBehaviour, ViewObjectModel.IViewController
{
    Transform[] proyections = new Transform[6];

    Transform originalParent;

    ViewObjectModel view;

    void Proyection()
    {
        if (TryGetComponent<ViewEntityController>(out var viewController) && viewController.entity != null)
            for (int i = 0; i < viewController.entity.carlitos.Length; i++)
            {
                proyections[i] = viewController.entity.carlitos[i].transform;
            }
        else
        {
            proyections = view.Proyections();

            var hex = GetComponentInParent<Hexagone>(true);

            hex?.SetProyections(transform, proyections, true).SuscribeOnSection(HexagonsManager.CalcEdge(transform.position - hex.transform.position), ChangeToProyection);

            for (int i = 0; i < proyections.Length; i++)
            {
                proyections[i].transform.localScale = transform.parent.localScale;
            }
        }       
    }

    void ChangeToProyection(int lado)
    {
        Transform aux;

        if (lado < 0)
            aux = originalParent;
        else
            aux = proyections[HexagonsManager.LadoOpuesto(lado)];

        view.transform.SetParent(aux);

        view.transform.localPosition = Vector3.zero;
    }

    public void OnEnterState(ViewObjectModel param)
    {
        view = param;

        originalParent = view.transform.parent;

        LoadSystem.AddPostLoadCorutine(Proyection);
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


