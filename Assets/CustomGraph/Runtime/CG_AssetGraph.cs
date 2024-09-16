
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CustomGraph
{
    [CreateAssetMenu(menuName = "Generic Node Graph / New Graph", fileName = "New Node Graph")]
    public class CG_AssetGraph : ScriptableObject
    {
        [SerializeReference]
        List<CG_Node> _nodes;

        [SerializeField]
        List<CG_FlowConnection> _connections;

        Dictionary<string, CG_Node> _nodeDict;

        public List<CG_Node> Nodes => _nodes;
        public List<CG_FlowConnection> Connections => _connections;

        public CG_AssetGraph()
        {
            _nodes = new();
            _connections = new();
        }

        /// <summary>
        /// Metodo para crear una copia del grafo asi poder ejecutarlo. Importante llamarlo en Awake o Enable.
        /// </summary>
        /// <returns>El grafo clonado</returns>
        public CG_AssetGraph Clone()
        {
            CG_AssetGraph clone = Instantiate(this);
            clone.Init();
            return clone;
        }

        void Init()
        {
            _nodeDict = new();

            foreach (CG_Node node in Nodes)
            {
                _nodeDict.Add(node.ID, node);
            }
        }

        public CG_Node GetNode(string nextNode)
        {
            if (_nodeDict.TryGetValue(nextNode, out CG_Node node))
                return node;

            return null;
        }

        public CG_Node GetNode(CG_Node[] nodes)
        {
            if (nodes.Length == 0)
            {
                Debug.LogError("El nodo es nulo");
                return null;
            }

            return nodes[0];
        }

        /// <summary>
        /// Ejecuta la cadena de acciones con el nodo Start.
        /// </summary>
        public void OnStart()
        {
            CG_Node startNode = GetNode(Nodes.OfType<ND_OnStart>().ToArray());
            NextNode(startNode);
        }

        /// <summary>
        /// Ejecuta la cadena de acciones con el nodo Update
        /// </summary>
        public void OnUpdate()
        {
            CG_Node update = GetNode(Nodes.OfType<ND_OnUpdate>().ToArray());
            NextNode(update);
        }

        /// <summary>
        /// Ejecuta la cadena de acciones con el nodo Exit
        /// </summary>
        public void OnExit()
        {
            CG_Node exit = GetNode(Nodes.OfType<ND_OnExit>().ToArray());
            NextNode(exit);
        }


        /// <summary>
        /// Metodo para pasar al siguiente nodo. Este metodo hace recursion hasta llegar al nodo final.
        /// </summary>
        /// <param name="currentNode">El nodo actual</param>
        void NextNode(CG_Node currentNode)
        {
            string nextNode = currentNode.OnProcess(this);

            if (!string.IsNullOrEmpty(nextNode))
            {
                CG_Node node = GetNode(nextNode);
                NextNode(node);
            }
        }


        public CG_Node GetNodeFromOutput(string ID, int index)
        {
            foreach (CG_FlowConnection connection in _connections)
            {
                if (connection.Output.ID == ID && connection.Output.PortIndex == index)
                {
                    string nodeID = connection.Input.ID;
                    CG_Node input = _nodeDict[nodeID];
                    return input;
                }
            }

            return null;
        }
    }

}