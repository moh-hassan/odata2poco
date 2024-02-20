// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco.Fake;

using System.Reflection;

public static class ProjectTestData
{
    private static readonly string s_relativeFakeFolder = Path.Combine("..", "..", "..", "..", "Fake", "project_samples");

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

    public static string FakeFolder => Path.GetFullPath(Path.Combine(BaseDirectory, s_relativeFakeFolder));

    public static string GenerateDefaultProjectTest => GetFullPath("Generate_default_project_Test.txt");
    public static string GenerateProjectForJsonTest => GetFullPath("Generate_project_for_json_Test.txt");

    public static string GenerateProjectForAttributesTest => GetFullPath("Generate_project_for_attributes_Test.txt");

    public static string GetFullPath(string relative)
    {
        var path = Path.GetFullPath(Path.Combine(BaseDirectory, s_relativeFakeFolder, relative));
        return path;
    }
}
