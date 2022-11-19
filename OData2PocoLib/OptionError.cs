// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco;

public class OptionError
{
    public string Message { get; set; }
    public int Level { get; set; }

    public OptionError(string message, int level)
    {
        Message = message;
        Level = level;
    }
    public OptionError(string message) : this(message, 0)
    {
    }
    public override string ToString()
    {
        return Message;
    }
}