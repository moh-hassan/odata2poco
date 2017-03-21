#define DEBUG
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace OData2Poco.Tests
{
    [TestFixture]
    class HelperTest
    {
        [Test]
        [TestCase("int","?")]
        [TestCase("DateTime", "?")]
        public void GetNullableTest(string  type,string nullable)
        {
            
            Assert.AreEqual(Helper.GetNullable(type),nullable);
        }
    }
}
