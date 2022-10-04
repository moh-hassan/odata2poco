using System.Collections.Generic;
using System.Linq;

namespace OData2Poco.graph
{
    internal class Dependency
    {
        readonly Graph Graph;
        readonly IEnumerable<ClassTemplate> Model;
        internal ClassTemplate? this[int i] => Model.FirstOrDefault(a => a.Id == i);
        private Dependency(IEnumerable<ClassTemplate> model)
        {
            Model = model;
            Graph = new Graph(model.Count() + 1);
            BuildTree(model);
        }
        public static IEnumerable<ClassTemplate> Search(IEnumerable<ClassTemplate> model,
          params ClassTemplate[] items)
        {
            var d = new Dependency(model);
            var nodes = d.DFS(items.ToArray());
            return nodes.ToList();
        }
        IEnumerable<ClassTemplate> DFS(params ClassTemplate[] list)
        {
            List<int> found = new();
            foreach (var ct in list)
                found.AddRange(Graph.DFS(ct.Id));
            foreach (var n in found.Distinct())
            {
                var ct = this[n];
                if (ct != null)
                    yield return ct;
            }
        }
        private void BuildTree(IEnumerable<ClassTemplate> classList)
        {
            foreach (ClassTemplate ct in classList)
            {
                var dep = GetDependency(ct, classList);
                if (!dep.Any()) continue;
                foreach (var d in dep.Where(a => a != ct))
                {                  
                    Graph.AddEdge(ct.Id, d.Id);
                }
            }
        }
        internal IEnumerable<ClassTemplate> GetDependency(ClassTemplate ct,
           IEnumerable<ClassTemplate> model)
        {
            var dependency = new HashSet<ClassTemplate>();
            //1) property dependency             
            foreach (var property in ct.Properties)
            {
                var type = property.PropType.GetGenericBaseType();
                ClassTemplate? found = type.ToClassTemplate(model);
                if (found != null && !found.Id.Equals(ct.Id))
                    dependency.Add(found);
            }
            //2) base type dependency            
            var baseType = ct.BaseTypeToClassTemplate(model);
            if (baseType != null)
                dependency.Add(baseType);

            return dependency.Distinct();
        }
    }
}
