// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

using System.Net;
using System.Security;
using OData2Poco.Extensions;

namespace OData2Poco;

public sealed class SecurityContainer : IDisposable
{
    public NetworkCredential Credential { get; }
    public bool IsKeyBoardEntry { get; }
    public bool IsSecuredString { get; }

    public SecurityContainer()
    {
        Credential = new NetworkCredential();
    }
    public SecurityContainer(string password) : this()
    {
        if (password == "?" || password == "-")
        {
            IsKeyBoardEntry = true;
        }
        else
        {
            string cipher = password.EncryptPassword();
            Credential.Password = cipher;
        }
    }

    public SecurityContainer(SecureString password) : this()
    {
        IsSecuredString = true;
        var cipher = password.EncryptPassword();
        Credential.Password = cipher;
    }

    internal string GetRawPassword()
    {
        if (IsSecuredString) //securedString
        {
            var sp = Credential.Password.DecryptPassword();
            return sp;
        }

        var password = Credential.Password.DecryptPassword();
        return password;
    }
    public string GetBasicAuth(string? user)
    {
        if (string.IsNullOrEmpty(user)) return string.Empty;
        var token = $"{user}:{GetRawPassword()}".ToBase64();
        return token;
    }
    public NetworkCredential GetCredential(string? user, string? domain)
    {
        return new(user, GetRawPassword(), domain);
    }
    public string GetToken()
    {
        return GetRawPassword();
    }
    public static SecurityContainer Empty => new();
    public static implicit operator SecurityContainer(string pw) => new(pw);
    public static implicit operator SecurityContainer(SecureString pw) => new(pw);

    public override string ToString()
    {
        return Credential.Password;
    }

    public void Dispose()
    {
        Credential.Password = string.Empty;
        Credential.SecurePassword.Clear();
        Credential.SecurePassword.Dispose();
    }
}