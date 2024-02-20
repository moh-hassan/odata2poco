// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco.Tests.Security;

using OData2Poco.Security;

public class EncryptionHelperTest : BaseTest
{
    [Test]
    public void Round_trip_encrypt_decrypt_string_test()
    {
        var password = "abc@123";
        var sp = password.EncryptPassword();
        var password2 = sp.DecryptPassword();
        password2.Should().Be(password);
    }

    [Test]
    public void Round_trip_encrypt_decrypt_securestring_test()
    {
        var password = "abc@123";
        var sp = password.ToSecureString();
        var password2 = sp.ToCredential().Password;
        var cipher = password2.EncryptPassword();
        var deCipher = cipher.DecryptPassword();
        deCipher.Should().Be(password);
    }
}
