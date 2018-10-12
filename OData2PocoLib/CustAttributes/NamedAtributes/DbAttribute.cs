using System.Collections.Generic;
using System.Linq;

namespace OData2Poco.CustAttributes.NamedAtributes
{
    public class DbAttribute : INamedAttribute
    {
        public string Name { get; } = "db";
       
        public List<string> GetAttributes(PropertyTemplate property)
        {
            var att = new List<string>();
            _sharedAttributes.ForEach(x =>
            {
                var a = x.GetAttributes(property);
                if (a.Any())
                    att.AddRange( a);
            });
            return att;
        }

        public List<string> GetAttributes(ClassTemplate property)
        {

            var att = new List<string>();
            _sharedAttributes.ForEach(x =>
            {
                var a = x.GetAttributes(property);
                if (a.Any())
                    att.AddRange(a);
            });
            return att;
        }

        private readonly List<INamedAttribute> _sharedAttributes;
        public DbAttribute()
        {
            _sharedAttributes = new List<INamedAttribute>
            {
               new  KeyAttribute(),
               new  TableAttribute(),
               new RequiredAttribute(),
            };
        }
    }
}
