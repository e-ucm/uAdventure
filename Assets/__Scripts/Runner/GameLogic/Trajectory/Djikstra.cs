using System;
using System.Collections.Generic;

namespace Dijkstras
{
    class Graph<T> where T : class
    {
        Dictionary<T, Dictionary<T, float>> vertices = new Dictionary<T, Dictionary<T, float>>();

        public void set_vertices(Dictionary<T, Dictionary<T, float>> vertices)
        {
            this.vertices = vertices;
        }

        public void add_vertex(T name, Dictionary<T, float> edges)
        {
            vertices[name] = edges;
        }

        public List<T> shortest_path(T start, T finish)
        {
            var previous = new Dictionary<T, T>();
            Dictionary<T, float> distances = new Dictionary<T, float>();
            var nodes = new List<T>();

            List<T> path = null;

            foreach (var vertex in vertices)
            {
                if (vertex.Key == start)
                {
                    distances[vertex.Key] = 0;
                }
                else
                {
                    distances[vertex.Key] = float.MaxValue;
                }

                nodes.Add(vertex.Key);
            }

            while (nodes.Count != 0)
            {
                nodes.Sort((x, y) => distances[x].CompareTo(distances[y]));

                var smallest = nodes[0];
                nodes.Remove(smallest);

                if (smallest == finish)
                {
                    path = new List<T>();
                    while (previous.ContainsKey(smallest))
                    {
                        path.Add(smallest);
                        smallest = previous[smallest];
                    }

                    break;
                }

                if (distances[smallest] == float.MaxValue)
                {
                    break;
                }

                foreach (var neighbor in vertices[smallest])
                {
                    var alt = distances[smallest] + neighbor.Value;
                    if (alt < distances[neighbor.Key])
                    {
                        distances[neighbor.Key] = alt;
                        previous[neighbor.Key] = smallest;
                    }
                }
            }

            return path;
        }
    }

    /*class MainClass
    {
        public static void Main(string[] args)
        {
            Graph g = new Graph();
            g.add_vertex('A', new Dictionary<T, float>() { { 'B', 7 }, { 'C', 8 } });
            g.add_vertex('B', new Dictionary<T, float>() { { 'A', 7 }, { 'F', 2 } });
            g.add_vertex('C', new Dictionary<T, float>() { { 'A', 8 }, { 'F', 6 }, { 'G', 4 } });
            g.add_vertex('D', new Dictionary<T, float>() { { 'F', 8 } });
            g.add_vertex('E', new Dictionary<T, float>() { { 'H', 1 } });
            g.add_vertex('F', new Dictionary<T, float>() { { 'B', 2 }, { 'C', 6 }, { 'D', 8 }, { 'G', 9 }, { 'H', 3 } });
            g.add_vertex('G', new Dictionary<T, float>() { { 'C', 4 }, { 'F', 9 } });
            g.add_vertex('H', new Dictionary<T, float>() { { 'E', 1 }, { 'F', 3 } });

            g.shortest_path('A', 'H').ForEach(x => Console.WriteLine(x));
        }
    }*/
}