using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using Object = UnityEngine.Object;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using CustomEulerEditor;

[CustomEditor(typeof(Object), true)]
[CanEditMultipleObjects]

public class InheterenceEditorOrder : Editor
{
    InheterenceOrder inheterenceOrder;

    public override VisualElement CreateInspectorGUI()
    {
        var main = new VisualElement();

        var serialized = serializedObject.GetIterator();

        // Mostrar propiedad m_Script primero y como no modificable
        SerializedProperty scriptProperty = serializedObject.FindProperty("m_Script");
        if (scriptProperty != null)
        {
            var scriptField = new PropertyField(scriptProperty, "Script");
            scriptField.SetEnabled(false);
            main.Add(scriptField);
        }

        if(serialized.NextVisible(true))
        {
            inheterenceOrder = new InheterenceOrder(target.GetType(), serialized);
            inheterenceOrder.CreateGUI(main);
        }
        

        return main;
    }
}

namespace CustomEulerEditor
{
    public class InheterenceOrder
    {
        private bool showInheritedPropertiesInverted = false; // Configuración para mostrar propiedades heredadas abajo
        private bool showClassTitles = false; // Configuración para mostrar títulos de clase
        private bool showClassAllTitles = false; // Configuración para mostrar títulos de clase

        private VisualElement container;

        Type type;
        SerializedProperty serializedProperty;

        List<SerializedProperty> allProperties;
        List<(Type tipo, List<SerializedProperty> lista)> types;

        public InheterenceOrder(Type type, SerializedProperty serializedObject)
        {
            this.type = type;
            this.serializedProperty = serializedObject;
        }

        public VisualElement CreateGUI(VisualElement main)
        {
            if (serializedProperty == null)
                return main;

            allProperties = new List<SerializedProperty>();
            types = new List<(Type, List<SerializedProperty>)>();
            container = new VisualElement();

            var configContainer = new VisualElement();

            configContainer.style.flexDirection = FlexDirection.Row;
            configContainer.style.justifyContent = Justify.SpaceBetween;
            configContainer.style.backgroundColor = new Color(0.18f, 0.18f, 0.18f,1);
            configContainer.style.marginBottom = 5;

            main.Add(configContainer);

            main.Add(container);

            //configContainer.style.marginBottom = 10;

            var showInheritedToggle = new Button(() =>
            {
                showInheritedPropertiesInverted = !showInheritedPropertiesInverted;
                AddPropertiesToContainer();
            }) { text = "Invert Inheterence"};

            var showClassAllTitlesToggle = new Button(() =>
            {
                showClassAllTitles = !showClassAllTitles;
                AddPropertiesToContainer();
            }) { text = "Show All"};
            

            var showClassTitlesToggle = new Button(() =>
            {
                showClassTitles = !showClassTitles;
                if (showClassTitles)
                {
                    showClassAllTitlesToggle.SetEnabled(true);
                }
                else
                {
                    showClassAllTitlesToggle.SetEnabled(false);
                }
                AddPropertiesToContainer();
            }) { text = "Show Classes"};

            showClassAllTitlesToggle.SetEnabled(showClassTitles);

            Init();

            configContainer.Add(showInheritedToggle);
            configContainer.Add(showClassTitlesToggle);
            configContainer.Add(showClassAllTitlesToggle);

            AddPropertiesToContainer();

            return main;
        }

        private void Init()
        {
            Type baseType = type;
            while (baseType != null && baseType != typeof(System.Object))
            {
                types.Insert(0, (baseType, new List<SerializedProperty>()));
                baseType = baseType.BaseType;
            }

            SerializedProperty property = serializedProperty.Copy();
            int depth = serializedProperty.depth;

            // Obtener todas las propiedades serializadas
            do
            {
                if (property.propertyPath != "m_Script") // FIX propiedades duplicadas
                    allProperties.Add(property.Copy());
            }
            while (property.NextVisible(false) && property.depth == depth);


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

            if (types.Count > 0)
            {
                types[types.Count - 1].lista.AddRange(allProperties);
            }

            //Debug.Log($"");
        }

        void AddPropertiesToContainer()
        {
            container.Clear();

            if (!showInheritedPropertiesInverted)
            {
                for (int i = 0; i < types.Count; i++)
                {
                    AddPropertiesToContainer(types[i].lista, types[i].tipo);
                }
            }
            else
            {
                for (int i = types.Count - 1; i >= 0; i--)
                {
                    AddPropertiesToContainer(types[i].lista, types[i].tipo);
                }
            }
        }

        void AddPropertiesToContainer(List<SerializedProperty> properties, Type type)
        {
            //Debug.Log($"{type.Name} {properties.Count}");
            if(!showClassTitles && properties.Count > 0)
            {
                // Añadir línea separadora
                container.Add(new IMGUIContainer(() => EditorGUILayout.LabelField("", GUI.skin.horizontalSlider)));
            }
            else if (showClassTitles && (properties.Count > 0 || showClassAllTitles))
            {
                // Contenedor para el título y el botón
                container.Add(new IMGUIContainer(() => EditorGUILayout.LabelField("", GUI.skin.horizontalSlider)));
                VisualElement titleSection = new VisualElement();
                titleSection.style.flexDirection = FlexDirection.Row;
                titleSection.style.justifyContent = Justify.SpaceBetween;

                // Añadir el título
                Label title = new Label { text = type.Name };
                title.style.unityFontStyleAndWeight = FontStyle.Bold;
                titleSection.Add(title);

                // Añadir botón para abrir el script si corresponde
                if (ScriptAssetProcessor.HasInstanceID(type))
                {
                    Button openScriptButton = new Button(() =>
                    {
                        var id = ScriptAssetProcessor.GetInstanceID(type);
                        AssetDatabase.OpenAsset(id.ID, id.line);
                    })
                    {
                        text = "Open"
                    };
                    titleSection.Add(openScriptButton);
                }

                container.Add(titleSection);
            }

            // Añadir las propiedades
            for (int i = 0; i < properties.Count; i++)
            {
                var propertyField = new PropertyField();
                propertyField.BindProperty(properties[i]);
                container.Add(propertyField);
            }
        }
    }

}
