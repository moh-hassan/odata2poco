// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco.CommandLine.Test;

#if !NETCOREAPP
using System.Reflection;
#endif

public class BaseTest
{
    public BaseTest()
    {
        Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
        //in case multi target project,use #if because no appdomain in netcore
#if !NETCOREAPP
        /* Preparing test start */
        var assembly = Assembly.GetCallingAssembly();
        var manager = new AppDomainManager();
        var entryAssemblyField = manager.GetType()
            .GetField("m_entryAssembly", BindingFlags.Instance | BindingFlags.NonPublic);
        entryAssemblyField?.SetValue(manager, assembly);

        var domain = AppDomain.CurrentDomain;
        var domainManagerField = domain.GetType()
            .GetField("_domainManager", BindingFlags.Instance | BindingFlags.NonPublic);
        domainManagerField?.SetValue(domain, manager);
        /* Preparing test end */
#endif
    }

    /// <summary>
    /// create temp file in user temp folder
    /// </summary>
    /// <param name="content"></param>
    /// <param name="extension"> extension w/o . like .txt or txt</param>
    /// <returns></returns>
    //extension ".txt"
    protected string NewTempFile(string content, string extension = null)
    {
        var filepath = Path.GetTempFileName();
        if (!string.IsNullOrEmpty(extension))
        {
            extension = extension.TrimStart('.');
            filepath = Path.ChangeExtension(Path.GetTempFileName(), $".{extension}");
        }

        File.WriteAllText(filepath, content);
        return filepath;
    }
}
