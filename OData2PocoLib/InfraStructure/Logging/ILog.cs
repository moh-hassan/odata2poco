// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

#pragma warning disable CA1716 // Identifiers should not match keywords
namespace OData2Poco.InfraStructure.Logging;

using System.Text;

public interface ILog
{
    StringBuilder Output { get; set; }
    bool Silent { get; set; }
    void Debug(string msg);
    void Trace(string msg);
    void Warn(string msg);
    void Warn(Func<string> message);
    void Info(string msg);
    void Info(Func<string> message);
    void Error(string msg);
    void Error(Func<string> message);
    void Fatal(string msg);
    void Success(string msg);
    void Confirm(string msg);
    void Normal(string msg);
    void Clear();
}
