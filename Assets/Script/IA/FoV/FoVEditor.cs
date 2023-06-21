using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
[CustomEditor (typeof (FieldOfView))]
public class FoVEditor : Editor
{
    private void OnSceneGUI()
    {
        /*
        FieldOfView fow = (FieldOfView)target;
        Handles.color = Color.white;
        //Circulo
        Handles.DrawWireArc(fow.transform.position, Vector3.forward, Vector3.right, 360, fow.viewRadius);
        //angulos
        Vector2 viewAngleA = fow.DirFromAngle(-fow.viewAngle / 2, false);
        Vector2 viewAngleB = fow.DirFromAngle(fow.viewAngle / 2, false);

        //longitud de la línea
        Handles.DrawLine(fow.transform.position, (fow.transform.position).Vect3To2() + viewAngleA * fow.viewRadius);
        Handles.DrawLine(fow.transform.position, (fow.transform.position).Vect3To2() + viewAngleB * fow.viewRadius);

        //Linea hasta el target
        Handles.color = Color.red;
        foreach (Transform visibleTarget in fow.visibleTargets)
        {
            Handles.DrawLine(fow.transform.position, visibleTarget.position);
        }
        */
    }

}

#endif
