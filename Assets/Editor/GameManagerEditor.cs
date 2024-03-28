using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        //en target siempre tenemos lo que queremos analizar en caso de editores
        GameManager gameManager = target as GameManager;

        string state = gameManager.ActualStringState();

        string tiempo = Time.timeScale.ToString("0.00");

        GUIStyle style = new GUIStyle(GUI.skin.label);

        style.richText = true;

        if (Time.timeScale < 1)
            tiempo = tiempo.RichTextColor(Color.red);
        else if (Time.timeScale == 1)
            tiempo = tiempo.RichTextColor(Color.green);
        else
            tiempo = tiempo.RichTextColor(Color.blue);

        switch (state)
        {
            case "Load":
                state = state.RichTextColor(Color.yellow);
                break;

            case "Gameplay":
                state = state.RichTextColor(Color.green);
                break;

            case "Pause":
                state = state.RichTextColor(Color.grey);
                break;

            default:
                state = state.RichTextColor(Color.red);
                break;
        }

        GUILayout.Label(("Estado del juego: " + state).RichText("size", "20"), style);

        GUILayout.Space(10);

        GUILayout.Label(("Escala de tiempo: " + tiempo).RichText("size", "20"), style);

        GUILayout.Space(20);

        base.OnInspectorGUI();
    }
}
