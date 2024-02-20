// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco.Graphs;

// This class represents a directed graph using adjacency list representation
internal class Graph
{
    // Array of lists for Adjacency List Representation
    private readonly List<int>[] _adj;
    private readonly int _v; // No. of vertices

    public Graph(int v)
    {
        _v = v;
        _adj = new List<int>[v];
        for (var i = 0; i < v; ++i)
        {
            _adj[i] = [];
        }
    }

    // DFS traversal, It uses recursive DFSUtil()
    public List<int> Dfs(int v)
    {
        List<int> found = [];

        // Mark all the vertices as not visited (set as false by default)
        var visited = new bool[_v];

        // Call the recursive helper function for traversal
        DfsUtil(v, visited, found);
        return found;
    }

    // Add an edge into the graph
    internal void AddEdge(int v, int w)
    {
        _adj[v].Add(w); // Add w to v's list.
    }

    // Used by DFS
    private void DfsUtil(int v, bool[] visited, List<int> found)
    {
        // Mark the current node as visited
        visited[v] = true;

        // Recur for all the vertices adjacent to this vertex
        var vList = _adj[v];
        foreach (var n in vList)
        {
            if (!visited[n])
            {
                found.Add(n);
                DfsUtil(n, visited, found);
            }
        }
    }
}
