// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

using FluentAssertions;
using NUnit.Framework;

namespace OData2Poco.CommandLine.Test;

internal class ApplicationInfoTest
{
    [Test]
    public void ApplicationInfo_test()
    {
        var versionPattern = @"[0-9a-z.-]+\+[0-9a-z]{9}";
        var titlePattern = @"O2Pgen\s\(.+\)";
        var headingPattern = $@"^{titlePattern}\sVersion\s{versionPattern}$";

        var version = ApplicationInfo.Version;   //1.2.3+3bbb56990
        version.Should().MatchRegex($"^{versionPattern}$");

        var title = ApplicationInfo.Title; //O2Pgen (net472)
        title.Should().MatchRegex(titlePattern);

        var hi = ApplicationInfo.HeadingInfo;//O2Pgen (net472) Version 1.2.3+3bbb56990
        hi.Should().MatchRegex(headingPattern);
    }
}