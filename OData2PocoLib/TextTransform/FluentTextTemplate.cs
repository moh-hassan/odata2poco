// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco.TextTransform;

internal class FluentTextTemplate : FluentTextTemplate<FluentTextTemplate>
{
    public static implicit operator string(FluentTextTemplate ft)
    {
        return ft.ToString();
    }
}
