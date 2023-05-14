// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

using System.Net;
using System.Security;

namespace OData2Poco;

public sealed class SecuredContainer : IDisposable
{
    internal NetworkCredential Credential { get; }
    public bool IsKeyBoardEntry { get; }

    private SecuredContainer()
    {
        Credential = new NetworkCredential();
    }
    public static SecuredContainer Empty => new();
    public SecuredContainer(string password) : this()
    {
        if (password == "?" || password == "-")
        {
            IsKeyBoardEntry = true;
            Credential = new NetworkCredential();
        }
        else
        {
            Credential = new NetworkCredential(string.Empty, ToSecureString(password));
        }
    }

    public SecuredContainer(SecureString password)
    {
        Credential = new NetworkCredential(string.Empty, password);
    }

    internal static SecureString ToSecureString(string? password)
    {
        SecureString secureString = new();
        if (string.IsNullOrEmpty(password))
            return secureString;
        foreach (char c in password)
            secureString.AppendChar(c);
        return secureString;
    }

    private void SetPassword(string? password) => Credential.SecurePassword = ToSecureString(password);
    public void SetPassword(SecureString password) => Credential.SecurePassword = password;
    public NetworkCredential SetUpCredential(string? userName, string? domain)
    {
        if (!string.IsNullOrWhiteSpace(userName))
            Credential.UserName = userName;
        if (!string.IsNullOrWhiteSpace(domain))
            Credential.Domain = domain;
        return Credential;
    }

    public static implicit operator NetworkCredential(SecuredContainer sp) => sp.Credential;

    public static implicit operator SecuredContainer(string pw) => new(pw);

    public void Dispose()
    {
        Credential.SecurePassword.Clear();
        Credential.SecurePassword.Dispose();
        Credential.UserName = null;
    }
}