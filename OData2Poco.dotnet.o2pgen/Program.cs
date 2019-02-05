using System.Threading.Tasks;
using OData2Poco.CommandLine;

namespace OData2Poco.dotnet.o2pgen
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            await StartUp.Main1(args);
        }
    }
}
