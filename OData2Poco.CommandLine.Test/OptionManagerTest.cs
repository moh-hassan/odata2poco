
using NUnit.Framework;
 

namespace OData2Poco.CommandLine.Test
{
    public class OptionManagerTest
    {
        [Test]
        [TestCase("pas")]
        [TestCase("PAS")]        
        [TestCase("camel")]
        [TestCase("none")]      
        public void NameCase_valid_Test(CaseEnum nameCase)
        {
            var options = new Options
            {
                Lang =  Language.CS,
                NameCase = nameCase
            };
            var om = new OptionManager(options);
            Assert.That(options.Errors, Is.Empty);
           
        }
       
    }
}
