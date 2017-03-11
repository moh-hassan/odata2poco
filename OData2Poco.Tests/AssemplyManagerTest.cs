using System.Collections.Concurrent;
using NUnit.Framework;
using OData2Poco.Shared;

namespace OData2Poco.Tests
{
      [TestFixture]
    class AssemplyManagerTest
    {
          [Test]
          public void AddAsemplyTest()
          {
              var pocosetting = new PocoSetting();
              AssemplyManager am = new AssemplyManager(pocosetting, new ConcurrentDictionary<string, ClassTemplate>());
              am.AddAssemply("xyz");
              Assert.IsTrue(am.AssemplyReference.Exists(m => m.Contains("xyz")));
          }
          [Test]
          public void AddAsemplyArrayTest()
          {
              var pocosetting = new PocoSetting();
              AssemplyManager am = new AssemplyManager(pocosetting, new ConcurrentDictionary<string, ClassTemplate>());
              am.AddAssemply("xyz", "abc");
              Assert.IsTrue(am.AssemplyReference.Exists(m => m.Contains("xyz")));
              Assert.IsTrue(am.AssemplyReference.Exists(m => m.Contains("abc")));
          }
          [Test]
          //[TestCase("int", "?")]
              [TestCase("key","System.ComponentModel.DataAnnotations")]
            //{"required" ,"System.ComponentModel.DataAnnotations.Schema"},
            //{"table" ,"System.ComponentModel.DataAnnotations.Schema"},
            //{"json","Newtonsoft.Json"},  
            ////assemplies for datatype
            //{"geometry","Microsoft.Spatial"},  
            //{"geography", "Microsoft.Spatial"}  
          public void AddAsemplyByKey(string key ,string value)
          {
              var pocosetting =new PocoSetting
              {
                  AddKeyAttribute = true
              };

              AssemplyManager am = new AssemplyManager(pocosetting,new ConcurrentDictionary<string, ClassTemplate>());
              Assert.IsTrue(am.AssemplyReference.Exists(m => m.Contains(value)));
          }
          [Test]
          public void AddAsemplyMultiAttributes()
          {
              var pocosetting = new PocoSetting
              {
                  AddTableAttribute = true,
                  AddRequiredAttribute = true
              };

              AssemplyManager am = new AssemplyManager(pocosetting, new ConcurrentDictionary<string, ClassTemplate>());
              Assert.IsTrue(am.AssemplyReference.Exists(m => m.Contains("System.ComponentModel.DataAnnotations.Schema")));
          }
          [Test]
          public void AddExternalAsemply()
          {
              var pocosetting = new PocoSetting
              {
                  AddTableAttribute = true,
                  AddRequiredAttribute = true
              };

              AssemplyManager am = new AssemplyManager(pocosetting, new ConcurrentDictionary<string, ClassTemplate>());
              am.AddAssemply("xyz");
              Assert.IsTrue(am.AssemplyReference.Exists(m => m.Contains("System.ComponentModel.DataAnnotations.Schema")));
              Assert.IsTrue(am.AssemplyReference.Exists(m => m.Contains("xyz")));
          }
    }
}
