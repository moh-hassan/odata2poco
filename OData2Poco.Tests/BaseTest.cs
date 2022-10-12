// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

using NUnit.Framework;

namespace OData2Poco.Tests;

public abstract class BaseTest
{
    protected List<ClassTemplate> ClassList;

    [OneTimeSetUp]
    public void Setup()
    {
        ClassList = Moq.TripPinModel;
        Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;
    }
    public ClassTemplate GetClassTemplateSample(string name)
    {

        var ct = ClassList.FirstOrDefault(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        return ct;

    }

    protected string[] StringToArray(string text, char sep = ',')
    {
        return text == ""
            ? Array.Empty<string>()
            : text.Split(sep);
    }
}