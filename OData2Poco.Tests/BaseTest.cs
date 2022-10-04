using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using OData2Poco.Extensions;

namespace OData2Poco.Tests
{
    public abstract class BaseTest
    {        
        protected List<ClassTemplate> ClassList;

        [OneTimeSetUp]
        public void Setup()
        {            
            ClassList = Moq.TripPinModel;
            Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;
        }
        public ClassTemplate GetClassTemplateSample(string name)
        {

            var ct = ClassList.FirstOrDefault(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            return ct;

        }

        protected string[] StringToArray(string text, char sep = ',')
        {
            return text == ""
                ? Array.Empty<string>() 
                : text.Split(sep);
        }
    }
}
