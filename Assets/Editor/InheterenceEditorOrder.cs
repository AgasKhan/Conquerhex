using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using Object = UnityEngine.Object;

[CustomEditor(typeof(Object), true)]
[CanEditMultipleObjects]
public class InheterenceEditorOrder : Editor
{
    private bool showInheritedPropertiesInverted = false; // Configuración para mostrar propiedades heredadas abajo
    private bool showClassTitles = false; // Configuración para mostrar títulos de clase
    private bool showClassAllTitles = false; // Configuración para mostrar títulos de clase

    public override void OnInspectorGUI()
    {
        // Mostrar propiedad m_Script primero y como no modificable
        SerializedProperty scriptProperty = serializedObject.FindProperty("m_Script");
        if (scriptProperty != null)
        {
            // Mostrar el nombre del script
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(scriptProperty);
            EditorGUI.EndDisabledGroup();
        }

        // Añadir opciones de configuración en la parte superior
        EditorGUILayout.BeginVertical("box");
        showInheritedPropertiesInverted = EditorGUILayout.Toggle("Mostrar propiedades heredadas abajo", showInheritedPropertiesInverted);
        showClassTitles = EditorGUILayout.Toggle("Mostrar títulos de clase", showClassTitles);

        if(showClassTitles)
            showClassAllTitles = EditorGUILayout.Toggle("Mostrar títulos de clase sin miembros", showClassAllTitles);

        EditorGUILayout.EndVertical();

        // Obtener el tipo del objeto
        Type type = target.GetType();

        List<SerializedProperty> allProperties = new List<SerializedProperty>();
        List<(Type tipo, List<SerializedProperty> lista)> types = new List<(Type, List<SerializedProperty>)>();

        // Obtener todos los tipos de la jerarquía de herencia
        {
            Type baseType = type;
            while (baseType != null && baseType != typeof(System.Object))
            {
                types.Insert(0, (baseType, new List<SerializedProperty>()));
                baseType = baseType.BaseType;
            }
        }

        // Obtener todas las propiedades serializadas
        {
            SerializedProperty property = serializedObject.GetIterator();
            if (property.NextVisible(true))
            {
                do
                {
                    if (property.propertyPath != "m_Script")
                        allProperties.Add(property.Copy());

                } while (property.NextVisible(false));
            }
        }

        // Clasificar propiedades por tipo
        for (int i = 0; i < types.Count; i++)
        {
            FieldInfo[] fields = types[i].tipo.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            foreach (var field in fields)
            {
                if (field.IsPrivate && !(field.IsDefined(typeof(SerializeField), true) || field.IsDefined(typeof(SerializeReference), true)))
                    continue;

                for (int ii = 0; ii < allProperties.Count; ii++)
                {
                    if (allProperties[ii].name == field.Name)
                    {
                        types[i].lista.Add(allProperties[ii]);
                        allProperties.RemoveAt(ii);
                        break;
                    }
                }
            }
        }

        // Añadir las propiedades no asignadas al último tipo (el tipo actual)
        if (types.Count > 0)
        {
            types[types.Count - 1].lista.AddRange(allProperties);
        }

        // Dibujar propiedades según configuración
        if (!showInheritedPropertiesInverted)
        {
            for (int i = 0; i < types.Count; i++)
            {
                DrawProperties(types[i].lista, types[i].tipo);
            }
        }
        else
        {
            for (int i = types.Count - 1; i >= 0; i--)
            {
                DrawProperties(types[i].lista, types[i].tipo);
            }
        }

        // Aplicar los cambios
        serializedObject.ApplyModifiedProperties();
    }

    private void DrawProperties(List<SerializedProperty> properties, Type type)
    {
        if (properties.Count > 0)
        {
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        }

        if (showClassTitles)
        {
            if(properties.Count > 0 || showClassAllTitles)
            {
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField(type.Name, EditorStyles.boldLabel);

                if(ScriptAssetProcessor.HasInstanceID(type))
                {
                    GUIContent scriptIcon = EditorGUIUtility.IconContent("cs Script Icon"); // Icono predeterminado, puedes cambiarlo según tus preferencias
                    if (GUILayout.Button(scriptIcon, GUILayout.Width(100), GUILayout.Height(20)))
                    {
                        // Abrir el script asociado

                        var id = ScriptAssetProcessor.GetInstanceID(type);

                        AssetDatabase.OpenAsset(id.ID, id.line);
                    }
                }


                EditorGUILayout.EndHorizontal();
            }
        }

        foreach (SerializedProperty property in properties)
        {
            EditorGUILayout.PropertyField(property, true);
        }
    }
}