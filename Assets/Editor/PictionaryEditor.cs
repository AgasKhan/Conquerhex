using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Reflection;

namespace CustomEulerEditor
{
    public static class ExtensionPropertyDrawer
    {
        /// <summary>
        /// Extension para obtener en caso de ser generico o un array el tipo del elemento, a la par del tipo en si en caso que no
        /// </summary>
        /// <param name="fieldInfo"></param>
        /// <returns></returns>
        public static System.Type GetObjectType(this FieldInfo fieldInfo)
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

        /// <summary>
        /// Avanza en uno la propiedad
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static VisualElement GeneratePropertyFielWithoutFoldOut(this SerializedProperty property)
        {
            VisualElement container = new();
            int depth = property.depth;

            if (property.propertyType == (SerializedPropertyType.ManagedReference | SerializedPropertyType.Generic))
            {
                var label = new Label(property.displayName + ":");
                label.style.marginLeft = 3;

                container.Add(label);
                container.Add(new IMGUIContainer(() => EditorGUILayout.LabelField("", GUI.skin.horizontalSlider)));

                var margin = new VisualElement();
                container.Add(margin);
                margin.AddToClassList("unity-foldout__content");

                property.Next(true);
                do
                {
                    margin.Add(new PropertyField(property));
                }
                while (property.Next(false) && property.depth >= depth);

                container.Add(new IMGUIContainer(() => EditorGUILayout.LabelField("", GUI.skin.horizontalSlider)));
            }
            else
            {
                container.Add(new PropertyField(property));
                property.Next(false);
            }


            return container;
        }
    }


    [CustomPropertyDrawer(typeof(Internal.Pictionary<,>))]
    public class PictionaryEditor : PropertyDrawer
    {
        Foldout container;
        PropertyField propertyField;

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            container = new Foldout();
            container.text = property.displayName;

            property.Next(true);
            container.contentContainer.Add(property.GeneratePropertyFielWithoutFoldOut());
            container.contentContainer.Add(property.GeneratePropertyFielWithoutFoldOut());

            //propertyField = new PropertyField(property);

            //propertyField.RegisterCallback<GeometryChangedEvent>(AfterCreate);

            //container.Add(propertyField);

            return container;
        }
    }

    [CustomPropertyDrawer(typeof(Pictionarys<,>))]
    public class PictionarysEditor : PropertyDrawer
    {
        PropertyField container;

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            string name = property.displayName;

            property.Next(true);

            container = new PropertyField(property, name);

            return container;
        }
    }
}
