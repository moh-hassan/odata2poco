// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

using System.Net;
using System.Runtime.InteropServices;
using System.Security;

namespace OData2Poco;

public class SecuredPassword
{
    public string? Password { get; set; }
    public SecuredPassword()
    {

    }
    public SecuredPassword(string password)
    {
        Password = password;
    }

    public static implicit operator string?(SecuredPassword sp)
    {
        return sp?.Password;
    }
    public static implicit operator SecuredPassword(string pw)
    {
        return new SecuredPassword(pw);
    }


    //public override string ToString()
    //{
    //    return Password;
    //}

}

//-----------------------------
public static class SecurityExt
{
    public static SecureString ConvertToSecureString(this string password)
    {
        if (password == null)
            throw new ArgumentNullException("password");

        var securePassword = new SecureString();

        password.ToCharArray().ToList().ForEach(securePassword.AppendChar);
        //foreach (char c in password)
        //    securePassword.AppendChar(c);

        securePassword.MakeReadOnly();
        return securePassword;
    }
    public static string? ToUnsecureString(this SecureString source)
    {
        var returnValue = IntPtr.Zero;
        try
        {
            returnValue = Marshal.SecureStringToGlobalAllocUnicode(source);
            return Marshal.PtrToStringUni(returnValue);
        }
        finally
        {
            Marshal.ZeroFreeGlobalAllocUnicode(returnValue);
        }
    }

    public static string? ToBase64(this string? toEncode)
    {
        if (string.IsNullOrEmpty(toEncode)) return null;
        byte[] toEncodeAsBytes = System.Text.Encoding.ASCII.GetBytes(toEncode);
        string returnValue = Convert.ToBase64String(toEncodeAsBytes);
        return returnValue;
    }

    public static string? ToBase64(this SecureString ss)
    {
        var plain = ss.ToUnsecureString().ToBase64();
        return plain;
    }

    public static NetworkCredential ToNetworkCredential(this string password, string userName)
    {
        return new NetworkCredential(userName, password);
    }

    public static NetworkCredential ToNetworkCredential(this SecureString password, string userName)
    {
        return new NetworkCredential(userName, password);
    }
}