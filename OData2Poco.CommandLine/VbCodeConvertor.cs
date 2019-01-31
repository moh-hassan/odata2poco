using System;
using System.Threading.Tasks;

namespace OData2Poco.CommandLine
{
    internal static class VbCodeConvertor
    {
        public static async Task<string> CodeConvert(string code)
        {
            //throw new NotImplementedException();
            var vbCode = await CodeConvertorRestService.CodeConvert(code);
            return vbCode;
        }
    }
}