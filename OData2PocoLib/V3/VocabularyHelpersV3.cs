// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

using Microsoft.Data.Edm;

namespace OData2Poco.V3;
#pragma warning disable IDE0060
internal static class VocabularyHelpersV3
{
    //Computed and Permissions Vocabulary are not supported in OData V3
    internal static bool IsReadOnly(this IEdmModel model, IEdmProperty property)
    {
        return false;
    }
}