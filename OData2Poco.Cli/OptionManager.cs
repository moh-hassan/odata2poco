// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco.CommandLine;

using Security;

public class OptionManager
{
    public OptionManager(Options options)
    {
        _ = options ?? throw new ArgumentNullException(nameof(options));
        ReadPassword(options);
        PocoOptions = options;
        PocoOptions.Validate();
    }

    public Options PocoOptions { get; }

    public void Deconstruct(out OdataConnectionString connectionString, out PocoSetting? pocoSetting)
    {
        var json = PocoOptions.ToJson();
        connectionString = json.ToObject<OdataConnectionString>() ??
                           throw new InvalidOperationException("ODataConnectionString is null");
        pocoSetting = json.ToObject<PocoSetting>();
    }

    private void ReadPassword(Options options)
    {
        if (!options.Password.IsKeyBoardEntry) return;
        var pw = PasswordReader.ReadPassword("Enter Password/token: ");
        options.Password = new SecurityContainer(pw);
    }
}
