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
        //id del nodo
        public int Id { get; set; }

        //conexiones con otros nodos
        public GraphNode[] AdjacentNodes;

        //La cantidad de aristas seteadas
        public int Count;

        //conjunto al que pertenece
        public GraphGroupNode perteneciente;

        //si ya fue inicializado
        public bool init;

        public GraphNode(int id)
        {
            Id = id;
            AdjacentNodes = new GraphNode[Graph.aristasPorConjunto];
        }
    }

    public class GraphGroupNode
    {
        public List<GraphNode> conjunto = new List<GraphNode>();

        public int[] verticesDivisorios;

        public GraphGroupNode()
        {
            verticesDivisorios = new int[Graph.aristasPorConjunto];
        }
    }

    [System.Serializable]
    public class Graph
    {
        public List<GraphNode> nodes = new List<GraphNode>();

        public GraphGroupNode[] conjuntosNodes;

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
                conjuntosNodes[i] = new();

                int rng = Random.Range(1, maxNodesPerConjunto+1);

                for (int j = 0; j < rng; j++)
                {
                    conjuntosNodes[i].conjunto.Add(new GraphNode(nodes.Count));

                    conjuntosNodes[i].conjunto[j].perteneciente = conjuntosNodes[i];

                    nodes.Add(conjuntosNodes[i].conjunto[j]);
                }
            }

            for (int i = 0; i < entriesEdges.Length; i++)
            {
                int rng;

                do
                {
                    rng = Random.Range(0, conjuntosNodes.Length);
                } while(conjuntosNodes[rng].conjunto[0].init);

                entriesNodes[i] = conjuntosNodes[rng].conjunto[0];

                conjuntosNodes[rng].conjunto[0].init = true;

                AddEdge(conjuntosNodes[rng].conjunto[0], entriesEdges[i], entry);

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
                    var randomNode = otherConjunto.perteneciente.conjunto[Random.Range(0, otherConjunto.perteneciente.conjunto.Count)];

                    if (!CompatiblesConjuntos(currentNode.perteneciente.conjunto, otherConjunto.perteneciente.conjunto, out var disp1))
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
                    for (int i = 0; i < item.perteneciente.conjunto.Count; i++)
                    {
                        if (item.perteneciente.conjunto[i] == item)
                        {
                            continue;   
                        }

                        for (int j = 0; j < aristasPorConjunto; j++)
                        {
                            if (item.AdjacentNodes[j] != null)
                                item.perteneciente.conjunto[i].AdjacentNodes[j] = item.AdjacentNodes[j];

                            item.AdjacentNodes[j] = null;
                        }

                        entriesNodes[k].Count = 0;
                        entriesNodes[k] = item.perteneciente.conjunto[i];

                        break;
                    }   
                }
            }

            foreach (var nodeList in conjuntosNodes)
            {
                nodeList.conjunto.RemoveAll((node)=>node.Count==0 || node.AdjacentNodes.All((a)=>a==null));
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

            foreach (var item in conjuntosNodes)
            {
                GraphInspector.FindDividingEdges(item);
                yield return null;
            }

            GraphInspector.PrintGraph(this);
        }


        public Graph(int cantidad, int nodosMaximosPorConjunto ,params int[] entradas)
        {
            entriesNodes = new GraphNode[entradas.Length];

            entriesEdges = new int[entradas.Length];

            conjuntosNodes = new GraphGroupNode[cantidad];

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
                foreach (var node in graph.conjuntosNodes[i].conjunto)
                {
                    pantalla+=$"\nNodo\t{node.Id}: \t" + string.Join("\t", GetAdjacentNodesIds(node.AdjacentNodes));
                }
                pantalla += "\naristas divosrias";
                foreach (var item in graph.conjuntosNodes[i].verticesDivisorios)
                {
                    pantalla += $"\t{item}";
                }               
            }

            Debug.Log(pantalla);
        }

        public static bool VerifyGraph(Graph graph)
        {
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

        public static void FindDividingEdges(GraphGroupNode group)
        {
            int n = Graph.aristasPorConjunto;
            int numVertices = n;

            bool[,] adjacencyMatrix = new bool[numVertices, numVertices];

            // Inicializar la matriz de adyacencia para los vértices
            foreach (var node in group.conjunto)
            {
                for (int i = 0; i < n; i++)
                {
                    if (node.AdjacentNodes[i] != null)
                    {
                        int vertex1 = i;
                        int vertex2 = (i + 1) % n;
                        adjacencyMatrix[vertex1, vertex2] = true;
                        adjacencyMatrix[vertex2, vertex1] = true;
                    }
                }
            }

            // Identificar componentes conectados
            bool[] visited = new bool[numVertices];
            List<List<int>> components = new List<List<int>>();

            for (int i = 0; i < numVertices; i++)
            {
                if (!visited[i])
                {
                    List<int> component = new List<int>();
                    DFS(adjacencyMatrix, i, visited, component);
                    components.Add(component);
                }
            }

            // Determinar vértices divisorios
            int[] dividingVertices = new int[numVertices];
            for (int i = 0; i < numVertices; i++) dividingVertices[i] = 1; // Inicializar todos los vértices como divisores

            // Si hay más de un componente, los vértices conectados no son divisores
            if (components.Count > 1)
            {
                foreach (var component in components)
                {
                    foreach (var vertex in component)
                    {
                        dividingVertices[vertex] = 0; // Marcar vértices conectados como no divisores
                    }
                }
            }

            group.verticesDivisorios = dividingVertices;
        }

        private static void DFS(bool[,] graph, int node, bool[] visited, List<int> component)
        {
            visited[node] = true;
            component.Add(node);

            for (int neighbor = 0; neighbor < graph.GetLength(0); neighbor++)
            {
                if (graph[node, neighbor] && !visited[neighbor])
                {
                    DFS(graph, neighbor, visited, component);
                }
            }
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

