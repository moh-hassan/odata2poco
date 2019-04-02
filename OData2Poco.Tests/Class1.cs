using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using OData2Poco.TestUtility;

namespace OData2Poco.Tests
{
  public   class ShouldContainConstraint: Constraint
    {
        string Text;
        public ShouldContainConstraint(string text)
        {
            Text=text;
        }
        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            return new ConstraintResult(this, actual, Regex.IsMatch(Text,Text.GetRegexPattern())) ;
        }
    }
}
