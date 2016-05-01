using System;
using System.CodeDom;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
//(c) 2016 Mohamed Hassan
// MIT License
//project site: http://odata2poco.codeplex.com/
namespace OData2Poco.CommandLine
{
    class Program
    {
        private static Stopwatch sw = new Stopwatch();
        static void Main(string[] args)
        {

            RunOptions(args);


#if DEBUG
            Console.ReadKey();
#endif
        }

        static void RunOptions(string[] args)
        {
            sw.Start();
            var options = new Options();

            if (global::CommandLine.Parser.Default.ParseArguments(args, options))
            {
                //var title = ApplicationInfo.Product + " Version " + ApplicationInfo.Version;
                Console.WriteLine(ApplicationInfo.HeadingInfo);
                Console.WriteLine(ApplicationInfo.Copyright);
                Console.WriteLine(ApplicationInfo.Description);
                Console.WriteLine("Start processing url: " + options.Url);
                ProcessComandLine(options);
            }
            //else
            //{
            //    // Display the default usage information
            //    Console.WriteLine(options.GetUsage());
            //}

        }

        static void ProcessComandLine(Options options)
        {
            //var h =    HelpText.AutoBuild(options);
            // var o=  h.Heading;
            //   Console.WriteLine(o);
            //   options.GetUsage();
            if (options.Url == null) return;
            O2P o2p = options.User == null
                ? new O2P(options.Url)
                : new O2P(options.Url, options.User, options.Password);

            var code = o2p.Generate();
            Console.WriteLine("Saving generated code to file : " + options.CodeFilename);
            File.WriteAllText(options.CodeFilename, code);
            //------------ header -h --------------------
            if (options.Header)
            {
                Console.WriteLine();
                Console.WriteLine("HTTP Header");
                Console.WriteLine(new string('=', 15));
                o2p.ServiceHeader.ToList().ForEach(m =>
                {
                    Console.WriteLine(" {0}: {1}", m.Key, m.Value);
                });
            }

            //---------metafile -m
            if (options.MetaFilename != null)
            {
                Console.WriteLine();
                o2p.SaveMetadata(options.MetaFilename);
                Console.WriteLine("Saving Metadata to file : " + options.MetaFilename);
            }
            //---------list -l 
            if (options.ListPoco)
            {
                Console.WriteLine();
                Console.WriteLine("POCO classes (count: {0})", o2p.ClassList.Count);
                Console.WriteLine(new string('=', 20));
                var items = o2p.ClassList.OrderBy(m => m.Name).ToList();
                items.ForEach(m =>
                {
                    int index = items.IndexOf(m);
                    Console.WriteLine("{0}: {1}", index + 1, m.Name);
                });
            }
            //---------verbose -v
            if (options.Verbose)
            {
                Console.WriteLine();
                Console.WriteLine(code);
            }
            sw.Stop();
            Console.WriteLine();
            Console.WriteLine("Total processing time: {0} sec", sw.ElapsedMilliseconds / 1000.0);

        }

    }
}
