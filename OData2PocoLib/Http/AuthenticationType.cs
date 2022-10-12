// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.


namespace OData2Poco.Http;

public enum AuthenticationType
{
    None,
    Basic,
    Token,
    Oauth2,
    Ntlm,
    Digest
}