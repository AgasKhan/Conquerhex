using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewPerspectiveController : MonoBehaviour, ViewObjectModel.IViewController
{
    [SerializeField]
    bool active = true;

    public void OnEnterState(ViewObjectModel param)
    {
        if (!active)
            return;

        if (MainCamera.instance.perspective)
            transform.rotation = MainCamera.instance.transform.GetChild(0).rotation;
        else
            transform.rotation = Quaternion.identity;
    }

    public void OnExitState(ViewObjectModel param)
    {

    }

    public void OnStayState(ViewObjectModel param)
    {

    }
}
