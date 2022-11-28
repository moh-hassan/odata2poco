// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

using OData2Poco.InfraStructure.Logging;

namespace OData2Poco.Extensions;

internal static class Policy
{
    private static readonly ILog Logger = PocoLogger.Default;
    public static async Task RetryAsync(Func<Task> action, int n = 3, int delay = 1000)
    {
        for (var i = 0; i < n; i++)
        {
            try
            {
                await action();
                break;
            }
            catch (Exception e) when (i < n)
            {
                Logger.Info($"Retry: {i} {e.Message}");
                await Task.Delay(delay);
            }
        }
    }
}

