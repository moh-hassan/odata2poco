// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

using FluentAssertions;
using NUnit.Framework;
using OData2Poco.Extensions;

namespace OData2Poco.Tests.TypeScript;

internal class TsTypeExtensionTest
{
    [Test]
    public void Reduce_namespace_test()
    {
        var s = "Microsoft.OData.SampleService.Models.TripPin";
        var sut = s.Reduce();
        sut.Should().Be("TripPin");
    }
}