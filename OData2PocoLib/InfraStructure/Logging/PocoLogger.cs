// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco.InfraStructure.Logging;

public class PocoLogger : ColoredConsole
{
    private static readonly Lazy<ILog> Lazy = new(() => new PocoLogger());

    private PocoLogger()
    {
    }

    public static ILog Default => Lazy.Value;
}