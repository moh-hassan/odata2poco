namespace OData2Poco.CommandLine
{
    using System.Threading.Tasks;

    public static class Program
    {
        public static async Task Main(string[] args)
        {
            await StartUp.Run(args);
        }
    }
}
