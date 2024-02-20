// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco.Http;

public class HttpTracer : IDisposable
{
    private readonly TextWriterTraceListener _listener;
    private readonly StringWriter _writer;

    public HttpTracer()
    {
        _writer = new StringWriter();
        _listener = new TextWriterTraceListener(_writer);
        TraceSource source = new("HttpSource");
        source.Listeners.Add(_listener);
        _listener.IndentSize = 2;
    }

    public string TraceOutput => _writer.ToString();

    public static HttpTracer Create()
    {
        return new();
    }

    public void Write(string message)
    {
        _listener.Write(message);
    }

    public void WriteLine(string message)
    {
        _listener.WriteLine(message);
    }

    public void Clear()
    {
        _writer.GetStringBuilder().Clear();
    }

    public string Dump()
    {
        Console.WriteLine(_writer.GetStringBuilder());
        return _writer.GetStringBuilder().ToString();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        Trace.Listeners.Remove(_listener);
        _listener.Dispose();
        _writer.Dispose();
    }
}
