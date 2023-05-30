// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

using System.Net.Http;
using System.Net;
using OData2Poco.InfraStructure.Logging;

namespace OData2Poco.Extensions;

internal static class Policy
{
    private static readonly ILog Logger = PocoLogger.Default;
   public static async Task<HttpResponseMessage> RetryAsync(Func<Task<HttpResponseMessage>> action, int maxRetries, int delay = 2)
    {
        int retryCount = 0;
        HttpResponseMessage? response = null;
        int delaySeconds = delay;

        while (retryCount < maxRetries)
        {
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
                    await Task.Delay(TimeSpan.FromSeconds(delaySeconds));
                    delaySeconds++;
                }
            }
        }

        return response;
    }
}

