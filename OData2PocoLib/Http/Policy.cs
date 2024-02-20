// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco.Http;

using System.Net;
using InfraStructure.Logging;

internal static class Policy
{
    private static readonly ILog s_logger = PocoLogger.Default;

    public static async Task<HttpResponseMessage?> RetryAsync(
        Func<Task<HttpResponseMessage>> action,
        int maxRetries,
        int delay = 2)
    {
        var retryCount = 0;
        HttpResponseMessage? response = null;
        var delaySeconds = delay;

        while (retryCount < maxRetries)
        {
            if (retryCount > 0)
            {
                s_logger.Info($"Retry http connection: {retryCount}");
            }

            try
            {
                response = await action().ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    break;
                }

                if (response.StatusCode is HttpStatusCode.ServiceUnavailable or
                    HttpStatusCode.GatewayTimeout)
                {
                    // Handle the 503 or 504 error and retry the request
                    retryCount++;
                    if (retryCount < maxRetries)
                    {
                        s_logger.Info($"Retry: {retryCount}, StatusCode: {response.StatusCode}");
                        Console.WriteLine($"Retry: {retryCount}, StatusCode: {response.StatusCode}");
                        await Task.Delay(TimeSpan.FromSeconds(delaySeconds)).ConfigureAwait(false);
                        delaySeconds++;
                    }
                }
                else
                {
                    break;
                }
            }
            catch (HttpRequestException ex)
            {
                retryCount++;
                if (retryCount < maxRetries)
                {
                    s_logger.Info($"Retry: {retryCount}, Error: {ex.Message}");
                    Console.WriteLine($"Retry: {retryCount}, StatusCode: {response?.StatusCode}");
                    await Task.Delay(TimeSpan.FromSeconds(delaySeconds)).ConfigureAwait(false);
                    delaySeconds++;
                }
            }
        }

        return response;
    }
}
