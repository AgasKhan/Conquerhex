using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Reflection;

[CustomPropertyDrawer(typeof(object), true)]
public class CustomClassPropertyDrawer : PropertyDrawer
{
    private bool showInheritedPropertiesInverted = false; // Configuración para mostrar propiedades heredadas abajo
    private bool showClassTitles = false; // Configuración para mostrar títulos de clase
    private bool showClassAllTitles = false; // Configuración para mostrar títulos de clase sin miembros

    private bool CheckIfObjective(SerializedProperty property)
    {
        return property.propertyType == SerializedPropertyType.ManagedReference ||
                (property.propertyType != SerializedPropertyType.ObjectReference &&
                property.propertyType != SerializedPropertyType.Enum &&
                property.propertyType != SerializedPropertyType.Vector2Int &&
                property.propertyType != SerializedPropertyType.Vector2 &&
                property.propertyType != SerializedPropertyType.Vector3 &&
                property.propertyType != SerializedPropertyType.Vector3Int &&
                property.propertyType != SerializedPropertyType.Vector4 &&
                property.propertyType != SerializedPropertyType.ArraySize &&
                property.propertyType != SerializedPropertyType.FixedBufferSize &&
                property.propertyType != SerializedPropertyType.Hash128 &&
                property.propertyType != SerializedPropertyType.Quaternion &&
                property.propertyType != SerializedPropertyType.Rect &&
                property.propertyType != SerializedPropertyType.Float &&
                property.propertyType != SerializedPropertyType.String &&
                property.propertyType != SerializedPropertyType.Integer &&
                property.propertyType != SerializedPropertyType.LayerMask &&
                property.propertyType != SerializedPropertyType.BoundsInt &&
                property.propertyType != SerializedPropertyType.Bounds &&
                property.propertyType != SerializedPropertyType.Color &&
                property.propertyType != SerializedPropertyType.Gradient);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (!CheckIfObjective(property))
        {
            EditorGUI.PropertyField(position, property, label, true);
            return;
        }

        // Configuración del foldout
        var foldoutRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, label);

        if (property.isExpanded)
        {
            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.indentLevel++; // El identado se aplica para todo lo que este dentro del Foldout

            // Añadir opciones de configuración en la parte superior
            Rect toggleRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight);
            showInheritedPropertiesInverted = EditorGUI.Toggle(toggleRect, "Mostrar propiedades heredadas abajo", showInheritedPropertiesInverted);
            toggleRect.y += EditorGUIUtility.singleLineHeight;
            showClassTitles = EditorGUI.Toggle(toggleRect, "Mostrar títulos de clase", showClassTitles);

            if (showClassTitles)
            {
                toggleRect.y += EditorGUIUtility.singleLineHeight;
                showClassAllTitles = EditorGUI.Toggle(toggleRect, "Mostrar títulos de clase sin miembros", showClassAllTitles);
            }

            var childProperty = property.Copy();
            var depth = property.depth;

            List<SerializedProperty> allProperties = new List<SerializedProperty>();
            List<(Type tipo, List<SerializedProperty> lista)> types = new List<(Type, List<SerializedProperty>)>();

            // Obtener el tipo del objeto
            Type type = fieldInfo.FieldType;

            // Obtener todos los tipos de la jerarquía de herencia
            Type baseType = type;
            while (baseType != null && baseType != typeof(System.Object))
            {
                types.Insert(0, (baseType, new List<SerializedProperty>()));
                baseType = baseType.BaseType;
            }

            // Obtener todas las propiedades serializadas
            while (childProperty.NextVisible(true) && childProperty.depth > depth)
            {
                if (childProperty.depth == depth + 1) // FIX propiedades duplicadas
                    allProperties.Add(childProperty.Copy());
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
            toggleRect.y = toggleRect.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            if (!showInheritedPropertiesInverted)
            {
                for (int i = 0; i < types.Count; i++)
                {
                    toggleRect.y = DrawProperties(position, types[i].lista, types[i].tipo, toggleRect.y);
                }
            }
            else
            {
                for (int i = types.Count - 1; i >= 0; i--)
                {
                    toggleRect.y = DrawProperties(position, types[i].lista, types[i].tipo, toggleRect.y);
                }
            }

            EditorGUI.indentLevel--;
            EditorGUI.EndProperty();
        }
    }

    private float DrawProperties(Rect position, List<SerializedProperty> properties, Type type, float yOffset)
    {
        if (showClassTitles && (properties.Count > 0 || showClassAllTitles))
        {
            yOffset += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            var titleRect = new Rect(position.x, yOffset, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(titleRect, "", GUI.skin.horizontalSlider);

            yOffset += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            var labelRect = new Rect(position.x, yOffset, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(labelRect, type.Name, EditorStyles.boldLabel);

            if (ScriptAssetProcessor.HasInstanceID(type))
            {
                var buttonRect = new Rect(position.x + position.width - 100, yOffset, 100, EditorGUIUtility.singleLineHeight);
                GUIContent scriptIcon = EditorGUIUtility.IconContent("cs Script Icon");
                if (GUI.Button(buttonRect, scriptIcon))
                {
                    var id = ScriptAssetProcessor.GetInstanceID(type);
                    AssetDatabase.OpenAsset(id.ID, id.line);
                }
            }

            yOffset += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        }

        foreach (SerializedProperty property in properties)
        {
            var propertyRect = new Rect(position.x, yOffset, position.width, EditorGUI.GetPropertyHeight(property, true));
            EditorGUI.PropertyField(propertyRect, property, true);
            yOffset += EditorGUI.GetPropertyHeight(property, true) + EditorGUIUtility.standardVerticalSpacing;
        }

        return yOffset;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (!CheckIfObjective(property))
        {
            return EditorGUIUtility.singleLineHeight;
        }

        float height = EditorGUIUtility.singleLineHeight; // Para el foldout

        if (property.isExpanded)
        {
            height += EditorGUIUtility.singleLineHeight * 2; // Para los dos toggles principales
            if (showClassTitles)
            {
                height += EditorGUIUtility.singleLineHeight; // Para el toggle de mostrar títulos de clase sin miembros
            }

            var childProperty = property.Copy();
            var depth = property.depth;

            List<SerializedProperty> allProperties = new List<SerializedProperty>();
            List<(Type tipo, List<SerializedProperty> lista)> types = new List<(Type, List<SerializedProperty>)>();

            // Obtener el tipo del objeto
            Type type = fieldInfo.FieldType;

            // Obtener todos los tipos de la jerarquía de herencia
            Type baseType = type;
            while (baseType != null && baseType != typeof(System.Object))
            {
                types.Insert(0, (baseType, new List<SerializedProperty>()));
                baseType = baseType.BaseType;
            }

            // Obtener todas las propiedades serializadas
            while (childProperty.NextVisible(true) && childProperty.depth > depth)
            {
                if (childProperty.depth == depth + 1) // FIX propiedades duplicadas
                    allProperties.Add(childProperty.Copy());
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

            for (int i = 0; i < types.Count; i++)
            {
                if (showClassTitles && (types[i].lista.Count > 0 || showClassAllTitles))
                {
                    height += 3 * EditorGUIUtility.singleLineHeight + 3 * EditorGUIUtility.standardVerticalSpacing; // Para el título de la clase y dos espacios verticales
                }

                foreach (SerializedProperty item in types[i].lista)
                {
                    height += EditorGUI.GetPropertyHeight(item, true) + EditorGUIUtility.standardVerticalSpacing;
                }
            }
        }

        return height;
    }
}
