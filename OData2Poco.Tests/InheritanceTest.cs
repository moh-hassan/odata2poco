using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

//these tests are moved from OData2Poco.CommandLine.Test, modified to use O2P class
namespace OData2Poco.Tests
{
    class InheritanceTest
    {
        [Test]
        public void InheritanceEnabledByDefaultTest()
        {
            var o2p= new O2P();
            Assert.IsTrue(o2p.Setting.UseInheritance);
        }
        [Test]
        public void InheritanceDisabledWithInheritSettingTest()
        {
           
            var setting = new PocoSetting
            {
                Inherit = "MyBaseClass",
            };
            var o2p = new O2P(setting);
            Assert.IsFalse(o2p.Setting.UseInheritance);
        }
    }
}
