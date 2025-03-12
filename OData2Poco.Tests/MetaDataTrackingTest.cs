// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco.Tests;

using System;

[TestFixture]
public class MetaDataTrackingTest
{
    private MetaDataTracking _metaDataTracking = new MetaDataTracking();

    [Test]
    public void String_date_format_should_be_converted_to_datetimeoffset()
    {
        var iso8601Date = "2025-03-12T06:54:15Z";
        var expected = new DateTimeOffset(2025, 3, 12, 6, 54, 15, TimeSpan.Zero);
        var httpDate = _metaDataTracking.DateTimeOffsetToHttpDate(iso8601Date);
        Assert.That(httpDate, Is.EqualTo(expected));
    }

    [Test]
    public void Exist_poco_file_return_valid_datetime()
    {
        //Arrange
        //     Generated On: 2025-03-08T13:43:08
        var filepath = TestSample.DemoCs;
        var expected = new DateTimeOffset(2025, 3, 8, 13, 43, 8, TimeSpan.Zero);
        //Act
        var dt = _metaDataTracking.GetLastUpdate(filepath);
        //Assert
        Assert.That(dt, Is.EqualTo(expected));
    }

    [Test]
    public void Not_exist_poco_file_return_null()
    {
        //Arrange
        //Act
        var dt = _metaDataTracking.GetLastUpdate("file-not-exist.cs");
        Assert.That(dt?.Date, Is.Null);
    }

    [Test]
    public void Exist_poco_file_in_pocosetting_return_valid_datetime()
    {
        //Arrange
        var pocoSetting = new PocoSetting
        {
            CodeFilename = TestSample.DemoCs
        };
        var expected = new DateTimeOffset(2025, 3, 8, 13, 43, 8, TimeSpan.Zero);
        //Act
        var dt = pocoSetting.GetLastUpdate();
        //Assert
        Assert.That(dt, Is.EqualTo(expected));
    }

    [Test]
    public void Not_exist_poco_in_pocosetting_file_return_null()
    {
        //Arrange
        var pocoSetting = new PocoSetting
        {
            CodeFilename = "file-not-exist.cs"
        };
        //Act
        var dt = pocoSetting.GetLastUpdate();
        //Assert
        Assert.That(dt, Is.Null);
    }
}
