// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco.InfraStructure.Logging;

public sealed class PocoLogger : ColoredConsole
{
    private static readonly Lazy<ILog> s_lazy = new(() => new PocoLogger());

    private PocoLogger()
    {
    }

    public static ILog Default => s_lazy.Value;
}
