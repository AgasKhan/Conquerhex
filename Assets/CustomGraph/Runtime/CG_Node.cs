
using System;
using UnityEngine;

namespace CustomGraph
{
    [Serializable]
    public class CG_Node
    {
        [SerializeField]
        string _guid;

        [SerializeField]
        Rect _position;

        public string TypeName;
        public string ID => _guid;
        public Rect NodePosition => _position;

        public CG_Node()
        {
            NewGUID();
        }

        void NewGUID()
        {
            _guid = Guid.NewGuid().ToString();
        }

        public void SetRectPosition(Rect pos)
        {
            _position = pos;
        }

        /// <summary>
        /// Metodo principal en donde se ejecuta el nodo.
        /// </summary>
        /// <param name="graph">Referencia del grafo</param>
        /// <returns>Devuelve el siguiente nodo o un string vacio</returns>
        public virtual string OnProcess(CG_AssetGraph graph)
        {
            CG_Node nextNode = graph.GetNodeFromOutput(_guid, 0);

            if (nextNode != null) return nextNode.ID;

            return string.Empty;
        }
    }
}

