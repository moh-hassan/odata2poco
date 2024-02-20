// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco.Graphs;

using Extensions;

internal sealed class Dependency
{
    private readonly Graph _graph;
    private readonly IEnumerable<ClassTemplate> _model;

    private Dependency(IEnumerable<ClassTemplate> model)
    {
        _model = model;
        _graph = new Graph(model.Count() + 1);
        BuildTree(model);
    }

    internal ClassTemplate? this[int i] => _model.FirstOrDefault(a => a.Id == i);

    public static IEnumerable<ClassTemplate> Search(IEnumerable<ClassTemplate> model,
        params ClassTemplate[] items)
    {
        Dependency d = new(model);
        var nodes = d.Dfs(items.ToArray());
        return nodes.ToList();
    }

    internal IEnumerable<ClassTemplate> GetDependency(ClassTemplate ct,
        IEnumerable<ClassTemplate> model)
    {
        HashSet<ClassTemplate> dependency = [];
        //1) property dependency
        foreach (var property in ct.Properties)
        {
            var type = property.PropType.GetGenericBaseType();
            var found = type.ToClassTemplate(model);
            if (found != null && !found.Id.Equals(ct.Id))
            {
                dependency.Add(found);
            }
        }

        //2) base type dependency
        var baseType = ct.BaseTypeToClassTemplate(model);
        if (baseType != null)
        {
            dependency.Add(baseType);
        }

        return dependency.Distinct();
    }

    private IEnumerable<ClassTemplate> Dfs(params ClassTemplate[] list)
    {
        List<int> found = [];
        foreach (var ct in list)
        {
            found.AddRange(_graph.Dfs(ct.Id));
        }

        foreach (var n in found.Distinct())
        {
            var ct = this[n];
            if (ct != null)
            {
                yield return ct;
            }
        }
    }

    private void BuildTree(IEnumerable<ClassTemplate> classList)
    {
        var list = classList.ToList();
        foreach (var ct in list)
        {
            var dep = GetDependency(ct, list).ToArray();
            if (dep.Length == 0)
            {
                continue;
            }

            foreach (var d in dep.Where(a => !a.Equals(ct)))
            {
                _graph.AddEdge(ct.Id, d.Id);
            }
        }
    }
}
