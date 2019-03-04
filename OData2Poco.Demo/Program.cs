using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OData2Poco.Api;
/*********************************************
 *Install-Package OData2Poco -Version 3.1.0 
 *
 */
namespace OData2Poco.Demo
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello OData2Poco Demo!");
          // string client_id="<Enter client id /or application id here>";
          // string client_secret="<Enter client secret  here>";

            //define connection parameters
            var connString = new OdataConnectionString
            {
                ServiceUrl = "http://services.odata.org/V4/OData/OData.svc",
                Authenticate = AuthenticationType.none,

                //for oauth2
                //Authenticate = AuthenticationType.oauth2,
                //UserName = client_id,  
                //Password = client_secret,  
                //TokenParams = "resource=...",
                //TokenUrl = "https://url/of/tokenserver",
                
            };
            var setting = new PocoSetting
            {
                Attributes = new List<string> { "key"},
                AddNavigation = true,
                AddNullableDataType = true,
            };

            try
            {
                var o2p = new O2P(setting);
                var code = await o2p.GenerateAsync(connString);
                Console.WriteLine(code);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Console.WriteLine("Press any key to exit");
                Console.ReadKey();
            }
            
          
        }
    }
}
