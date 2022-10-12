// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

using NUnit.Framework;


namespace OData2Poco.CommandLine.Test;

public class OptionManagerTest
{
    [Test]
    [TestCase("pas")]
    [TestCase("PAS")]
    [TestCase("camel")]
    [TestCase("none")]
    public void NameCase_valid_Test(string nameCase)
    {
        Enum.TryParse<CaseEnum>(nameCase, out var nameCase2);
        var options = new Options
        {
            Lang = Language.CS,
            NameCase = nameCase2
        };
        _ = new OptionManager(options);
        Assert.That(options.Errors, Is.Empty);

    }

}