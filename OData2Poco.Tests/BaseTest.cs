// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco.Tests;

using InfraStructure.FileSystem;

public abstract class BaseTest
{
    protected List<ClassTemplate> _classList;
    protected IPocoFileSystem _fileSystem;
    protected bool IsCi => Environment.GetEnvironmentVariable("CI") == "true";
    protected bool IsLocalTest => Environment.GetEnvironmentVariable("LOCAL_TEST") == "1";

    [OneTimeSetUp]
    public void BaseOneTimeSetup()
    {
        _classList = Moq.TripPinModel;
        Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;
        _fileSystem = new NullFileSystem();
        Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
    }

    protected ClassTemplate GetClassTemplateSample(string name)
    {
        var ct = _classList.Find(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        return ct;
    }

    protected string[] StringToArray(string text, char sep = ',')
    {
        return string.IsNullOrEmpty(text) ? [] : text.Split(sep);
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
        var filepath = Path.GetTempFileName();
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
    /// <param name="names"></param>
    protected void DelEnv(params string[] names)
    {
        if (names == null) return;
        foreach (var name in names) Environment.SetEnvironmentVariable(name, null);
    }

    protected void CreateEnv(string name, string value)
    {
        Environment.SetEnvironmentVariable(name, value);
    }
}
