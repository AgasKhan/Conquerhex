using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using CustomEulerEditor;

[CustomPropertyDrawer(typeof(object), true)]
public class CustomClassPropertyDrawer : PropertyDrawer
{
    InheterenceOrder inheterenceOrder;

    private bool CheckIfObjective(SerializedProperty property)
    {
        return property.propertyType == SerializedPropertyType.Generic && !(property.isArray || property.isFixedBuffer);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.PropertyField(position, property, label);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return base.GetPropertyHeight(property, label);
    }

    private System.Type GetObjectType(SerializedProperty property)
    {
        var fieldType = fieldInfo.FieldType;

        //if(property.isArray || property.isFixedBuffer)
        if (fieldType.IsArray)
        {
            return fieldType.GetElementType();
        }
        else if (fieldType.IsGenericType && typeof(System.Collections.IEnumerable).IsAssignableFrom(fieldType))
        {
            // Assume that we want the first generic argument type
            return fieldType.GetGenericArguments()[0];
        }
        

        // Otherwise, return the field type itself
        return fieldType;
    }

    public override VisualElement CreatePropertyGUI(SerializedProperty serializedProperty)
    {
        if(!CheckIfObjective(serializedProperty))
        {
            if(serializedProperty.propertyType == SerializedPropertyType.ObjectReference)
            {
                var aux = new ObjectField(serializedProperty.displayName)
                {
                    objectType = GetObjectType(serializedProperty),
                    bindingPath = serializedProperty.propertyPath
                };
                //aux.BindProperty(serializedProperty);
                return aux;
            }
            else
            {
                var aux = new PropertyField();
                aux.BindProperty(serializedProperty);
                return aux;
            }

        }

        var serializedPropertyChild = serializedProperty.Copy();
        serializedPropertyChild.NextVisible(true);

        inheterenceOrder = new InheterenceOrder(fieldInfo.FieldType, serializedPropertyChild);

        VisualElement main = inheterenceOrder.CreateGUI(new VisualElement());

        var foldout = new Foldout { text = serializedProperty.displayName, value = serializedProperty.isExpanded };
        foldout.RegisterValueChangedCallback(evt =>
        {
            if (evt.target != foldout)
                return;

            serializedProperty.isExpanded = evt.newValue;
        });

        foldout.Add(main);

        return foldout;
    }
}
