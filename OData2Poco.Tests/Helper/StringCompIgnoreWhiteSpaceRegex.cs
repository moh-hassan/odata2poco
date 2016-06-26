using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//http://stackoverflow.com/questions/4718965/c-sharp-string-comparison-ignoring-spaces-carriage-return-or-line-breaks
namespace OData2Poco.Tests.Helper
{
     class StringCompIgnoreWhiteSpaceRegex : IEqualityComparer<string>
    {
        public bool Equals(string strx, string stry)
        {
            if (strx == null)
                return string.IsNullOrWhiteSpace(stry);
            else if (stry == null)
                return string.IsNullOrWhiteSpace(strx);

            string a = System.Text.RegularExpressions.Regex.Replace(strx, @"\s", "");
            string b = System.Text.RegularExpressions.Regex.Replace(stry, @"\s", "");
            return String.Compare(a, b, true) == 0;
        }

        public int GetHashCode(string obj)
        {
            if (obj == null)
                return 0;

            string a = System.Text.RegularExpressions.Regex.Replace(obj, @"\s", "");
            return a.GetHashCode();
        }
    }
}
