// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

using FluentAssertions;
using OData2Poco.Extensions;

#pragma warning disable S3903  
public static class TestExtensions
#pragma warning restore S3903  
{
    public static void ShouldContain(this string source, string target, bool inOrder = true)
    {
        if (inOrder)
            source.ToLines().Should().ContainInOrder(target.ToLines());
        else
            source.ToLines().Should().Contain(target.ToLines());
    }
}
