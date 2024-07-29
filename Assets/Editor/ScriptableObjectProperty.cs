using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace CustomEulerEditor
{
    [CustomPropertyDrawer(typeof(ScriptableObject), true)]
    public class ScriptableObjectProperty : PropertyDrawer
    {
        const float multiplyHeight = 10;
        bool selected;
        Editor editor;
        VisualElement container;
        VisualElement contentContainer;
        Button toggleButton;
        ObjectField objectField;

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

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.ObjectField(position, property, label);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label);
        }

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            container = new VisualElement();
            contentContainer = new VisualElement();

            objectField = new ObjectField(property.displayName)
            {
                objectType = GetObjectType(property),
                value = property.objectReferenceValue,
                bindingPath = property.propertyPath
            };
            objectField.style.width = new Length(67, LengthUnit.Percent);

            objectField.RegisterValueChangedCallback(evt =>
            {
                property.objectReferenceValue = evt.newValue as ScriptableObject;
                property.serializedObject.ApplyModifiedProperties();
                UpdateEditor(property);
                UpdateContent(property);
            });

            toggleButton = new Button
            {
                text = "Desplegar"
            };
            toggleButton.style.width = new Length(33, LengthUnit.Percent);

            toggleButton.clicked += () =>
            {
                selected = !selected;
                toggleButton.text = selected ? "Plegar" : "Desplegar";
                UpdateContent(property);
            };

            var row = new VisualElement { style = { flexDirection = FlexDirection.Row } };
            row.Add(objectField);
            row.Add(toggleButton);

            container.Add(row);
            container.Add(contentContainer);

            if (property.objectReferenceValue != null)
            {
                UpdateEditor(property);
            }

            if (selected)
                UpdateContent(property);

            return container;
        }

        private void UpdateEditor(SerializedProperty property)
        {
            if (property.objectReferenceValue != null)
            {
                editor = Editor.CreateEditor(property.objectReferenceValue);
            }
        }

        private void UpdateContent(SerializedProperty property)
        {
            contentContainer.Clear();
            if (selected)
                if (property.objectReferenceValue != null)
                {
                    if (editor != null)
                    {
                        var scrollView = new ScrollView
                        {
                            style = { height = multiplyHeight * EditorGUIUtility.singleLineHeight }
                        };

                        var inspector = editor.CreateInspectorGUI();
                        inspector.style.backgroundColor = new StyleColor(new Color(0.2f, 0.2f, 0.2f, 1));
                        inspector.style.paddingBottom = 10;
                        inspector.style.paddingLeft = 10;
                        inspector.style.paddingRight = 10;
                        inspector.style.paddingTop = 10;

                        scrollView.Add(inspector);
                        scrollView.style.paddingBottom = 10;
                        scrollView.style.paddingLeft = 10;
                        scrollView.style.paddingRight = 10;
                        scrollView.style.paddingTop = 10;

                        contentContainer.Add(scrollView);
                    }
                }
                else
                {
                    contentContainer.Add(new Label("<color=red>Debes de seleccionar un Scriptable</color>"));
                }
        }
    }
}

