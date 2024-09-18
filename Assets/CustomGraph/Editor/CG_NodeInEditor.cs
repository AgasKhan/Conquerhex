using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;

namespace CustomGraph.Editor
{
    public class CG_NodeInEditor : UnityEditor.Experimental.GraphView.Node
    {
        CG_Node _node;
        Port _output;

        List<Port> _ports;
        SerializedProperty _serializedProperty;
        SerializedObject _serializedObject;
        public CG_Node Node => _node;
        public List<Port> Ports => _ports;

        public CG_NodeInEditor(CG_Node node, SerializedObject obj)
        {
            this.AddToClassList("node-graph");

            _node = node;
            _serializedObject = obj;

            Type typeInfo = node.GetType();
            InfoAttribute att = typeInfo.GetCustomAttribute<InfoAttribute>();

            title = att.Title;

            _ports = new();

            string[] depths = att.MenuItem.Split('/');

            foreach (string d in depths)
            {
                this.AddToClassList(d.ToLower().Replace(' ', '-'));
            }

            this.name = typeInfo.Name;

            if (att.hasFlowOutput)
                CreateFlowOutput();

            if (att.hasFlowInput)
                CreateFlowInput();

            foreach (FieldInfo property in typeInfo.GetFields())
            {
                if (property.GetCustomAttribute<ExposedPropertyAttribute>() is ExposedPropertyAttribute exposed)
                {
                    PropertyField field = DrawPropertyField(property.Name);
                }
            }

            RefreshExpandedState();
        }

        void FetchSerializedProperty()
        {
            SerializedProperty nodes = _serializedObject.FindProperty("_nodes");

            if (nodes.isArray)
            {
                int size = nodes.arraySize;

                for (int i = 0; i < size; i++)
                {
                    var elem = nodes.GetArrayElementAtIndex(i);
                    var elemID = elem.FindPropertyRelative("_guid");

                    if (elemID.stringValue == _node.ID) _serializedProperty = elem;
                }
            }
        }

        private PropertyField DrawPropertyField(string value)
        {
            if (_serializedProperty == null)
                FetchSerializedProperty();

            SerializedProperty property = _serializedProperty.FindPropertyRelative(value);
            PropertyField field = new(property);
            field.bindingPath = property.propertyPath;
            extensionContainer.Add(field);

            return field;
        }

        private void CreateFlowOutput()
        {
            _output = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(PortTypes.FlowPort));
            _output.portName = "Output";
            _output.tooltip = "Output";
            _ports.Add(_output);
            outputContainer.Add(_output);
        }

        private void CreateFlowInput()
        {
            Port input = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(PortTypes.FlowPort));
            input.portName = "Input";
            input.tooltip = "Input";
            _ports.Add(input);
            inputContainer.Add(input);
        }

        public void SavePosition()
        {
            _node.SetRectPosition(GetPosition());
        }
    }

    public class PortTypes
    {
        public class FlowPort { };
    }
}
