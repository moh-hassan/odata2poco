// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

using OData2Poco.Http;

namespace OData2Poco.Extensions;

internal static class CustomClientExtensions
{
    public static CustomHttpClient ToCustomHttpClient(this OdataConnectionString ocs,
        DelegatingHandler? dh = null)
    {
        return dh == null
            ? new CustomHttpClient(ocs)
            : new CustomHttpClient(ocs, dh);
    }
    public static string GetBasicAuth(this OdataConnectionString ocs)
    {
        if (string.IsNullOrEmpty(ocs.UserName)
            || string.IsNullOrEmpty(ocs.Password.Credential.Password))
            return string.Empty;
        return ocs.Password.GetBasicAuth(ocs.UserName);
    }
    public static string GetToken(this OdataConnectionString ocs)
    {
        if (string.IsNullOrEmpty(ocs.Password.Credential.Password))
            return string.Empty;
        return ocs.Password.GetToken();
    }
}