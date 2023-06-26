// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

using NUnit.Framework;
using OData2Poco.InfraStructure.FileSystem;

namespace OData2Poco.Tests;
#pragma warning disable CA1822
public abstract class BaseTest
{
    protected List<ClassTemplate> ClassList;
    protected IPocoFileSystem _fileSystem;


    [OneTimeSetUp]
    public void BaseOneTimeSetup()
    {
        ClassList = Moq.TripPinModel;
        Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;
        _fileSystem = new NullFileSystem();
        Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
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
    /// <summary>
    /// create temp file in user temp folder
    /// </summary>
    /// <param name="content"></param>
    /// <param name="extension"> extension w/o . like .txt or txt</param>
    /// <returns></returns>
    //extension ".txt"
    protected string NewTemporaryFile(string content, string extension = null)
    {
        string filepath = Path.GetTempFileName();
        if (!string.IsNullOrEmpty(extension))
        {
            extension = extension.TrimStart('.');
            filepath = Path.ChangeExtension(Path.GetTempFileName(), $".{extension}");
        }

        File.WriteAllText(filepath, content);
        return filepath;
    }
    /// <summary>
    /// Remove environment variable
    /// </summary>
    /// <param name="name"></param>
    protected void DelEnv(params string[] names)
    {
        foreach (var name in names)
        {
            Environment.SetEnvironmentVariable(name, null);
        }
    }

    protected void CreateEnv(string name, string value)
    {
        Environment.SetEnvironmentVariable(name, value);
    }
}