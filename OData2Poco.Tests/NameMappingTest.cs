// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco.Tests;

using Api;

public class NameMappingTest
{
    private readonly string _url = TestSample.TripPin4;
    private OdataConnectionString _connString;

    [OneTimeSetUp]
    public void Init()
    {
        _connString = new OdataConnectionString
        {
            ServiceUrl = _url
        };
    }

    [Test]
    public async Task Check_NameMapping_for_classes()
    {
        var code = await Generate(TestSample.RenameMap).ConfigureAwait(false);
        CommonTest.AssertRenameMap(code);
    }

    [Test]
    public async Task Check_NameMapping_for_class_properties()
    {
        var code = await Generate(TestSample.RenameMap2).ConfigureAwait(false);
        CommonTest.AssertRenameMap2(code);
    }

    [Test]
    public async Task Check_NameMapping_for_class_properties_with_all()
    {
        var code = await Generate(TestSample.RenameMap3).ConfigureAwait(false);
        CommonTest.AssertRenameMap3(code);
    }

    private async Task<string> Generate(string mapFile)
    {
        var json = File.ReadAllText(mapFile);
        var setting = new PocoSetting
        {
            RenameMap = json.ToObject<RenameMap>()
        };
        var o2P = new O2P(setting);
        var code = await o2P.GenerateAsync(_connString).ConfigureAwait(false);
        return code;
    }
}
