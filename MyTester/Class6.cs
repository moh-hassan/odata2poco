using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ODataDemo;

//http://stackoverflow.com/questions/3862226/dynamically-create-a-class-in-c-sharp
namespace MyTester
{
    public class DynamicClass : DynamicObject
    {
        private Dictionary<string, KeyValuePair<Type, object>> _fields;

        public DynamicClass(List<Field> fields)
        {
            _fields = new Dictionary<string, KeyValuePair<Type, object>>();
            fields.ForEach(x => _fields.Add(x.FieldName,
                new KeyValuePair<Type, object>(x.FieldType, null)));
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            if (_fields.ContainsKey(binder.Name))
            {
                var type = _fields[binder.Name].Key;
                if (value.GetType() == type)
                {
                    _fields[binder.Name] = new KeyValuePair<Type, object>(type, value);
                    return true;
                }
                else throw new Exception(string.Format("Value {0} is not of type {1}",
                    value, type.Name));
            }

            return false;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = _fields[binder.Name].Value;

            return true;
        }
    }

    public class Field
    {
        public Field(string fieldName, Type type)
        {
            FieldName = fieldName;
            FieldType = type;
            // throw new NotImplementedException();
        }

        public string FieldName { get; set; }
        public Type FieldType { get; set; }
    }

    public class TestClass
    {
        
//         store all class fields in a dictionary _fields together with their types and values. The both methods are to can get or set value to some of the properties. You must use the dynamic keyword to create an instance of this class.

//The usage with your example:
    static     public void Test1()
        {
            var fields = new List<Field>()
            {
                new Field("EmployeeID", typeof (int)),
                new Field("EmployeeName", typeof (string)),
                new Field("Designation", typeof (string))
            };

            dynamic obj = new DynamicClass(fields);
            Class1 c = new Class1();
            var text = c.GenerateSchemaForClass(obj.GetType());
            Console.WriteLine(text);

//set
            obj.EmployeeID = 123456;
            obj.EmployeeName = "John";
            obj.Designation = "Tech Lead";

            //obj.Age = 25; //Exception: DynamicClass does not contain a definition for 'Age'
            //obj.EmployeeName = 666; //Exception: Value 666 is not of type String

//get
            Console.WriteLine(obj.EmployeeID); //123456
            Console.WriteLine(obj.EmployeeName); //John
            Console.WriteLine(obj.Designation); //Tech Lead

           
        }
    }
}
