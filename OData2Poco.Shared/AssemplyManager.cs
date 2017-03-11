using System;
using System.Collections.Generic;
using System.Text;

namespace OData2Poco.Shared
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
        public string[] DefaultAssemply = { "System","System.Collections.Generic"};
        /// <summary>
        /// cto initialization
        /// </summary>
        /// <param name="pocoSetting"></param>
        /// <param name="model"></param>
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
                //Console.WriteLine(item);
                if (!AssemplyReference.Exists(a => a.Contains(item)))
                {
                    AssemplyReference.Add(item);
                    //Console.WriteLine(entry);
                }
            }
        }
        //add all assemplies for using either for attribute or datatype 
        //TODO: load entries from configuration file
        //TODO: Entries may be passed to pocosetting for external references defined by user
        private readonly Dictionary<string, string> _assemplyDict = new Dictionary<string, string>
        {
            //assemplies for attributes
            {"key","System.ComponentModel.DataAnnotations"},
            {"required" ,"System.ComponentModel.DataAnnotations.Schema"},
            {"table" ,"System.ComponentModel.DataAnnotations.Schema"},
            {"json","Newtonsoft.Json"}, //extrnal type can be installed from nuget
            //assemplies for Geographic data type
            {"geometry","Microsoft.Spatial"}, //extrnal type can be installed from nuget
            {"geography", "Microsoft.Spatial"} //extrnal type can be installed from nuget
        };


        private void AddAssemplyByKey(string name)
        {
            name = name.ToLower();
            //Console.WriteLine("try to add " + name);
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
        /// <param name="list"></param>
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
