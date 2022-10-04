// Copyright 2016-2022 Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

using System.Text;

namespace OData2Poco.TypeScript
{
    internal abstract class SimpleTemplate<T> where T : SimpleTemplate<T>
    {
        readonly StringBuilder sb;
        protected SimpleTemplate()
        {
            sb = new StringBuilder();
        }
        protected abstract void Build();
        protected string Generate()
        {
            Build();
            return sb.ToString();
        }
        public T AddText(string text)
        {
            sb.Append(text);
            return (T)this;
        }        
        public T AddNewLine()
        {
            sb.AppendLine();
            return (T)this;
        }
        public T LineTerminator()
        {
            return AddText(";");
        }
        public T Colon()
        {
            return AddText(": ");
        }
    }
}