// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco.Api;

using Extensions;
using TypeScript;

/// <summary>
///     Wrapper class
/// </summary>
public class O2P
{
    public O2P(PocoSetting? setting = null)
    {
        Setting = setting ?? new PocoSetting();
        ClassList = [];
        MetaData = new MetaDataInfo();
        CodeText = string.Empty;
    }

    public PocoSetting Setting { get; set; }
    public List<ClassTemplate> ClassList { get; set; }
    public string MetaDataAsString => MetaData.MetaDataAsString;
    public string MetaDataVersion => MetaData.MetaDataVersion;
    public Dictionary<string, string> ServiceHeader => MetaData.ServiceHeader;

    public string CodeText { get; set; }

    //warning due to renaming of reserved keywords
    public List<string> ModelWarning => ModelManager.ModelWarning;
    internal MetaDataInfo MetaData { get; set; }

    public static Task<string> GeneratePocoAsync(OdataConnectionString connection, PocoSetting setting)
    {
        O2P o2P = new(setting);
        return o2P.GenerateAsync(connection);
    }

    public static async Task<string> GeneratePocoAsync(string json)
    {
        var config = json.ToObject<Configuration>();
        return config == null
            ? string.Empty
            : await GeneratePocoAsync(config.ConnectionString, config.Setting).ConfigureAwait(false);
    }

    public async Task<IPocoGenerator> GenerateModel(OdataConnectionString odataConnString)
    {
        var gen = await PocoFactory.GenerateModel(odataConnString, Setting).ConfigureAwait(false);
        MetaData = gen.MetaData;
        return gen;
    }

    public async Task<IPocoGenerator> GenerateModel(string xmlContent)
    {
        var gen = await PocoFactory.GenerateModelAsync(xmlContent, Setting).ConfigureAwait(false);
        MetaData = gen.MetaData;
        return gen;
    }

    //Generate c# code
    public async Task<string> GenerateAsync(OdataConnectionString odataConnString)
    {
        return Setting.Lang switch
        {
            Language.CS => await GenerateCsAsync(odataConnString).ConfigureAwait(false),
            Language.TS => (await GenerateTsAsync(odataConnString).ConfigureAwait(false)).ToString(),
            _ => string.Empty
        };
    }

    //feature request #41
    //Generate c# using xml string.
    public async Task<string> GenerateAsync(string xmlContent)
    {
        var gen = await GenerateModel(xmlContent).ConfigureAwait(false);
        var generatorCs = PocoClassGeneratorCs.GenerateCsPocoClass(gen, Setting);
        CodeText = generatorCs.ToString();
        return CodeText;
    }

    public async Task<string> GenerateCsAsync(OdataConnectionString odataConnString)
    {
        var gen = await GenerateModel(odataConnString).ConfigureAwait(false);
        var generatorCs = PocoClassGeneratorCs.GenerateCsPocoClass(gen, Setting);
        CodeText = generatorCs.ToString();
        return CodeText;
    }

    public string GenerateProject()
    {
        ProjectGenerator proj = new(Setting.Attributes);
        return proj.GetProjectCode();
    }

    //generate typescript
    //[Obsolete("Use GenerateAsync method for both cs and ts. This method will be dropped.")]
    public async Task<PocoStore> GenerateTsAsync(OdataConnectionString odataConnString)
    {
        var gen = await GenerateModel(odataConnString).ConfigureAwait(false);
        var ts = new TsPocoGenerator(gen, Setting);
        var pocoStore = ts.GeneratePoco();
        return pocoStore;
    }
}
