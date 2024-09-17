using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace CustomEulerEditor
{
    [CustomEditor(typeof(TimersManager))]
    public class TimersManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            GUIStyle style = new GUIStyle(GUI.skin.label);

            style.richText = true;

            //TimersManager timersManager = target as TimersManager;

            //GUILayout.Label(("Cantidad de timers creados: " + TimersManager.CountCreated).RichText("size", "15"), style);

            GUILayout.Label(("Cantidad de timers en la lista: " + TimersManager.timersList.Count).RichText("size", "15"), style);

            base.OnInspectorGUI();
        }
    }
}