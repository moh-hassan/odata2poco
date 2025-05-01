// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco.Http;
using System;
using System.Linq;
using System.Net.NetworkInformation;
internal static class PortChecker
{
    public static bool IsPortInUse(int port, out string proceeName)
    {
        proceeName = string.Empty;
        var ipProperties = IPGlobalProperties.GetIPGlobalProperties();
        var tcpConnections = ipProperties.GetActiveTcpListeners();

        if (tcpConnections.Any(endpoint => endpoint.Port == port))
        {
            var process = GetProcessUsingPort(port);
            if (process != null)
            {
                proceeName = process.ProcessName;
            }
            return true;
        }

        return false;
    }
    private static Process? GetProcessUsingPort(int port)
    {
        // Use netstat to find the process ID (PID) using the port
        var startInfo = new ProcessStartInfo
        {
            FileName = "netstat",
            Arguments = "-ano",
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = Process.Start(startInfo);
        if (process == null) return null;

        var output = process.StandardOutput.ReadToEnd();
        process.WaitForExit();

        var lines = output.Split([Environment.NewLine], StringSplitOptions.RemoveEmptyEntries);
        foreach (var line in lines)
        {
            if (line.Contains($"0.0.0.0:{port}") || line.Contains($"127.0.0.1:{port}"))
            {
                var parts = line.Split([' '], StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length > 4 && int.TryParse(parts[^1], out var pid))
                {
                    return Process.GetProcessById(pid);
                }
            }
        }

        return null;
    }
}
