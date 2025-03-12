// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco;

using System;
using System.Globalization;
using System.Text.RegularExpressions;

internal class MetaDataTracking
{
    public DateTimeOffset? GetLastUpdate(string filePath)
    {
        DateTimeOffset now = DateTimeOffset.UtcNow.Date;
        if (!File.Exists(filePath))
            return null;
        var fileContent = File.ReadAllText(filePath);
        var pattern = @"\/\/\s+Generated\s+On:\s+(\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2})";
        var match = Regex.Match(fileContent, pattern);
        var dateTimeString = string.Empty;
        if (!match.Success)
        {
            Console.WriteLine("DateTime not found.");
            return null;
        }

        dateTimeString = match.Groups[1].Value.Trim();
        var generatedOn = DateTimeOffsetToHttpDate(dateTimeString);
        return generatedOn;
    }

    public DateTimeOffset DateTimeOffsetToHttpDate(string dateTime)
    {
        //var iso8601Date = "2025-03-08T13:43:08Z";
        var dateTimeOffset = DateTimeOffset
            .Parse(dateTime, null, DateTimeStyles.AssumeUniversal);
        return dateTimeOffset;
    }
}
