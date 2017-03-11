using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using OData2Poco.Shared;

namespace OData2Poco.Tests
{
    [TestFixture]
    class StringExtensionsTest
    {
        [Test]
        [TestCase("userName", "UserName")]
        [TestCase("user_Name", "UserName")]
        [TestCase("user Name", "UserName")]
        [TestCase("user-Name", "UserName")]
        public void ToPascalCaseTest(string name , string pascalName)
        {
            Assert.AreEqual(pascalName, name.ToPascalCase());
        }

        [Test]
        [TestCase("UserName", "userName")]
        [TestCase("user_Name", "userName")]
        [TestCase("User Name", "userName")]
        [TestCase("user-Name", "userName")]
        public void ToCamelCaseTest(string name, string pascalName)
        {
            Assert.AreEqual(pascalName, name.ToCamelCase());
        }


        [Test]
        [TestCase("cam", CaseEnum.Camel)]
        [TestCase("CAmel", CaseEnum.Camel)]
        [TestCase("pas", CaseEnum.Pas)]
        public void StringToCaseEnumTest(string val , CaseEnum caseEnum)
        {
            Assert.AreEqual(caseEnum, val.ToCaseEnum());
        }

        [Test]
        public void TrimAllSpaceAndCrLfTest()
        {
            var text = @"this            is  line1


          and this is     line2";
            //Console.WriteLine(text);
            var expected = "this is line1 and this is line2";
            Console.WriteLine(text.TrimAllSpace());
            Assert.AreEqual(text.TrimAllSpace(),expected);
        }

        [Test]
        public void TrimAllSpaceAndKeepCrLfTest()
        {
            //var text = "this            is  line1   \n\nand this is     line2";
            var text = @"this            is  line1


          and this is     line2";
            var expected = "this is line1\nand this is line2\n";

  
            Console.WriteLine("expected:\n{0}",expected);
            //var expected = "this is line1 and this is line2";
            //Console.WriteLine(text.TrimAllSpace(true));
            Assert.AreEqual(text.TrimAllSpace(true), expected);
        }

      
    }
}
