// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

using System.Reflection;
namespace OData2Poco.Fake;
public static class ProjectTestData
{
    public static string BaseDirectory
    {
        get
        {
            var assembly = Assembly.GetExecutingAssembly();
            var codebase = new Uri(assembly.Location);
            var path = codebase.LocalPath;
            return Path.GetDirectoryName(path);
        }
    }

    private static readonly string RelativeFakeFolder = Path.Combine("..", "..", "..", "..", "Fake", "project_samples");

    public static string FakeFolder => Path.GetFullPath(Path.Combine(BaseDirectory, RelativeFakeFolder));

    public static string GenerateDefaultProjectTest => GetFullPath("Generate_default_project_Test.txt");
    public static string GenerateProjectForJsonTest => GetFullPath("Generate_project_for_json_Test.txt");

    public static string GenerateProjectForAttributesTest => GetFullPath("Generate_project_for_attributes_Test.txt");

    public static string GetFullPath(string relative)
    {
        var path = Path.GetFullPath(Path.Combine(BaseDirectory, RelativeFakeFolder, relative));
        return path;
    }
}

