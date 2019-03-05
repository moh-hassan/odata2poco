
using System;
using NUnit.Framework;
using OData2Poco.Exceptions;
using OData2Poco.Extensions;

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

        public void NameCase_valid_Test(string nameCase)
        {
            var options = new Options
            {
                Lang = "cs",
                NameCase = nameCase
            };
            var om = new OptionManager(options);
            Assert.That(options.Errors, Is.Empty);
            Console.WriteLine(options.Errors.Dump());
        }
        [Test]
        [TestCase("zz")]
        [TestCase("_")]
        [TestCase("")]
        public void NameCase_invalid_Test(string nameCase)
        {
            var options = new Options
            {
                Lang = "cs",
                NameCase = nameCase
            };
            var om = new OptionManager(options);
            Assert.That(options.Errors, Is.Not.Empty);
        }
    }
}
