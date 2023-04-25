// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

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

}