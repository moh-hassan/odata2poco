using System;
using System.Threading.Tasks;

namespace OData2Poco.CommandLine
{
    internal static class VbCodeConvertor
    {
        public static async Task<string> CodeConvert(string code)
        {
            // Individuals do not want to call out to an external service.
            //throw new NotImplementedException();
            await Task.FromResult(false);
            Console.WriteLine("Vb Converter is removed");
            return code;
        }
    }
}