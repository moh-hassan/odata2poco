
using System;
using NUnit.Framework;
//using OData2Poco.Exceptions;
//using OData2Poco.Extensions;

namespace OData2Poco.CommandLine.Test
{
    public class OptionManagerTest
    {
        [Test]
        [TestCase("pas")]
        [TestCase("PAS")]
        [TestCase("cam")]
        [TestCase("camel")]
        [TestCase("none")]
        [TestCase("zzz")]
        public void NameCase_valid_Test(string nameCase)
        {
            var options = new Options
            {
                Lang = "cs",
                NameCase = nameCase
            };
            var om = new OptionManager(options);
            Assert.That(options.Errors, Is.Empty);
           
        }
       
    }
}
