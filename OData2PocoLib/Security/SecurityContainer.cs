// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

#pragma warning disable CA1721, SA1202
namespace OData2Poco.Security;

using System.Net;
using System.Security;
using Extensions;

public sealed class SecurityContainer : IDisposable
{
    public SecurityContainer()
    {
        Credential = new NetworkCredential();
    }

    public SecurityContainer(string password) : this()
    {
        if (password is "?" or "-")
        {
            IsKeyBoardEntry = true;
        }
        else
        {
            var cipher = password.EncryptPassword();
            Credential.Password = cipher;
        }
    }

    public SecurityContainer(SecureString password) : this()
    {
        IsSecuredString = true;
        var cipher = password.EncryptPassword();
        Credential.Password = cipher;
    }

    public static SecurityContainer Empty => new();

    public NetworkCredential Credential { get; }
    public bool IsKeyBoardEntry { get; }
    public bool IsSecuredString { get; }

    public static implicit operator SecurityContainer(string pw)
    {
        return new(pw);
    }

    public static implicit operator SecurityContainer(SecureString pw)
    {
        return new(pw);
    }

    public string GetBasicAuth(string? user)
    {
        if (string.IsNullOrEmpty(user))
        {
            return string.Empty;
        }

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

    public override string ToString()
    {
        return Credential.Password;
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

    public void Dispose()
    {
        Credential.Password = string.Empty;
        Credential.SecurePassword.Clear();
        Credential.SecurePassword.Dispose();
    }
}
