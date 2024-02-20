// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco.CommandLine.Test;

public class ApplicationInfoTest : BaseTest
{
    [Test]
    public void ApplicationInfo_test()
    {
        var versionPattern = @"[0-9a-z.-]+\+[0-9a-z]{9}";
        var version = ApplicationInfo.Version; //1.2.3+3bbb56990
        version.Should().MatchRegex($"^{versionPattern}$");
    }
}
