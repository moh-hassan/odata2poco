namespace OData2Poco.TextTransform
{
    /// <summary>
    /// Specialized CSharp TextTemplate for generating C# code
    /// </summary>
    public class FluentCsTextTemplate : FluentTextTemplate<FluentCsTextTemplate>
    {

        public FluentCsTextTemplate LeftBrace()
        {
            WriteLine("{");
            return this;
        }
        public FluentCsTextTemplate RightBrace()
        {
            WriteLine("}");
            return this;
        }
        public FluentCsTextTemplate UsingNamespace(string name)
        {
            WriteLine("using {0};", name);
            return this;
        }
        public FluentCsTextTemplate StartNamespace(string name)
        {
            WriteLine("namespace {0}", name);
            LeftBrace();
            return this;
        }
        public FluentCsTextTemplate EndNamespace()
        {
            RightBrace();
            return this;
        }

        public FluentCsTextTemplate WriteLineComment(string str)
        {
            // WriteLine("//" + str);
            WriteComment(str);
            NewLine();
            return this;
        }
        public FluentCsTextTemplate WriteComment(string str)
        {
            if (str.Trim().StartsWith(@"//")) Write(str);
            else Write("//" + str);
            return this;
        }
        public FluentCsTextTemplate WriteLineAttribute(string att)
        {
            WriteLine("[{0}]", att);
            return this;
        }

        public FluentCsTextTemplate WriteLineProperty(string typeName, string name,
            string visiblity = "public",
            bool isVirtual = false,
            bool isNullable =false,
            string comment = "")
        {
            if (isVirtual) Write("virtual ");
            Write("{0} ", visiblity);
            //if (!string.IsNullOrEmpty(att))
            //    return WriteLine("{0}\n {1} {2} {3} {{get;set;}}",att, visible, typeName, name);
            Write("{0}{1} {2}  {{get;set;}} ", typeName, isNullable?"?":"" ,name );
            if (!string.IsNullOrEmpty(comment)) WriteComment(comment);
            NewLine();
            return this;
        }

        public FluentCsTextTemplate StartClass(string name, string inherit, string visibility = "public")
        {
            PushTabIndent();// ident one tab
            Write("{0} ", visibility);
            if (string.IsNullOrWhiteSpace(inherit))
                WriteLine("class {0}", name);
            else
                WriteLine("class {0} : {1}", name, inherit);

            LeftBrace();

            PushSpaceIndent(); //prepare for the next write
            return this;
        }

        public FluentCsTextTemplate EndClass()
        {
            PopIndent();
            RightBrace();
            PopIndent();
            return this;
        }

    }
}

