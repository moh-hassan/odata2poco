using System.Collections.Generic;

namespace OData2Poco
{
    //partial extension to implemend methods for creating class attributes
    public partial class ClassTemplate
    {
        /// <summary>
        /// Generate all attributes of the class
        /// </summary>
        /// <param name="setting"></param>
        /// <returns></returns>
        public List<string> GetAttributes(PocoSetting setting)
        {
            var list = new List<string>();
            
            //[Table("depts")]
            if (setting.AddTableAttribute)
            {
                if (EntitySetName != "")
                {
                    list.Add( string.Format("[Table(\"{0}\")]", EntitySetName));
                  
                }
            }

            //in future may be extra attributes or even custom user defined attributes
            //[DataContract]
            //if (setting.AddDataMemberAttribute)
            //{

            //    list.Add(string.Format("[{0}]", "DataContract"));

                
            //}
            return list;
        }
      
     }
}