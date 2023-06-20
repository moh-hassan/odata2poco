// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

using NSubstitute;
using OData2Poco.Api;
using OData2Poco.Extensions;
using OData2Poco.Fake;

namespace OData2Poco.Tests;

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
        var o2P = new O2P(setting);
        var gen = await o2P.GenerateModel(connection);
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
        return await TripPin4IgenAsync(new PocoSetting());
    }
    public static List<ClassTemplate> TripPinModel
    {
        get
        {
            var jsonFile = Path.Combine(TestSample.FakeFolder, "TripPinModel.json");
            var classList = File.ReadAllText(jsonFile).ToObject<List<ClassTemplate>>();
            return classList;
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
        PocoSetting setting = new();
        string url = TestSample.NorthWindV3;
        return await Moq4IPocoGeneratorAsync(url, setting);
    }
    #endregion
}