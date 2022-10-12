// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco.CommandLine;

public class OptionManager
{
    private Options PocoOptions { get; }
    public OptionManager(Options options)
    {
        PocoOptions = options;
        PocoOptions.Validate();

    }
    public PocoSetting GetPocoSetting()
    {
        return new PocoSetting
        {
            Lang = PocoOptions.Lang,
            AddNavigation = PocoOptions.Navigation,
            AddNullableDataType = PocoOptions.AddNullableDataType,
            AddEager = PocoOptions.Eager,
            Inherit = string.IsNullOrWhiteSpace(PocoOptions.Inherit)
                ? string.Empty : PocoOptions.Inherit,
            NamespacePrefix = string.IsNullOrEmpty(PocoOptions.Namespace)
                ? string.Empty : PocoOptions.Namespace,
            NameCase = PocoOptions.NameCase,
            RenameMap = PocoOptions.RenameMap,
            Attributes = PocoOptions.Attributes.ToList(),
            Include = PocoOptions.Include.ToList(),
            EntityNameCase = PocoOptions.EntityNameCase,
            ReadWrite = PocoOptions.ReadWrite,
            EnableNullableReferenceTypes = PocoOptions.EnableNullableReferenceTypes,
            InitOnly = PocoOptions.InitOnly,
            OpenApiFileName = PocoOptions.OpenApiFileName,
            GeneratorType = PocoOptions.GeneratorType,
            MultiFiles = PocoOptions.MultiFiles,
            CodeFilename = PocoOptions.CodeFilename,
            UseFullName = PocoOptions.UseFullName,
        };
    }

    public OdataConnectionString GetOdataConnectionString()
    {
        var connString = new OdataConnectionString
        {
            ServiceUrl = PocoOptions.Url,
            UserName = PocoOptions.User,
            Password = PocoOptions.Password,
            TokenUrl = PocoOptions.TokenEndpoint,
            TokenParams = PocoOptions.TokenParams,
            ParamFile = PocoOptions.ParamFile,
            Domain = PocoOptions.Domain,
            Authenticate = PocoOptions.Authenticate,
            Proxy = PocoOptions.Proxy,
        };
        return connString;
    }

    public static implicit operator Options(OptionManager optionManager)
    {
        return optionManager.PocoOptions;
    }
}