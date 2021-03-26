using System.Linq;
using System;
using System.Collections.Generic;

namespace DumbML {
    public class Graph<Tid, Tvalue> {
        Dictionary<Tid, GraphVertex<Tvalue>> vertices = new Dictionary<Tid, GraphVertex<Tvalue>>();
        List<GraphVertex<Tvalue>> leaves;
        List<GraphVertex<Tvalue>> roots;


        public GraphVertex<Tvalue> AddVertex(Tid id, Tvalue val) {
            if (vertices.ContainsKey(id)) {
                if (!EqualityComparer<Tvalue>.Default.Equals(vertices[id].Value, val)) {
                    throw new ArgumentException($"Graph already contains a vertex for: {id}");
                }
                else {
                    return vertices[id];
                }
            }

            var result = new GraphVertex<Tvalue>(val);
            vertices.Add(id, new GraphVertex<Tvalue>(val));
            return result;
        }
        public GraphVertex<Tvalue> GetVertex(Tid id) {
            return vertices[id];
        }

        public void AddEdge(Tid first, Tid second, bool directed = false) {
            var f = GetVertex(first);
            var s = GetVertex(second);

            if (directed) {
                f.AddEdge(s, EdgeType.Leaving);
                s.AddEdge(f, EdgeType.Arriving);
            }
            else {
                f.AddEdge(s, EdgeType.NonDirected);
                s.AddEdge(f, EdgeType.NonDirected);
            }
        }


        public IEnumerable<GraphVertex<Tvalue>> Vertices() {
            foreach (var v in vertices) {
                yield return v.Value;
            }
        }

        public IEnumerable<GraphVertex<Tvalue>> GetLeaves() {
            if(leaves == null) {
                Build();
            }
            return leaves;
        }
        public IEnumerable<GraphVertex<Tvalue>> GetRoots() {
            if (roots == null) {
                Build();
            }
            return roots;
        }

        public bool Contains(Tid id) {
            return vertices.ContainsKey(id);
        }
        public void Build() {
            leaves = new List<GraphVertex<Tvalue>>();
            roots = new List<GraphVertex<Tvalue>>();
            foreach (var v in vertices) {
                // leaves - no previous
                if (!v.Value.PreviousVertices().Any()) {
                    leaves.Add(v.Value);
                }
                // roots - no forward connections
                else if (!v.Value.NextVertices().Any()) {
                    roots.Add(v.Value);
                }
            }
        }



        public string Summary() {
            var sb = new System.Text.StringBuilder();

            foreach (var v in Vertices()) {
                sb.AppendLine($"Node '{v.Value.ToString()}'");
                foreach (var c in v.PreviousVertices()) {
                    sb.AppendLine($"{"\t"}'{c.Value.ToString()}'");
                }
            }

            sb.AppendLine("Roots");
            foreach (var v in GetRoots()) {
                sb.AppendLine($"{"\t"}'{v.Value.ToString()}'");
            }
            sb.AppendLine("Leaves");
            foreach (var v in GetLeaves()) {
                sb.AppendLine($"{"\t"}'{v.Value.ToString()}'");
            }

            return sb.ToString();
        }
    }

    public class Graph<T> : Graph<T, T> {
        public GraphVertex<T> AddVertex(T val) {
            return AddVertex(val, val);
        }
    }





    public class GraphVertex<T> {
        public T Value { get; private set; }

        public GraphVertex(T val) {
            Value = val;
        }

        List<(GraphVertex<T> v, EdgeType e)> edges = new List<(GraphVertex<T>, EdgeType)>();

        public void AddEdge(GraphVertex<T> other, EdgeType type) {
            edges.Add((other, type));
        }

        public IEnumerable<GraphVertex<T>> ConnectedVertices() {
            foreach (var e in edges) {
                yield return e.v;

            }
        }
        public IEnumerable<GraphVertex<T>> NextVertices() {
            foreach (var e in edges) {
                switch (e.e) {
                    case EdgeType.Leaving:
                    case EdgeType.NonDirected:
                        yield return e.v;
                        break;
                    case EdgeType.Arriving:
                        break;
                }

            }
        }
        public IEnumerable<GraphVertex<T>> PreviousVertices() {
            foreach (var e in edges) {
                switch (e.e) {
                    case EdgeType.Arriving:
                    case EdgeType.NonDirected:
                        yield return e.v;
                        break;
                    case EdgeType.Leaving:
                        break;
                }

            }
        }
    }



    public enum EdgeType {
        Arriving,
        Leaving,
        NonDirected        
    }
}
