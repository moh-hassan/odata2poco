// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco.Tests;

using System.Net;
using System.Net.Http.Headers;
using Newtonsoft.Json;

public class CustomeHandler : DelegatingHandler
{
    private readonly Action<HttpRequestMessage> _testingAction;

    public CustomeHandler(Action<HttpRequestMessage> testingAction)
    {
        _testingAction = testingAction;
    }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        _testingAction(request);
        var resp = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new PayloadContent(new
            {
                Success = true,
                Message = "Success"
            })
        };
        var tsc = new TaskCompletionSource<HttpResponseMessage>();
        tsc.SetResult(resp);
        return tsc.Task;
    }
}

internal sealed class PayloadContent : HttpContent
{
    private readonly MemoryStream _stream = new();

    public PayloadContent(object value)
    {
        Headers.ContentType = new MediaTypeHeaderValue("application/json");
        var jw = new JsonTextWriter(new StreamWriter(_stream))
        {
            Formatting = Formatting.Indented
        };
        var serializer = new JsonSerializer();
        serializer.Serialize(jw, value);
        jw.Flush();
        _stream.Position = 0;
    }

    protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
    {
        return _stream.CopyToAsync(stream);
    }

    protected override bool TryComputeLength(out long length)
    {
        length = _stream.Length;
        return true;
    }
}
