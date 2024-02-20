// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

#pragma warning disable CA1810,CA5401
namespace OData2Poco.Security;

using System.Net;
using System.Security;
using System.Security.Cryptography;
using System.Text;

public static class EncryptionHelper
{
    //A Session 128-bit initialization vector
    private static readonly byte[] s_ivBytes;

    // A Session 256-bit encryption key
    private static readonly byte[] s_keyBytes;

    static EncryptionHelper()
    {
        using var rng = RandomNumberGenerator.Create();
        s_keyBytes = new byte[32];
        rng.GetBytes(s_keyBytes);
        s_ivBytes = new byte[16];
        rng.GetBytes(s_ivBytes);
    }

    public static string EncryptPassword(this string password)
    {
        using var aes = Aes.Create();
        aes.Key = s_keyBytes;
        aes.IV = s_ivBytes;

        var encryptor = aes.CreateEncryptor();
        var passwordBytes = Encoding.UTF8.GetBytes(password);
        var encryptedPasswordBytes = encryptor.TransformFinalBlock(passwordBytes, 0, passwordBytes.Length);
        return Convert.ToBase64String(encryptedPasswordBytes);
    }

    public static string EncryptPassword(this SecureString password)
    {
        return new NetworkCredential(string.Empty, password).Password.EncryptPassword();
    }

    public static string DecryptPassword(this string encryptedPassword)
    {
        using var aes = Aes.Create();
        aes.Key = s_keyBytes;
        aes.IV = s_ivBytes;

        var decryptor = aes.CreateDecryptor();
        var encryptedPasswordBytes = Convert.FromBase64String(encryptedPassword);
        var passwordBytes = decryptor.TransformFinalBlock(encryptedPasswordBytes, 0, encryptedPasswordBytes.Length);
        return Encoding.UTF8.GetString(passwordBytes);
    }

    public static SecureString ToSecureString(this string plainText)
    {
        if (plainText == null)
        {
            throw new ArgumentNullException(nameof(plainText));
        }

        SecureString secureString = new();
        foreach (var c in plainText)
        {
            secureString.AppendChar(c);
        }

        secureString.MakeReadOnly();
        return secureString;
    }

    public static NetworkCredential ToCredential(this string password, string userName = "")
    {
        return password == null ? throw new ArgumentNullException(nameof(password)) : new NetworkCredential(userName, password);
    }

    public static NetworkCredential ToCredential(this SecureString password, string userName = "")
    {
        return password == null ? throw new ArgumentNullException(nameof(password)) : new NetworkCredential(userName, password);
    }
}

public interface IEncryption
{
    string EncryptPassword(string password);
    string EncryptPassword(SecureString password);
    string DecryptPassword(string encryptedPassword);
}
