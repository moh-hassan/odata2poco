// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco.Tests;

using Api;
using NSubstitute;

public static class Moq
{
    public static List<ClassTemplate> TripPinModel
    {
        get
        {
            var jsonFile = Path.Combine(TestSample.FakeFolder, "TripPinModel.json");
            var classList = File.ReadAllText(jsonFile).ToObject<List<ClassTemplate>>();
            return classList;
        }
    }

    public static List<ClassTemplate> NorthWindModel
    {
        get
        {
            var gen = NorthWind3Async(new PocoSetting()).Result;
            var list = gen.GeneratePocoList();
            return list;
        }
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
        var list = new List<ClassTemplate>
        {
            ct
        };
        var gen = Substitute.For<IPocoGenerator>();
        gen.GeneratePocoList().Returns(list);
        gen.MetaData = GetMetadataInfo();
        return gen;
    }

    public static async Task<IPocoGenerator> Moq4IPocoGeneratorAsync(string url,
        PocoSetting setting)
    {
        var connection = OdataConnectionString.Create(url);
        var o2P = new O2P(setting);
        var gen = await o2P.GenerateModel(connection).ConfigureAwait(false);
        return gen;
    }

    public static Task<IPocoGenerator> TripPin4IgenAsync(PocoSetting setting)
    {
        var url = TestSample.TripPin4;
        return Moq4IPocoGeneratorAsync(url, setting);
    }

    public static Task<IPocoGenerator> TripPin4IgenAsync()
    {
        return TripPin4IgenAsync(new PocoSetting());
    }

    public static Task<IPocoGenerator> NorthWind3Async(PocoSetting setting)
    {
        var url = TestSample.NorthWindV3;
        return Moq4IPocoGeneratorAsync(url, setting);
    }

    public static Task<IPocoGenerator> NorthWindGeneratorAsync()
    {
        PocoSetting setting = new();
        var url = TestSample.NorthWindV3;
        return Moq4IPocoGeneratorAsync(url, setting);
    }

    private static MetaDataInfo GetMetadataInfo()
    {
        return new MetaDataInfo
        {
            MetaDataVersion = "v4.0",
            ServiceUrl = "http://localhost"
        };
    }
}
