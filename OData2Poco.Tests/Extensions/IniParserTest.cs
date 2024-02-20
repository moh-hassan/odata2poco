// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco.Tests;

using Extensions;

public class IniParserTest
{
    [Test]
    public void IniParser_test()
    {
        var iniData = """
                          ;including formula
                          [json]
                          Format= "[JsonProperty(PropertyName = {{PropName.Quote()}})]"
                      
                          [_key_]
                          Format= [Key]
                          Filter= IsKey;
                      """;

        var result = iniData.ParseIni();
        result.Should().NotBeEmpty();
        result.Should().ContainKey("json");
        result.Should().ContainKey("_key_");
        result["json"].Should().ContainKey("Format");
        result["json"]["Format"].Should()
            .BeEquivalentTo("[JsonProperty(PropertyName = {{PropName.Quote()}})]");
    }
}
