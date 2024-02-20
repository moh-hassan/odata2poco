// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco.TypeScript;

using System.Text;

internal abstract class SimpleTemplate<T>
    where T : SimpleTemplate<T>
{
    private readonly StringBuilder _sb = new();

    public T AddText(string text)
    {
        _sb.Append(text);
        return (T)this;
    }

    public T AddNewLine()
    {
        _sb.AppendLine();
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

    protected abstract void Build();

    protected string Generate()
    {
        Build();
        return _sb.ToString();
    }
}
