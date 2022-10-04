using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;
using OData2Poco.Api;

namespace OData2Poco.Tests
{
    public static class Moq
    {
        private static MetaDataInfo GetMetadataInfo()
        {
            return new MetaDataInfo
            {
                MetaDataVersion = "v4.0",
                ServiceUrl = "http://localhost",
            };
        }
       
        public static IPocoGenerator Moq4IPocoGenerator(List<ClassTemplate> list)
        {
            var gen = Substitute.For<IPocoGenerator>();
            gen.GeneratePocoList().Returns(list);
            gen.MetaData = GetMetadataInfo();
            return gen;
        }
        public static IPocoGenerator Moq4IPocoGenerator(ClassTemplate ct)
        {
            var list = new List<ClassTemplate> { ct };
            var gen = Substitute.For<IPocoGenerator>();
            gen.GeneratePocoList().Returns(list);
            gen.MetaData = GetMetadataInfo();
            return gen;
        }
        public static async Task<IPocoGenerator> Moq4IPocoGeneratorAsync(string url,
           PocoSetting setting)
        {
            OdataConnectionString connection = OdataConnectionString.Create(url);
            var o2p = new O2P(setting);
            var gen = await o2p.GenerateModel(connection);
            return gen;
        }
        #region TripPin
        public static async Task<IPocoGenerator> TripPin4IgenAsync(PocoSetting setting)
        {
            string url = TestSample.TripPin4;
            return await Moq4IPocoGeneratorAsync(url, setting);
        }
        public static async Task<IPocoGenerator> TripPin4IgenAsync()
        {
             return  await TripPin4IgenAsync(new PocoSetting());
        }
        public static List<ClassTemplate> TripPinModel
        {
            get
            {
                var gen = TripPin4IgenAsync(new PocoSetting()).Result;
                 var list= gen.GeneratePocoList();
                return list;
            }
        }
        
        #endregion
        #region NorthWind
        public static List<ClassTemplate> NorthWindModel
        {
            get
            {
                var gen = NorthWind3Async(new PocoSetting()).Result;
                var list = gen.GeneratePocoList();
                return list;
            }
        }        
        public static async Task<IPocoGenerator> NorthWind3Async(PocoSetting setting)
        {
            string url = TestSample.NorthWindV3;
            return await Moq4IPocoGeneratorAsync(url, setting);
        }
        public static async Task<IPocoGenerator> NorthWindGeneratorAsync()
        {
            PocoSetting setting = new ();
            string url = TestSample.NorthWindV3;
            return await Moq4IPocoGeneratorAsync(url, setting);
        }
        #endregion
    }
}
