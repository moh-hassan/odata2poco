using FluentAssertions;
using NUnit.Framework;
using OData2Poco.Extensions;
using OData2Poco.TypeScript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OData2Poco.Tests.TypeScript
{
    internal class TsTypeExtensionTest
    { 
        [Test]
        public void Reduce_namespace_test()
        {
            var s = "Microsoft.OData.SampleService.Models.TripPin";
            var sut = s.Reduce();            
            sut.Should().Be("TripPin");
        }
    }
}
