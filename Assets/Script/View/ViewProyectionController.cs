using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewProyectionController : MonoBehaviour, ViewObjectModel.IViewController
{
    Transform[] proyections = new Transform[6];

    Transform originalParent;

    ViewObjectModel view;

    Hexagone hex;

    int lado;

    void Proyection()
    {
        hex = GetComponentInParent<Hexagone>(true);

        if (hex == null)
            return;

        lado = HexagonsManager.CalcEdge(transform.position - hex.transform.position);

        hex.SuscribeOnSection(lado, ChangeToProyection);

        if (TryGetComponent<ViewEntityController>(out var viewController) && viewController.entity != null)
        {
            for (int i = 0; i < viewController.entity.carlitos.Length; i++)
            {
                proyections[i] = viewController.entity.carlitos[i].transform;
            }

            if(viewController.entity is DynamicEntity)
            {
                var dyn = (DynamicEntity)viewController.entity;

                dyn.move.onMove += Move_onMove;

                dyn.move.onTeleport += Move_onTeleport;
            }
        }
        else
        {
            proyections = view.Proyections();

            hex.SetProyections(transform, proyections);

            /*
            for (int i = 0; i < proyections.Length; i++)
            {
                proyections[i].transform.localScale = transform.parent.localScale;
            }
            */
        }       
    }

    private void Move_onTeleport(Hexagone arg1, int arg2)
    {
        hex.DesuscribeOnSection(lado, ChangeToProyection);
        hex = arg1;
        lado = arg2;
        hex.SuscribeOnSection(lado, ChangeToProyection);
    }

    private void Move_onMove(Vector2 obj)
    {
        var aux = HexagonsManager.CalcEdge(view.transform.position - hex.transform.position);

        if(aux==lado)
            return;

        hex.DesuscribeOnSection(lado, ChangeToProyection);

        lado = aux;

        hex.SuscribeOnSection(lado, ChangeToProyection);
    }

    void ChangeToProyection(int lado)
    {
        if (!isActiveAndEnabled)
            return;

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
      
    }

    public void OnExitState(ViewObjectModel param)
    {

    }
    
}


