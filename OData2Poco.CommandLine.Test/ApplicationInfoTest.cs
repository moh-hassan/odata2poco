using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OData2Poco.CommandLine.Test
{
    internal class ApplicationInfoTest
    {
        [Test]
        public void ApplicationInfo_test()
        {
            var versionPattern = @"[0-9a-z.-]+\+[0-9a-z]{9}";
            var titlePattern   = @"O2Pgen\s\(.+\)";
            var headingPattern = $@"^{titlePattern}\sVersion\s{versionPattern}$";

            var version = ApplicationInfo.Version;   //1.2.3+3bbb56990
            Console.WriteLine($"Version: {version}");
            version.Should().MatchRegex($"^{versionPattern}$");

            var title = ApplicationInfo.Title; //O2Pgen (net472)
            Console.WriteLine($"Title: {title}");
            title.Should().MatchRegex(titlePattern);             

            var hi = ApplicationInfo.HeadingInfo;//O2Pgen (net472) Version 1.2.3+3bbb56990
            Console.WriteLine($"HeadingInfo: {hi}");
            hi.Should().MatchRegex(headingPattern);            

            var author = ApplicationInfo.Author;
            Console.WriteLine($"Author: {author}");
            var product = ApplicationInfo.Product;
            Console.WriteLine($"Product: {product}");
            var cr = ApplicationInfo.Copyright;
            Console.WriteLine($"Copyright: {cr}");
            var des = ApplicationInfo.Description;
            Console.WriteLine($"Description: {des}");
        }
    }
}
/*
 * Version: 0.0.0+3bbb56990
Title: O2Pgen (net472)
HeadingInfo: O2Pgen (net472) Version 0.0.0+3bbb56990
Author: Mohamed Hassan
Product: OData2Poco.CommandLine
Copyright: Copyright © 2016-2022, Mohamed Hassan 
Description: o2gen is a CommandLine tool for generating c# and vb.net POCO classes from OData feeds.


*/