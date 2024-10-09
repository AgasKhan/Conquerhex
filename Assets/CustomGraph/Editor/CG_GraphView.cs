
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;


namespace CustomGraph.Editor
{
    public class CG_GraphView : GraphView
    {
        SerializedObject _serializedObject;
        CG_EditorWindow _window;
        CG_AssetGraph _graph;

        CG_WindowsSearch _searchProvider;

        public CG_EditorWindow Window => _window;
        public List<CG_NodeInEditor> GraphNodes;
        public Dictionary<string, CG_NodeInEditor> EditorNodes;
        public Dictionary<Edge, CG_FlowConnection> ConnectionsNodes;

        public CG_GraphView(SerializedObject obj, CG_EditorWindow window)
        {
            _serializedObject = obj;
            _graph = (CG_AssetGraph)obj.targetObject;
            _window = window;

            GraphNodes = new();
            EditorNodes = new();
            ConnectionsNodes = new();

            _searchProvider = ScriptableObject.CreateInstance<CG_WindowsSearch>();
            _searchProvider.Graph = this;

            this.nodeCreationRequest = ShowSearchWindow;

            StyleSheet s = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/CustomGraph/UI/USS/CG_Background.uss");
            styleSheets.Add(s);

            GridBackground background = new();
            background.name = "GridBack";
            Add(background);
            background.SendToBack();

            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(new ClickSelector());
            this.AddManipulator(new ContentZoomer());

            DrawNodes();
            DrawConnections();

            graphViewChanged += OnGraphChanged;
        }

        private void DrawConnections()
        {
            if (_graph.Connections == null) return;

            foreach (CG_FlowConnection c in _graph.Connections)
            {
                DrawConnectionSolo(c);
            }
        }

        private void DrawConnectionSolo(CG_FlowConnection connection)
        {
            CG_NodeInEditor input = GetNode(connection.Input.ID);
            CG_NodeInEditor output = GetNode(connection.Output.ID);

            if (input == null || output == null) return;

            Port inPort = input.Ports[connection.Input.PortIndex];
            Port outPort = output.Ports[connection.Output.PortIndex];

            Edge edge = inPort.ConnectTo(outPort);
            AddElement(edge);

            ConnectionsNodes.Add(edge, connection);
        }

        private CG_NodeInEditor GetNode(string ID)
        {
            CG_NodeInEditor node = null;

            EditorNodes.TryGetValue(ID, out node);
            return node;
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            List<Port> allPorts = new();
            List<Port> ports = new();

            foreach (var node in GraphNodes)
            {
                allPorts.AddRange(node.Ports);
            }

            foreach (Port p in allPorts)
            {
                if (p == startPort) continue;

                if (p.node == startPort.node) continue;

                if (p.direction == startPort.direction) continue;

                if (p.portType == startPort.portType) ports.Add(p);
            }

            return ports;
        }

        private GraphViewChange OnGraphChanged(GraphViewChange graphViewChange)
        {
            if (graphViewChange.movedElements != null)
            {
                Undo.RecordObject(_serializedObject.targetObject, "Moved Node");

                foreach (CG_NodeInEditor node in graphViewChange.movedElements.OfType<CG_NodeInEditor>())
                {
                    node.SavePosition();
                }
            }

            if (graphViewChange.elementsToRemove != null)
            {
                Undo.RecordObject(_serializedObject.targetObject, "Removed Node");

                List<CG_NodeInEditor> nodes = graphViewChange.elementsToRemove.OfType<CG_NodeInEditor>().ToList();

                if (nodes.Count > 0)
                {
                    for (int i = nodes.Count - 1; i >= 0; i--)
                    {
                        RemoveNode(nodes[i]);
                    }
                }

                foreach (Edge e in graphViewChange.elementsToRemove.OfType<Edge>())
                {
                    RemoveConnection(e);
                }
            }

            if (graphViewChange.edgesToCreate != null)
            {
                Undo.RecordObject(_serializedObject.targetObject, "Added Connection");
                foreach (Edge e in graphViewChange.edgesToCreate)
                {
                    CreateEdge(e);
                }
            }

            return graphViewChange;
        }

        private void CreateEdge(Edge edge)
        {
            CG_NodeInEditor input = (CG_NodeInEditor)edge.input.node;
            int inputIndex = input.Ports.IndexOf(edge.input);

            CG_NodeInEditor output = (CG_NodeInEditor)edge.output.node;
            int outputIndex = output.Ports.IndexOf(edge.output);

            CG_FlowConnection connection = new(input.Node.ID, inputIndex, output.Node.ID, outputIndex);
            _graph.Connections.Add(connection);
        }

        private void RemoveNode(CG_NodeInEditor node)
        {
            _graph.Nodes.Remove(node.Node);
            EditorNodes.Remove(node.Node.ID);
            GraphNodes.Remove(node);
            _serializedObject.Update();
        }

        void RemoveConnection(Edge edge)
        {
            if (ConnectionsNodes.TryGetValue(edge, out CG_FlowConnection connection))
            {
                _graph.Connections.Remove(connection);
                ConnectionsNodes.Remove(edge);
            }
        }

        void ShowSearchWindow(NodeCreationContext obj)
        {
            _searchProvider.Item = (VisualElement)focusController.focusedElement;
            SearchWindow.Open(new SearchWindowContext(obj.screenMousePosition), _searchProvider);
        }

        void DrawNodes()
        {
            foreach (CG_Node node in _graph.Nodes)
            {
                AddNodeToGraph(node);
            }

            Bind();
        }

        public void Add(CG_Node node)
        {
            Undo.RecordObject(_serializedObject.targetObject, "Added Node");

            _graph.Nodes.Add(node);

            _serializedObject.Update();

            AddNodeToGraph(node);
            Bind();
        }

        void AddNodeToGraph(CG_Node node)
        {
            node.TypeName = node.GetType().AssemblyQualifiedName;

            CG_NodeInEditor editorNode = new(node, _serializedObject);
            editorNode.SetPosition(node.NodePosition);
            GraphNodes.Add(editorNode);
            EditorNodes.Add(node.ID, editorNode);

            AddElement(editorNode);
        }
        

        void Bind()
        {
            _serializedObject.Update();
            this.Bind(_serializedObject);
        }
    }
}
