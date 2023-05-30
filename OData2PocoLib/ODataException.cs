// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.


namespace OData2Poco;

[Serializable]
public class OData2PocoException : Exception
{
    public OData2PocoException()
    {
    }

    public OData2PocoException(string message)
        : base(message)
    {
    }

    public OData2PocoException(string message, Exception inner)
        : base(message, inner)
    {
    }
}