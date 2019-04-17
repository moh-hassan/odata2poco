using System;
using System.Collections.Generic;
using System.Text;
using NSubstitute;

namespace OData2Poco.Tests
{
    public class Moq
    {
        private static MetaDataInfo GetMetadataInfo()
        {
            return  new MetaDataInfo
            {
                MetaDataVersion="v4.0",
                ServiceUrl="http://localhost",
            }; 
        }
        public static IPocoGenerator Moq4IPocoGenerator(List<ClassTemplate> list)
        {
            var gen = Substitute.For<IPocoGenerator>();
            gen.GeneratePocoList().Returns(list);
            gen.MetaData= GetMetadataInfo();
            return gen;
        }
        public static IPocoGenerator Moq4IPocoGenerator(ClassTemplate ct)
        {
            var list = new List<ClassTemplate> {ct};
            var gen = Substitute.For<IPocoGenerator>();
            gen.GeneratePocoList().Returns(list);
            gen.MetaData= GetMetadataInfo();
            return gen;
        }
    }
}
