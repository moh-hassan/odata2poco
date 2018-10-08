using System;
using System.Collections.Generic;

namespace OData2Poco
{
    /// <summary>
    /// Manage assemplies which is needed by using statement 
    /// sources of assemplies are: Attributes , dataType of properties and may be from external via PocoSetting.ExternalAssemplie
    /// </summary>
   public  class AssemplyManager
    {
        /// <summary>
        /// List of referenced assemplies
        /// </summary>
        public  List<string> AssemplyReference;
        private readonly PocoSetting _pocoSetting;
        private readonly IDictionary<string, ClassTemplate> _model;
        /// <summary>
        /// Default using assemplies
        /// </summary>
        public string[] DefaultAssemply = { "System", "System.IO","System.Collections.Generic" };
        /// <summary>
        /// cto initialization
        /// </summary>
        /// <param name="pocoSetting">Seting parameters of generating code</param>
        /// <param name="model">The model containing all classes </param>
        public AssemplyManager(PocoSetting pocoSetting, IDictionary<string, ClassTemplate> model)
        {
            _pocoSetting = pocoSetting;
            _model = model;
            AssemplyReference = new List<string>();
            AddAssemply(DefaultAssemply);//add default assemplies
            AddAssemplyReferenceList();

        }
        /// <summary>
        /// Add assemply or more
        /// </summary>
        /// <param name="list"></param>
        public void AddAssemply(params string[] list)
        {
            foreach (var item in list)
            {
                if (!AssemplyReference.Exists(a => a.Contains(item)))
                {
                    AssemplyReference.Add(item);
                }
            }
        }
        //add all assemplies for using either for attribute or datatype 
        //TODO: load entries from configuration file
        //TODO: Entries may be passed to pocosetting for external references defined by user
        private readonly Dictionary<string, string> _assemplyDict = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
        {
            //assemplies for attributes
            {"Key","System.ComponentModel.DataAnnotations"},
            {"Required" ,"System.ComponentModel.DataAnnotations"},
            {"Table" ,"System.ComponentModel.DataAnnotations.Schema"},
            {"Json","Newtonsoft.Json"}, //extrnal type can be installed from nuget
            //assemplies for Geographic data type
            {"Geometry","Microsoft.Spatial"}, //extrnal type can be installed from nuget
            {"Geography", "Microsoft.Spatial"} ,//extrnal type can be installed from nuget
            {"GeographyPoint","Microsoft.Spatial" }
            
        };


        private void AddAssemplyByKey(string name)
        {
            //name = name.ToLower();
            if (!_assemplyDict.ContainsKey(name)) return;
            var entry = _assemplyDict[name];
            AddAssemply(entry);
            
        }
       
     
        //initialize AssemplyReference
        private void AddAssemplyReferenceList()
        {
            //Add required namespace for attributes
            if (_pocoSetting.AddKeyAttribute) AddAssemplyByKey("key");
            if (_pocoSetting.AddRequiredAttribute) AddAssemplyByKey("required");
            if (_pocoSetting.AddTableAttribute) AddAssemplyByKey("table");
            if (_pocoSetting.AddJsonAttribute) AddAssemplyByKey("json");
            AddAssempliesOfDataType();//add assemplies of datatype
        }

        /// <summary>
        /// Scan the model to find all uniqe dataTypes which may have referenced or external assemply
        /// </summary>
        /// <returns></returns>
        private void AddAssempliesOfDataType()
        {
            foreach (var entry in _model)
            {
                var properties = entry.Value.Properties;
                foreach (var property in properties)
                {
                    var type = property.PropType;
                    if (Helper.NullableDataTypes.ContainsKey(type)) continue; // skip simple data type
                    AddAssemplyByKey(type);
                }
            }
        }
    }
}
