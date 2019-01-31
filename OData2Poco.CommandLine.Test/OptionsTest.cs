using NUnit.Framework;

namespace OData2Poco.CommandLine.Test
{
    class OptionsTest
    {
        [Test]
        [TestCase("pas")]
        [TestCase("PAS")]
        [TestCase("cam")]
        [TestCase("camel")]
        [TestCase("none")]
       
        public void NameCase_valid_Test(string name)
        {
            var options = new Options
            {
                Lang = "cs",
                NameCase = name
            };
            var ret = options.Validate();
            Assert.That(ret , Is.EqualTo(0));
            Assert.That(options.Errors, Is.Empty);
        }
        [Test]
        [TestCase("zz")]
        [TestCase("_")]
        [TestCase("")]
        public void NameCase_invalid_Test(string name)
        {
            var options = new Options
            {
                Lang = "cs",
                NameCase = name
            };
            var ret = options.Validate();
            Assert.That(ret, Is.EqualTo(0));
            Assert.That(options.Errors, Is.Not.Empty);
          
        }
    }
}
