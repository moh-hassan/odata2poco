// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

using System.Net;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace OData2Poco;
public static class EncryptionHelper
{
    // A Session 256-bit encryption key
    private static readonly byte[] KeyBytes;

    //A Session 128-bit initialization vector
    private static readonly byte[] IvBytes;
    static EncryptionHelper()
    {
        using RandomNumberGenerator rng = RandomNumberGenerator.Create();
        KeyBytes = new byte[32];
        rng.GetBytes(KeyBytes);
        IvBytes = new byte[16];
        rng.GetBytes(IvBytes);
    }

    public static string EncryptPassword(this string password)
    {
        using var aes = Aes.Create();
        aes.Key = KeyBytes;
        aes.IV = IvBytes;

        var encryptor = aes.CreateEncryptor();
        var passwordBytes = Encoding.UTF8.GetBytes(password);
        var encryptedPasswordBytes = encryptor.TransformFinalBlock(passwordBytes, 0, passwordBytes.Length);
        return Convert.ToBase64String(encryptedPasswordBytes);
    }

    public static string EncryptPassword(this SecureString password)
    {
        return EncryptPassword(new NetworkCredential("", password).Password);
    }

    public static string DecryptPassword(this string encryptedPassword)
    {
        using var aes = Aes.Create();
        aes.Key = KeyBytes;
        aes.IV = IvBytes;

        var decryptor = aes.CreateDecryptor();
        var encryptedPasswordBytes = Convert.FromBase64String(encryptedPassword);
        var passwordBytes = decryptor.TransformFinalBlock(encryptedPasswordBytes, 0, encryptedPasswordBytes.Length);
        return Encoding.UTF8.GetString(passwordBytes);
    }

    public static SecureString ToSecureString(this string plainText)
    {
        if (plainText == null)
            throw new ArgumentNullException(nameof(plainText));

        var secureString = new SecureString();
        foreach (var c in plainText)
            secureString.AppendChar(c);

        secureString.MakeReadOnly();
        return secureString;
    }
    public static NetworkCredential ToCredential(this string password, string userName = "")
    {
        if (password == null)
            throw new ArgumentNullException(nameof(password));

        return new NetworkCredential(userName, password);
    }
    public static NetworkCredential ToCredential(this SecureString password, string userName = "")
    {
        if (password == null)
            throw new ArgumentNullException(nameof(password));

        return new NetworkCredential(userName, password);
    }
}

public interface IEncryption
{
    string EncryptPassword(string password);
    string EncryptPassword(SecureString password);
    string DecryptPassword(string encryptedPassword);

}