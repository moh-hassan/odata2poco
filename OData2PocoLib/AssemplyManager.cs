// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco;

/// <summary>
///     Manage assemplies which is needed by using statement
///     sources of assemplies are: Attributes , dataType of properties and may be from external via
///     PocoSetting.ExternalAssemplie
/// </summary>
internal class AssemplyManager
{
    private readonly Dictionary<string, string> _assemplyDict = new(StringComparer.OrdinalIgnoreCase)
    {
        //assemplies for attributes
        { "Key", "System.ComponentModel.DataAnnotations" },
        { "Required", "System.ComponentModel.DataAnnotations" },
        { "Req", "System.ComponentModel.DataAnnotations" },
        { "Table", "System.ComponentModel.DataAnnotations.Schema" },
        { "json", "Newtonsoft.Json" }, //extrnal type can be installed from nuget
        { "json3", "System.Text.Json.Serialization" }, //netcore 3+
        //assemplies for Geographic data type
        { "Geometry", "Microsoft.Spatial" }, //extrnal type can be installed from nuget
        { "Geography", "Microsoft.Spatial" }, //extrnal type can be installed from nuget
        { "GeographyPoint", "Microsoft.Spatial" },
        { "DataMember", "System.Runtime.Serialization" },
        { "proto", "ProtoBuf" } //extrnal type can be installed from nuget
    };

    /// <summary>
    ///     Default using assemplies
    /// </summary>
    private readonly string[] _defaultAssemply = { "System", "System.IO", "System.Collections.Generic" };

    private readonly List<ClassTemplate> _model;
    private readonly PocoSetting _pocoSetting;

    /// <summary>
    ///     List of referenced assemplies
    /// </summary>
    public List<string> AssemplyReference;

    /// <summary>
    ///     cto initialization
    /// </summary>
    /// <param name="pocoSetting">Seting parameters of generating code</param>
    /// <param name="model">The model containing all classes </param>
    //public AssemplyManager(PocoSetting pocoSetting, IDictionary<string, ClassTemplate> model)
    public AssemplyManager(PocoSetting pocoSetting, List<ClassTemplate> model)
    {
        _pocoSetting = pocoSetting;
        _model = model;
        AssemplyReference = new List<string>();
        AddAssemply(_defaultAssemply); //add default assemplies
        AddAssemplyReferenceList();
    }

    /// <summary>
    ///     Add assemply or more
    /// </summary>
    /// <param name="list"></param>
    public void AddAssemply(params string[] list)
    {
        foreach (var item in list)
            if (!AssemplyReference.Exists(a => a.Contains(item)))
                AssemplyReference.Add(item);
    }


    private void AddAssemplyByKey(string name)
    {
        if (!_assemplyDict.ContainsKey(name)) return;
        var entry = _assemplyDict[name];
        AddAssemply(entry);
    }

    //initialize AssemplyReference
    private void AddAssemplyReferenceList()
    {
        //Add required namespace for attributes          
        if (_pocoSetting.Attributes.Contains("key")) AddAssemplyByKey("key");
        if (_pocoSetting.Attributes.Contains("req")) AddAssemplyByKey("required");
        if (_pocoSetting.Attributes.Contains("tab")) AddAssemplyByKey("table");
        if (_pocoSetting.Attributes.Contains("json")) AddAssemplyByKey("json");
        if (_pocoSetting.Attributes.Contains("json3")) AddAssemplyByKey("json3"); //netcore 3
        if (_pocoSetting.Attributes.Contains("dm")) AddAssemplyByKey("DataMember");
        if (_pocoSetting.Attributes.Contains("proto")) AddAssemplyByKey("proto");
        AddAssempliesOfDataType(); //add assemplies of datatype
    }

    /// <summary>
    ///     Scan the model to find all uniqe dataTypes which may have referenced or external assemply
    /// </summary>
    /// <returns></returns>
    private void AddAssempliesOfDataType()
    {
        foreach (var type in _model
                     .SelectMany(entry => entry.Properties, (_, property) => property.PropType)
                     .Where(type => !Helper.NullableDataTypes.ContainsKey(type)))
            AddAssemplyByKey(type);
    }
}