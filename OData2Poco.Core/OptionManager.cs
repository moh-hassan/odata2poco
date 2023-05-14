// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

using OData2Poco.Extensions;

namespace OData2Poco.CommandLine;

public class OptionManager
{
    public Options PocoOptions { get; }
    public OptionManager(Options options)
    {
        ReadPassword(options);
        PocoOptions = options;
        PocoOptions.Validate();
    }

    private void ReadPassword(Options options)
    {
        if (!options.Password.IsKeyBoardEntry) return;
        var pw = PasswordReader.ReadPassword("Enter Password/token: ");
        options.Password = new SecuredContainer(pw);
    }
    public void Deconstruct(out OdataConnectionString connectionString, out PocoSetting pocoSetting)
    {
        var json = PocoOptions.ToJson();
        connectionString = json.ToObject<OdataConnectionString>();
        pocoSetting = json.ToObject<PocoSetting>();
    }
}