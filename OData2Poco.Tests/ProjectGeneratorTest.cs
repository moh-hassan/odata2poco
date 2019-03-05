using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace OData2Poco.Tests
{
    class ProjectGeneratorTest
    {
        [Test]
        public void Test1()
        {
            var attributes= new List<string> { ""};
           var sut = new ProjectGenerator(attributes);
           Console.WriteLine(sut.GetPackageCommon());
           Console.WriteLine(sut.GetProjectCode());
        }
        [Test]
        public void Test1a()
        {
            var attributes= new List<string> { "json"};
            var sut = new ProjectGenerator(attributes);
            Console.WriteLine(sut.GetPackageCommon());
            Console.WriteLine(sut.GetProjectCode());
        }
        [Test]
        public void Test1aa()
        {
            var attributes= new List<string> { "proto"};
            var sut = new ProjectGenerator(attributes);
            Console.WriteLine(sut.GetPackageCommon());
            Console.WriteLine(sut.GetProjectCode());
        }
        [Test]
        public void Test1aaa()
        {
            var attributes= new List<string> {"key"};
            var sut = new ProjectGenerator(attributes);
            //Console.WriteLine(sut.GetPackageCommon());
            Console.WriteLine(sut.GetProjectCode());
        }
    }
}
