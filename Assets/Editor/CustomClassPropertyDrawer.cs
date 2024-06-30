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

    public override VisualElement CreatePropertyGUI(SerializedProperty serializedProperty)
    {
        if(!CheckIfObjective(serializedProperty))
        {
            var aux = new PropertyField();
            aux.BindProperty(serializedProperty);
            return aux;
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
