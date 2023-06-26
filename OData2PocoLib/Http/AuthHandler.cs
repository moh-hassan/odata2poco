// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

using System.Net;
using System.Net.Http.Headers;

namespace OData2Poco;
#pragma warning disable IDE0060
public class AuthHandler : DelegatingHandler
{
    public HttpRequestMessage? Request { get; set; }
    public HttpResponseMessage? Response { get; set; }
    public AuthenticationHeaderValue? AuthHeader { get; private set; }
    public string? Scheme => AuthHeader?.Scheme;
    public string? Parameter => AuthHeader?.Parameter;
    private readonly Action<HttpRequestMessage>? _testingAction;
    private readonly bool _isLive;
    private readonly HttpTracer _tracer = HttpTracer.Create();
    public AuthHandler(bool isLive = false)
    {
        _isLive = isLive;
    }
    public AuthHandler(Action<HttpRequestMessage> testingAction, bool isLive = false) : this()
    {
        _testingAction = testingAction;
    }
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        Request = request;
        AuthHeader = request.Headers.Authorization;
        _testingAction?.Invoke(request);
        _tracer.WriteLine($"AuthHeader: {AuthHeader}");
        _tracer.WriteLine($"Scheme: {Scheme}, Parameter: {Parameter}");
        _tracer.WriteLine($"Request:\n {request}");

        if (!_isLive) return new HttpResponseMessage(HttpStatusCode.OK);

        try
        {
            Response = await base.SendAsync(request, cancellationToken);
            _tracer.WriteLine($"Response:\n {Response}");
            return Response;
        }
        catch (HttpRequestException ex)
        {
            if (Response != null) _tracer.WriteLine($"Response:\n {Response}");
            _tracer.WriteLine($"DelegationHandler Exception:\n {ex.Message}");
            throw;
        }
    }
    public void Dump()
    {
        _tracer.Dump();
        _tracer.Dispose();
    }
}
