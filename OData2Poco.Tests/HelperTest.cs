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
        public void GetNullableTest(string  type,string nullable)
        {
            Console.WriteLine("tes");
            Console.WriteLine("*** {0}",Helper.GetNullable(type));
            Assert.AreEqual(Helper.GetNullable(type),nullable);
        }
    }
}
