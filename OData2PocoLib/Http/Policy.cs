// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

using System.Net;
using OData2Poco.InfraStructure.Logging;

namespace OData2Poco.Http;

internal static class Policy
{
    private static readonly ILog Logger = PocoLogger.Default;
    public static async Task<HttpResponseMessage?> RetryAsync(Func<Task<HttpResponseMessage>> action, int maxRetries, int delay = 2)
    {
        var retryCount = 0;
        HttpResponseMessage? response = null;
        var delaySeconds = delay;

        while (retryCount < maxRetries)
        {
            if (retryCount > 0)
                Logger.Info($"Retry http connection: {retryCount}");
            try
            {
                response = await action();

                if (response.IsSuccessStatusCode)
                    break;
                if (response.StatusCode == HttpStatusCode.ServiceUnavailable ||
                    response.StatusCode == HttpStatusCode.GatewayTimeout)
                {
                    // Handle the 503 or 504 error and retry the request
                    retryCount++;
                    if (retryCount < maxRetries)
                    {
                        Logger.Info($"Retry: {retryCount}, StatusCode: {response.StatusCode}");
                        Console.WriteLine($"Retry: {retryCount}, StatusCode: {response.StatusCode}");
                        await Task.Delay(TimeSpan.FromSeconds(delaySeconds));
                        delaySeconds++;
                    }
                }
                else
                    break;
            }
            catch (HttpRequestException ex)
            {
                retryCount++;
                if (retryCount < maxRetries)
                {
                    Logger.Info($"Retry: {retryCount}, Error: {ex.Message}");
                    Console.WriteLine($"Retry: {retryCount}, StatusCode: {response?.StatusCode}");
                    await Task.Delay(TimeSpan.FromSeconds(delaySeconds));
                    delaySeconds++;
                }
            }
        }

        return response;
    }
}

