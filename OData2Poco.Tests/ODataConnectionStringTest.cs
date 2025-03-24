// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco.Tests;
using System;

public class ODataConnectionStringTest
{
    [Test]
    public void OdataConnectionString_LastUpdated_can_be_from_pocosetting()
    {
        var pocosetting = new PocoSetting
        {
            CodeFilename = TestSample.DemoCs,
        };
        var connection = new OdataConnectionString
        {
            ServiceUrl = "http://localhost"
        };
        connection.SetLastUpdated(pocosetting);
        Assert.That(connection.LastUpdated, Is.Not.Null);
    }

    [Test]
    public void OdataConnectionString_LastUpdated_can_be_set_by_value()
    {
        var connection = new OdataConnectionString
        {
            ServiceUrl = "http://localhost"
        };
        connection.SetLastUpdated(DateTimeOffset.UtcNow);
        Assert.That(connection.LastUpdated, Is.Not.Null);
    }
}
