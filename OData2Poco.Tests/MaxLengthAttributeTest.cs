// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

using NUnit.Framework;
using OData2Poco.Api;
using OData2Poco.Extensions;
using OData2Poco.Fake;

namespace OData2Poco.Tests;

public class MaxLengthAttributeTest
{

    [Test]
    public async Task String_maxlength_attribute_test()
    {
        var conn = new OdataConnectionString
        {
            ServiceUrl = TestSample.SampleWebApiV4,

        };
        var setting = new PocoSetting
        {
            Lang = Language.CS,
            NameCase = CaseEnum.None,
            Attributes = new List<string> { "max" },
        };
        var o2P = new O2P(setting);
        var code = await o2P.GenerateAsync(conn);
        var expected = @"
[MaxLength(30)]
 public string Title {get;set;} 
";
        Assert.That(code.TrimAllSpace(), Does.Contain(expected.TrimAllSpace()));

    }
}