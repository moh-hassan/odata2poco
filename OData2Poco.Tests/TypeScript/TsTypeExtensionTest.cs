// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco.Tests.TypeScript;

public class TsTypeExtensionTest
{
    [Test]
    public void Reduce_namespace_test()
    {
        var s = "Microsoft.OData.SampleService.Models.TripPin";
        var sut = s.Reduce();
        sut.Should().Be("TripPin");
    }
}
