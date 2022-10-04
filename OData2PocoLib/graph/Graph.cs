using System.Collections.Generic;
//based on: https://www.geeksforgeeks.org/depth-first-search-or-dfs-for-a-graph/
//with modification

namespace OData2Poco.graph
{
    // This class represents a directed graph using adjacency list representation
    internal class Graph
    {
        private readonly int V; // No. of vertices

        // Array of lists for Adjacency List Representation
        private readonly List<int>[] adj;
        public Graph(int v)
        {
            V = v;
            adj = new List<int>[v];
            for (int i = 0; i < v; ++i)
                adj[i] = new List<int>();
        }

        // Add an edge into the graph
        internal void AddEdge(int v, int w)
        {
            adj[v].Add(w); // Add w to v's list.
        }

        // Used by DFS
        void DFSUtil(int v, bool[] visited, List<int> found)
        {
            // Mark the current node as visited             
            visited[v] = true;             

            // Recur for all the vertices adjacent to this vertex
            List<int> vList = adj[v];
            foreach (var n in vList)
            {
                if (!visited[n])
                {
                    found.Add(n);
                    DFSUtil(n, visited, found);
                }
            }
        }

        // DFS traversal, It uses recursive DFSUtil()
        public List<int> DFS(int v)
        {
            List<int> found = new List<int>();

            // Mark all the vertices as not visited (set as false by default)
            bool[] visited = new bool[V];

            // Call the recursive helper function for traversal
            DFSUtil(v, visited, found);
            return found;
        }
    }
}

