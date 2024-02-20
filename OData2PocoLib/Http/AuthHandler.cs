// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco.Http;

using System.Net;
using System.Net.Http.Headers;

public class AuthHandler : DelegatingHandler
{
    private readonly bool _isLive;
    private readonly Action<HttpRequestMessage>? _testingAction;
    private readonly HttpTracer _tracer = HttpTracer.Create();
    private bool _disposed;

    public AuthHandler(bool isLive = false)
    {
        _isLive = isLive;
    }

    public AuthHandler(Action<HttpRequestMessage> testingAction) : this()
    {
        _testingAction = testingAction;
    }

    public HttpRequestMessage? Request { get; set; }
    public HttpResponseMessage? Response { get; set; }
    public AuthenticationHeaderValue? AuthHeader { get; private set; }
    public string? Scheme => AuthHeader?.Scheme;
    public string? Parameter => AuthHeader?.Parameter;

    public void Dump()
    {
        _tracer.Dump();
        _tracer.Dispose();
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        Request = request;
        Debug.Assert(request != null, nameof(request) + " != null");
        AuthHeader = request.Headers.Authorization;
        _testingAction?.Invoke(request);
        _tracer.WriteLine($"AuthHeader: {AuthHeader}");
        _tracer.WriteLine($"Scheme: {Scheme}, Parameter: {Parameter}");
        _tracer.WriteLine($"Request:\n {request}");

        if (!_isLive)
        {
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        try
        {
            Response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
            _tracer.WriteLine($"Response:\n {Response}");
            return Response;
        }
        catch (HttpRequestException ex)
        {
            if (Response != null)
            {
                _tracer.WriteLine($"Response:\n {Response}");
            }

            _tracer.WriteLine($"DelegationHandler Exception:\n {ex.Message}");
            throw;
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _tracer.Dispose();
            }

            _disposed = true;
        }

        base.Dispose(disposing);
    }
}
