using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace GraphNodesLibrary
{
    public class GraphsNodesScripts : MonoBehaviour
    {
        [SerializeField]
        int cantidad;

        [SerializeField]
        int[] entradas;

        [SerializeField, Range(1,Graph.aristasPorConjunto)]
        int nodosMaximos;

        [SerializeReference]
        Graph graph;

        [ContextMenu("Generar")]

        void Generar()
        {
            graph = new Graph(cantidad, nodosMaximos ,entradas);
            StartCoroutine(graph.Calculate());
        }

        [ContextMenu("Comprobar")]
        void Verificar()
        {
            GraphInspector.PrintGraph(graph);
            GraphInspector.VerifyGraph(graph);
        }
    }

    public class GraphNode
    {
        public int Id { get; set; }

        public GraphNode[] AdjacentNodes;

        //La cantidad de aristas seteadas
        public int Count;

        public List<GraphNode> conjunto;

        public bool init;

        public GraphNode(int id)
        {
            Id = id;
            AdjacentNodes = new GraphNode[Graph.aristasPorConjunto];
        }
    }

    [System.Serializable]
    public class Graph
    {
        public List<GraphNode> nodes = new List<GraphNode>();

        public List<GraphNode>[] conjuntosNodes;

        public GraphNode[] entriesNodes;

        public int[] entriesEdges;

        public int[] edgesDisponibles;

        public const int aristasPorConjunto = 6;

        int maxNodesPerConjunto;

        /// <summary>
        /// auxiliar para setear un nodo salida que luego sera remplazado en cada caso
        /// </summary>
        GraphNode entry = new GraphNode(-1);

        int LadoOpuesto(int edge)
        {
            return ((edge - (aristasPorConjunto / 2)) >= 0) ? (edge - (aristasPorConjunto / 2)) : (edge + (aristasPorConjunto / 2));
        }

        List<int> CalcularEdgesDisponibles(List<GraphNode> graphNodes)
        {
            List<int> vs = new List<int>();

            for (int i = 0; i < aristasPorConjunto; i++)
            {
                vs.Add(i);
            }

            for (int i = 0; i < graphNodes.Count; i++)
            {
                for (int j = 0; j < graphNodes[i].AdjacentNodes.Length; j++)
                {
                    if (graphNodes[i].AdjacentNodes[j] != null)
                    {
                        vs.Remove(j);
                    }
                }
            }

            return vs;
        }


        //vincula el lado del nodo 1, con el lado opuesto del nodo2
        void AddEdge(GraphNode node1, int edge, GraphNode node2)
        {
            node1.AdjacentNodes[edge] = node2;
            node1.Count++;

            node2.AdjacentNodes[LadoOpuesto(edge)] = node1;
            node2.Count++;

            edgesDisponibles[edge]--;
            edgesDisponibles[LadoOpuesto(edge)]--;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="graphNodeParent"></param>
        /// <param name="graphNodesToConect"></param>
        /// <param name="dispo1">Lista de todos los edges compatibles entre ambos</param>
        /// <returns>Si son compatible los conjuntos para su conexion</returns>
        bool CompatiblesConjuntos(List<GraphNode> graphNodeParent, List<GraphNode> graphNodesToConect, out List<int> dispo1)
        {
            dispo1 = CalcularEdgesDisponibles(graphNodeParent);
            var dispo2 = CalcularEdgesDisponibles(graphNodesToConect);
            
            for (int i = dispo1.Count - 1; i >= 0; i--)    
            {
                bool quitar = true;
                for (int j = 0; j < dispo2.Count; j++)
                {
                    if ((edgesDisponibles[dispo1[i]] > 0 && edgesDisponibles[LadoOpuesto(dispo2[j])] > 0) && dispo1[i] == LadoOpuesto(dispo2[j]))
                    {
                        quitar = false;
                    }
                }
                if(quitar)
                    dispo1.RemoveAt(i);
            }

            return dispo1.Count>0;
        }

        public IEnumerator Calculate()
        {
            nodes.Clear();

            for (int i = 0; i < aristasPorConjunto; i++)
            {
                edgesDisponibles[i] = conjuntosNodes.Length;
            }

            //lleno el array
            for (int i = 0; i < conjuntosNodes.Length; i++)
            {
                conjuntosNodes[i] = new List<GraphNode>();

                int rng = Random.Range(1, maxNodesPerConjunto+1);

                for (int j = 0; j < rng; j++)
                {
                    conjuntosNodes[i].Add(new GraphNode(nodes.Count));

                    conjuntosNodes[i][j].conjunto = conjuntosNodes[i];

                    nodes.Add(conjuntosNodes[i][j]);
                }
            }

            for (int i = 0; i < entriesEdges.Length; i++)
            {
                int rng;

                do
                {
                    rng = Random.Range(0, conjuntosNodes.Length);
                } while(conjuntosNodes[rng][0].init);

                entriesNodes[i] = conjuntosNodes[rng][0];

                conjuntosNodes[rng][0].init = true;

                AddEdge(conjuntosNodes[rng][0], entriesEdges[i], entry);

            }

            Queue<GraphNode> queue = new Queue<GraphNode>();
            HashSet<GraphNode> visited = new HashSet<GraphNode>();

            var nodeOrdered = nodes/*.OrderBy((n) => n.GetHashCode())*/;

            foreach (var node in nodeOrdered)
            {
                queue.Enqueue(node);
                visited.Add(node);
            }

            yield return null;

            while (queue.Count > 0)
            {
                var currentNode = queue.Dequeue();

                foreach (var otherConjunto in nodeOrdered.OrderBy((n) => n.GetHashCode()))
                {
                    var randomNode = otherConjunto.conjunto[Random.Range(0, otherConjunto.conjunto.Count)];

                    if (!CompatiblesConjuntos(currentNode.conjunto, otherConjunto.conjunto, out var disp1))
                    {
                        continue;
                    }

                    AddEdge(currentNode, disp1[Random.Range(0, disp1.Count)], randomNode);

                    if (!visited.Contains(randomNode))
                    {
                        queue.Enqueue(randomNode);
                        visited.Add(randomNode);
                    }
                }

            }

            for (int k = 0; k < entriesNodes.Length; k++)
            {
                var item = entriesNodes[k];

                if (item.Count < 2)
                {
                    for (int i = 0; i < item.conjunto.Count; i++)
                    {
                        if (item.conjunto[i] == item)
                        {
                            continue;   
                        }

                        for (int j = 0; j < aristasPorConjunto; j++)
                        {
                            if (item.AdjacentNodes[j] != null)
                                item.conjunto[i].AdjacentNodes[j] = item.AdjacentNodes[j];

                            item.AdjacentNodes[j] = null;
                        }

                        entriesNodes[k].Count = 0;
                        entriesNodes[k] = item.conjunto[i];

                        break;
                    }   
                }
            }

            foreach (var nodeList in conjuntosNodes)
            {
                nodeList.RemoveAll((node)=>node.Count==0 || node.AdjacentNodes.All((a)=>a==null));
            }

            nodes.RemoveAll((node) => node.Count == 0 || node.AdjacentNodes.All((a) => a == null));

            yield return null;

            
            if (!GraphInspector.VerifyGraph(this))
            {
                foreach (var item in nodes)
                {
                    item.Count = 0;
                    for (int i = 0; i < item.AdjacentNodes.Length; i++)
                    {
                        item.AdjacentNodes[i] = null;
                    }
                }
                yield return Calculate();
                yield break;
            }
            

            GraphInspector.PrintGraph(this);
        }


        public Graph(int cantidad, int nodosMaximosPorConjunto ,params int[] entradas)
        {
            entriesNodes = new GraphNode[entradas.Length];

            entriesEdges = new int[entradas.Length];

            conjuntosNodes = new List<GraphNode>[cantidad];

            edgesDisponibles = new int[aristasPorConjunto];

            maxNodesPerConjunto = Mathf.Clamp(nodosMaximosPorConjunto,1,aristasPorConjunto);

            for (int i = 0; i < entradas.Length; i++)
            {
                entriesEdges[i] = entradas[i];
            }
        }
    }


    public class GraphInspector
    {
        public static void PrintGraph(Graph graph)
        {
            string pantalla = ("==== Grafo ====");
            for (int i = 0; i < graph.conjuntosNodes.Length; i++)
            {
                pantalla += $"\n\nConjunto {i}:";
                foreach (var node in graph.conjuntosNodes[i])
                {
                    pantalla+=$"\nNodo\t{node.Id}: \t" + string.Join("\t", GetAdjacentNodesIds(node.AdjacentNodes));
                } 
            }

            Debug.Log(pantalla);
        }

        public static bool VerifyGraph(Graph graph)
        {
            // Verificar cantidad de aristas por conjunto
            /*
            foreach (var conjunto in graph.conjuntosNodes)
            {
                int count = 0;
                int id=-1;

                foreach (var node in conjunto)
                {
                    count += node.Count;
                    id = node.Id;
                }

                if (count != Graph.aristasPorConjunto)
                {
                    Debug.LogError($"Error: El conjunto {id} tiene {count} aristas en lugar de {Graph.aristasPorConjunto}");
                    //                        return false;
                }
            }
            */

            HashSet<GraphNode> visited = new HashSet<GraphNode>();
            Queue<GraphNode> queue = new Queue<GraphNode>();

            queue.Enqueue(graph.entriesNodes[0]);
            visited.Add(graph.entriesNodes[0]);

            while (queue.Count > 0)
            {
                var currentNode = queue.Dequeue();

                foreach (var neighbor in currentNode.AdjacentNodes)
                {
                    if (neighbor != null && neighbor.Id != -1  && !visited.Contains(neighbor))
                    {
                        queue.Enqueue(neighbor);
                        visited.Add(neighbor);
                    }
                }
            }

            if (visited.Count < graph.nodes.Count)
            {
                Debug.LogError($"Error: nodos no visitados\nvisitados: {visited.Count} / Totales: {graph.nodes.Count}");
                return false; // Si algún nodo no fue visitado, el grafo no está conectado correctamente
            }

            return true; // Si todos los nodos fueron visitados, el grafo está conectado correctamente
        }

        private static string[] GetAdjacentNodesIds(GraphNode[] adjacentNodes)
        {
            string[] ids = new string[adjacentNodes.Length];
            for (int i = 0; i < adjacentNodes.Length; i++)
            {
                ids[i] = adjacentNodes[i] != null ? adjacentNodes[i].Id.ToString() : "-";
            }
            return ids;
        }
    }
}

