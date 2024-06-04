using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(ScriptableObject), true)]
public class ScriptableObjectProperty : PropertyDrawer
{
    const float muliplyHeight = 10;

    bool selected;

    float height;

    Editor editor;

    Vector2 scrollPos;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        height = base.GetPropertyHeight(property, label);

        return height;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        GUIStyle style = new GUIStyle(GUI.skin.box);

        style.richText = true;

        if (selected)
            EditorGUILayout.Space(height);

        var copyPos = position;

        copyPos.width *= 0.75f;

        position.width *= 0.25f;

        position.x = copyPos.width + copyPos.x;

        EditorGUI.ObjectField(copyPos, property, label);

        if (EditorGUI.DropdownButton(position, new GUIContent(selected? "Plegar" : "Desplegar"), FocusType.Passive))
            selected = !selected;

        if (!selected)
            return;

        if(property.objectReferenceValue==null)
        {
            EditorGUILayout.LabelField("Debes de seleccionar un Scriptable".RichTextColor(Color.red), style);
            return;
        }

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, style, GUILayout.Height(height * muliplyHeight));

        editor = Editor.CreateEditor(property.objectReferenceValue);

        editor.DrawDefaultInspector();

        EditorGUILayout.EndScrollView();

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        EditorGUILayout.Space(height);
    }
}
