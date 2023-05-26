// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

using FluentAssertions;
using NUnit.Framework;

namespace OData2Poco.CommandLine.Test;

internal class ApplicationInfoTest: BaseTest
{
    [Test]
    public void ApplicationInfo_test()
    {
        var versionPattern = @"[0-9a-z.-]+\+[0-9a-z]{9}";
        var titlePattern = @"o2pgen\s\(.+\)";
        var headingPattern = $@"^{titlePattern}\sVersion\s{versionPattern}$";

        var version = ApplicationInfo.Version;   //1.2.3+3bbb56990
        version.Should().MatchRegex($"^{versionPattern}$");
    }
}