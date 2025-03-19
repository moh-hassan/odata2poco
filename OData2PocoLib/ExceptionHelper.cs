// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco;

using System;
using System.Linq;
using System.Text;

public static class ExceptionHelper
{
    public static string GetExceptionDetails(this Exception ex, bool showTrace = false)
    {
        if (ex == null)
            throw new ArgumentNullException(nameof(ex));

        var sb = new StringBuilder();

        sb.AppendLine($"Exception Type: {ex.GetType().FullName}");
        sb.AppendLine($"Message: {ex.Message}");

        if (ex.Data.Count > 0)
        {
            sb.AppendLine("Data:");
            foreach (DictionaryEntry entry in ex.Data)
            {
                sb.AppendLine($"   {entry.Key}: {entry.Value}");
            }
        }

        // Capture inner exception details if present
        if (ex.InnerException != null)
        {
            sb.AppendLine("Inner Exception:");
            sb.AppendLine(GetExceptionDetails(ex.InnerException));
        }

        if (!showTrace) return sb.ToString();

        // Capture filtered stack trace
        sb.AppendLine("Stack Trace (Filtered):");
        var stackTrace = new StackTrace(ex, true); // Include file info
        var filteredFrames = stackTrace.GetFrames()
            ?.Where(frame => frame.GetFileLineNumber() != 0) // Filter frames with line numbers
            .ToList();

        if (filteredFrames != null && filteredFrames.Count != 0)
        {
            foreach (var frame in filteredFrames)
            {
                sb.AppendLine($"   at {frame.GetMethod()} in {frame.GetFileName()}:line {frame.GetFileLineNumber()}");
            }
        }
        else
        {
            sb.AppendLine("   No stack trace frames with source line numbers available.");
        }

        return sb.ToString();
    }
}
