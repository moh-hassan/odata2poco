using System.Threading.Tasks;

namespace OData2Poco.CommandLine
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            await StartUp.Main1(args);
        }
		static Program()
    {
        CosturaUtility.Initialize();
    }

    }
}
