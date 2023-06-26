// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

using System.Security;
using FluentAssertions;
using NUnit.Framework;

namespace OData2Poco.Tests.Security
{
    internal class EncryptionHelperTest : BaseTest
    {
        [Test]
        public void Round_trip_encrypt_decrypt_string_test()
        {
            var password = "abc@123";
            string sp = password.EncryptPassword();
            string password2 = sp.DecryptPassword();
            password2.Should().Be(password);
        }

        [Test]
        public void Round_trip_encrypt_decrypt_securestring_test()
        {
            var password = "abc@123";
            SecureString sp = password.ToSecureString();
            var password2 = sp.ToCredential().Password;
            string cipher = password2.EncryptPassword();
            string deCipher = cipher.DecryptPassword();
            deCipher.Should().Be(password);
        }
    }
}
