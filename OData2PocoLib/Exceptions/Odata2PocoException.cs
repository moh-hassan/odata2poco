using System;

namespace OData2Poco.Exceptions
{
    public class Odata2PocoException : Exception
    {
        public Odata2PocoException()
        {
        }

        public Odata2PocoException(string message)
            : base(message)
        {
        }

        public Odata2PocoException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
