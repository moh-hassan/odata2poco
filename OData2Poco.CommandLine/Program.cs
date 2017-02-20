using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using CommandLine;
using OData2Poco.Extension;
using O2P = OData2Poco.O2P;
//todo: file source
//(c) 2016 Mohamed Hassan
// MIT License
//project site: http://odata2poco.codeplex.com/
namespace OData2Poco.CommandLine
{
    class Program
    {
        private static readonly Stopwatch Sw = new Stopwatch();
        private static PocoSetting _PocoSetting = new PocoSetting();

        // [STAThread]
        static void Main(string[] args)
        {

            try
            {

                //// Catch all unhandled exceptions in all threads.
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

                Task t = RunOptionsAsync(args);
                t.Wait();
                Environment.Exit(0);
#if DEBUG
                 Console.ReadKey();
#endif
            }
            catch (Exception ex)
            {

                var argument = string.Join(" ", args);
                Console.WriteLine("Error in executing the command: o2pgen {0}", argument);
#if DEBUG
                Console.WriteLine("Error Message:\n {0}", ex.FullExceptionMessage(true));
#else
                Console.WriteLine("Error Message:\n {0}", ex.FullExceptionMessage());
#endif
                //Console.WriteLine("Error Details: {0}", ex);
                Environment.Exit(-1);
            }


        }


        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {

            Console.WriteLine("Unhandled exception: {0}", (e.ExceptionObject as Exception).Message);
            Environment.Exit(-99);
        }
        static async Task RunOptionsAsync(string[] args)
        {
            Sw.Start();
            var options = new Options();

            if (Parser.Default.ParseArguments(args, options))
            {

                Console.WriteLine(ApplicationInfo.HeadingInfo);
                Console.WriteLine(ApplicationInfo.Copyright);
                Console.WriteLine(ApplicationInfo.Description);
                Console.WriteLine("Start processing url: " + options.Url);
                await ProcessComandLineAsync(options);
            }
        }

        static async Task ProcessComandLineAsync(Options options)
        {
            //------- PocoSetting------
            if (options.Key) _PocoSetting.AddKeyAttribute = true;
            if (options.Table) _PocoSetting.AddTableAttribute = true;
            if (options.Required) _PocoSetting.AddRequiredAttribute = true;
            if (options.Navigation) _PocoSetting.AddNavigation = true;
            if (options.Eager) _PocoSetting.AddEager = true;
            if (options.AddNullableDataType) _PocoSetting.AddNullableDataType = true;
            _PocoSetting.Inherit = string.IsNullOrWhiteSpace(options.Inherit) ? string.Empty : options.Inherit;
            _PocoSetting.NamespacePrefix = string.IsNullOrWhiteSpace(options.Namespace) ? string.Empty : options.Namespace;

            //  if (options.Url == null) return;
            //O2P o2p = options.User == null
            //    ? new O2P(new Uri(options.Url), _PocoSetting)
            //    : new O2P(new Uri(options.Url), options.User, options.Password, _PocoSetting);

            O2P o2p = new O2P(_PocoSetting);
            string code = "";
            if (options.Url.StartsWith("http"))
            {
                code = await o2p.GenerateAsync(new Uri(options.Url), options.User, options.Password); //
            }
            else
            {
                var xml = File.ReadAllText(options.Url);
                code = o2p.Generate(xml);

            }
            //   Console.WriteLine(code);
            //.Generate(_PocoSetting);
            Console.WriteLine("Saving generated code to file : " + options.CodeFilename);
            SaveToFile(options.CodeFilename, code, " c# code is empty");


            //---------metafile -m
            if (options.MetaFilename != null)
            {
                Console.WriteLine();
                //SaveMetaDataTo(options.MetaFilename);
                Console.WriteLine("Saving Metadata to file : {0}", options.MetaFilename);
                var metaData = FormatXml(o2p.MetaDataAsString);
                SaveToFile(options.MetaFilename, metaData, " Metadata is empty");
            }

            //------------ header -h for http media only not file--------------------
            if (options.Header && options.Url.StartsWith("http"))
            {
                //   MetaDataInfo meta = o2p;
                Console.WriteLine();
                Console.WriteLine("HTTP Header");
                Console.WriteLine(new string('=', 15));
                //  o2p.MetaData.ServiceHeader.ToList().ForEach(m =>
                o2p.ServiceHeader.ToList().ForEach(m =>
                {
                    Console.WriteLine(" {0}: {1}", m.Key, m.Value);
                });
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
                    var remoteUrl = string.IsNullOrEmpty(m.EntitySetName) ? "" : options.Url + @"/" + m.EntitySetName;
                    //  Console.WriteLine("{0}: {1} ", index + 1, m.Name);
                    //v1.5
                    Console.WriteLine("{0}: {1} {2}", index + 1, m.Name, remoteUrl);
                });
            }
            //---------verbose -v
            if (options.Verbose)
            {
                Console.WriteLine();
                Console.WriteLine(code);
            }


            Sw.Stop();
            Console.WriteLine();
            Console.WriteLine("Total processing time: {0} sec", Sw.ElapsedMilliseconds / 1000.0);

        }

        public static bool SaveToFile(string fname, string text, string errorMsg)
        {
            //todo:check file exist
            if (string.IsNullOrEmpty(text))
                throw new Exception(errorMsg);
            FileInfo file = new FileInfo(fname);
            file.Directory.Create(); // If the directory already exists, this method does nothing.
            File.WriteAllText(file.FullName, text);
            File.WriteAllText(fname, text);
            long length = new FileInfo(fname).Length;
            if (length == 0) throw new Exception(fname + " is empty");
            return true;
        }



        public static string FormatXml(string xml)
        {
            XDocument doc = XDocument.Parse(xml);
            return doc.ToString();
        }

    }
}
