// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

#if !NETCOREAPP
using System.Reflection;
#endif

namespace OData2Poco.CommandLine.Test
{
    public class BaseTest
    {

        public BaseTest()
        {
            Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
            //in case multi target project,use #if because no appdomain in netcore
#if !NETCOREAPP
            /* Preparing test start */
            Assembly assembly = Assembly.GetCallingAssembly();

            AppDomainManager manager = new AppDomainManager();
            FieldInfo entryAssemblyfield = manager.GetType()
                .GetField("m_entryAssembly", BindingFlags.Instance | BindingFlags.NonPublic);
            entryAssemblyfield?.SetValue(manager, assembly);

            AppDomain domain = AppDomain.CurrentDomain;
            FieldInfo domainManagerField = domain.GetType()
                .GetField("_domainManager", BindingFlags.Instance | BindingFlags.NonPublic);
            domainManagerField?.SetValue(domain, manager);
            /* Preparing test end */
#endif
        }
    }
}