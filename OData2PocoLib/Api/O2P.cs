// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

using OData2Poco.Extensions;
using OData2Poco.TypeScript;

namespace OData2Poco.Api;

/// <summary>
///     Wrapper class
/// </summary>
public class O2P
{
    public PocoSetting Setting { get; set; }
    public List<ClassTemplate> ClassList { get; set; }
    internal MetaDataInfo MetaData { get; set; }
    public string MetaDataAsString => MetaData.MetaDataAsString;
    public string MetaDataVersion => MetaData.MetaDataVersion;
    public Dictionary<string, string> ServiceHeader => MetaData.ServiceHeader;

    public string CodeText { get; set; }

    //warning due to renaming of reserved keywords
    public List<string> ModelWarning => ModelManager.ModelWarning;

    public O2P(PocoSetting? setting = null)
    {
        Setting = setting ?? new PocoSetting();
        ClassList = new List<ClassTemplate>();
        MetaData = new MetaDataInfo();
        CodeText = "";
    }

    public async Task<IPocoGenerator> GenerateModel(OdataConnectionString odataConnString)
    {
        var gen = await PocoFactory.GenerateModel(odataConnString, Setting);
        MetaData = gen.MetaData;
        ClassList = gen.GeneratePocoList();
        return gen;
    }

    public async Task<IPocoGenerator> GenerateModel(string xmlContent)
    {
        var gen = await PocoFactory.GenerateModel(xmlContent, Setting);
        MetaData = gen.MetaData;
        ClassList = gen.GeneratePocoList();
        return gen;
    }

    //Generate c# code
    public async Task<string> GenerateAsync(OdataConnectionString odataConnString)
    {
        return Setting.Lang switch
        {
            Language.CS => await GenerateCsAsync(odataConnString).ConfigureAwait(false),
            Language.TS => (await GenerateTsAsync(odataConnString).ConfigureAwait(false)).ToString(),
            _ => ""
        };
    }

    //feature request #41
    //Generate c# using xml string.
    public async Task<string> GenerateAsync(string xmlContent)
    {
        var gen = await GenerateModel(xmlContent);
        var generatorCs = PocoClassGeneratorCs.GenerateCsPocoClass(gen, Setting);
        CodeText = generatorCs.ToString();
        return CodeText;
    }
    public async Task<string> GenerateCsAsync(OdataConnectionString odataConnString)
    {
        var gen = await GenerateModel(odataConnString);
        var generatorCs = PocoClassGeneratorCs.GenerateCsPocoClass(gen, Setting);
        CodeText = generatorCs.ToString();
        return CodeText;
    }
    public string GenerateProject()
    {
        var proj = new ProjectGenerator(Setting.Attributes);
        return proj.GetProjectCode();
    }
#if OPENAPI
        public async Task<string> GenerateOpenApiAsync(OdataConnectionString odataConnString,
          int openApiVersion = 3)
        {
            IPocoGenerator gen = await GenerateModel(odataConnString);
            var apiText = gen.GenerateOpenApi(openApiVersion);
            return apiText;
        }
#endif

    //generate typescript
    //[Obsolete("Use GenerateAsync method for both cs and ts. This method will be dropped.")]
    public async Task<PocoStore> GenerateTsAsync(OdataConnectionString odataConnString)
    {
        var gen = await GenerateModel(odataConnString);
        var ts = new TsPocoGenerator(gen, Setting);
        var pocoStore = ts.GeneratePoco();
        return pocoStore;
    }

    public static async Task<string> GeneratePocoAsync(OdataConnectionString connection, PocoSetting setting)
    {
        var o2P = new O2P(setting);
        return await o2P.GenerateAsync(connection);
    }

    public static async Task<string> GeneratePocoAsync(string json)
    {
        var config = json.ToObject<Configuration>();
        if (config == null) return "";
        return await GeneratePocoAsync(config.ConnectionString, config.Setting);
    }
}