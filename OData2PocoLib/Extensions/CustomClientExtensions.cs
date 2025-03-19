// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco.Extensions;

using Http;

internal static class CustomClientExtensions
{
    //public static CustomHttpClient ToCustomHttpClient(this OdataConnectionString ocs,
    //    DelegatingHandler? dh = null)
    //{
    //    return dh == null
    //        ?  CustomHttpClient.CreateAsync(ocs)
    //        : new CustomHttpClient(ocs, dh);
    //}

    public static string GetBasicAuth(this OdataConnectionString ocs)
    {
        return string.IsNullOrEmpty(ocs.UserName)
            || string.IsNullOrEmpty(ocs.Password.Credential.Password)
            ? string.Empty
            : ocs.Password.GetBasicAuth(ocs.UserName);
    }

    public static string GetToken(this OdataConnectionString ocs)
    {
        return string.IsNullOrEmpty(ocs.Password.Credential.Password) ? string.Empty : ocs.Password.GetToken();
    }
}
