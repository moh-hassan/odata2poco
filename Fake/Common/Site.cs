// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco.HttpMock;

internal class Site
{
    public string Name { get; set; }
    public string Path { get; set; }
    public string OdataPath { get; set; }
    public string Url { get; set; }
    public string OdataUrl { get; set; }
    public string XmlFile { get; set; }
    public override string ToString()
    {
        return $"Name: {Name}, Path: {Path}, Url: {Url}, XmlFile: {XmlFile}";
    }
    public static implicit operator string(Site s) => s.Url;
}