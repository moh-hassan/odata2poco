// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.


namespace OData2Poco;

[Serializable]
public class ODataException : Exception
{
    public ODataException()
    {
    }

    public ODataException(string message)
        : base(message)
    {
    }

    public ODataException(string message, Exception inner)
        : base(message, inner)
    {
    }
}