// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

using System.Globalization;
using System.Text;
using OData2Poco.Extensions;

//part of logic is derived from microsoft textTemplate
namespace OData2Poco.TextTransform;

// use a combination of the Builder pattern with generics :
internal class FluentTextTemplate<T> where T : FluentTextTemplate<T>
{
    private bool _endsWithNewline;

    public FluentTextTemplate()
    {
        ToStringHelper = new ToStringInstanceHelper();
        GenerationText = new StringBuilder();
        IndentLengths = new List<int>();
        Footer = "";
        PopIndentText = "";
    }

    public string? Header { get; set; }
    public string Footer { get; set; }

    /// <summary>
    ///     A list of the lengths of each indent that was added with PushIndent
    /// </summary>
    private List<int> IndentLengths { get; }


    /// <summary>
    ///     Gets the current indent we use when adding lines to the output
    /// </summary>
    public string CurrentIndent { get; private set; } = "";

    /// <summary>
    ///     Current transformation session
    /// </summary>
    public virtual IDictionary<string, object>? Session { get; set; }

    /// <summary>
    ///     The string builder that generation-time code is using to assemble generated output
    /// </summary>
    protected StringBuilder GenerationText { get; set; }

    public string PopIndentText { get; set; }


    /// <summary>
    ///     Helper to produce culture-oriented representation of an object as a string
    /// </summary>
    public ToStringInstanceHelper ToStringHelper { get; }


    /// <summary>
    ///     Write formatted text directly into the generated output
    /// </summary>
    public T Write(string format, params object[] args)
    {
        Write(string.Format(CultureInfo.CurrentCulture, format, args));
        return (T)this;
    }

    /// <summary>
    ///     Write text directly into the generated output
    /// </summary>
    public T Write(string textToAppend)
    {
        if (string.IsNullOrEmpty(textToAppend))
            return (T)this;
        // If we're starting off, or if the previous text ended with a newline,
        // we have to append the current indent first.
        if (GenerationText.Length == 0 || _endsWithNewline)
        {
            GenerationText.Append(CurrentIndent);
            _endsWithNewline = false;
        }

        // Check if the current text ends with a newline
        if (textToAppend.EndsWith(Environment.NewLine, StringComparison.CurrentCulture))
            _endsWithNewline = true;
        // This is an optimization. If the current indent is "", then we don't have to do any
        // of the more complex stuff further down.
        if (CurrentIndent.Length == 0)
        {
            GenerationText.Append(textToAppend);
            return (T)this;
        }

        // Everywhere there is a newline in the text, add an indent after it
        textToAppend = textToAppend.Replace(Environment.NewLine, Environment.NewLine + CurrentIndent);
        // If the text ends with a newline, then we should strip off the indent added at the very end
        // because the appropriate indent will be added when the next time Write() is called
        if (_endsWithNewline)
            GenerationText.Append(textToAppend, 0, textToAppend.Length - CurrentIndent.Length);
        else
            GenerationText.Append(textToAppend);
        return (T)this;
    }

    /// <summary>
    ///     Write text directly into the generated output
    /// </summary>
    public T WriteLine(string textToAppend)
    {
        Write(textToAppend);
        GenerationText.AppendLine();
        _endsWithNewline = true;
        return (T)this;
    }

    /// <summary>
    ///     write char n times
    /// </summary>
    /// <param name="c"></param>
    /// <param name="n"></param>
    /// <returns></returns>
    public T WriteLine(char c, int n)
    {
        var textToAppend = new string(c, n);
        WriteLine(textToAppend);
        return (T)this;
    }


    /// <summary>
    ///     Write formatted text directly into the generated output
    /// </summary>
    public T WriteLine(string format, params object[] args)
    {
        WriteLine(string.Format(CultureInfo.CurrentCulture, format, args));
        return (T)this;
    }


    /// <summary>
    ///     PushIndent space n times
    /// </summary>
    /// <param name="n"></param>
    /// <returns></returns>
    public T PushSpaceIndent(int n = 4)
    {
        var s = new string(' ', n);
        PushIndent(s);
        return (T)this;
    }

    /// <summary>
    ///     PushIndent tab n times
    /// </summary>
    /// <param name="n"></param>
    /// <returns></returns>
    public T PushTabIndent(int n = 1)
    {
        var s = new string('\t', n);
        PushIndent(s);
        return (T)this;
    }

    /// <summary>
    ///     Increase the indent
    /// </summary>
    public T PushIndent(string indent)
    {
        CurrentIndent += indent ?? throw new ArgumentNullException(nameof(indent));
        IndentLengths.Add(indent.Length);
        return (T)this;
    }

    /// <summary>
    ///     Remove the last indent that was added with PushIndent
    /// </summary>
    public T PopIndent()
    {
        var returnValue = "";
        if (IndentLengths.Count > 0)
        {
            var indentLength = IndentLengths[IndentLengths.Count - 1];
            IndentLengths.RemoveAt(IndentLengths.Count - 1);
            if (indentLength > 0)
            {
                returnValue = CurrentIndent[^indentLength..];
                CurrentIndent = CurrentIndent.Remove(CurrentIndent.Length - indentLength);
            }
        }

        PopIndentText = returnValue;
        return (T)this;
    }

    /// <summary>
    ///     Remove any indentation
    /// </summary>
    public T ClearIndent()
    {
        IndentLengths.Clear();
        CurrentIndent = "";
        return (T)this;
    }

    public T NewLine()
    {
        WriteLine("");
        return (T)this;
    }

    public T Tab(int n = 1)
    {
        for (var i = 0; i < n; i++) Write("\t");

        return (T)this;
    }


    public T WriteIf(bool condition, string ifTrue)
    {
        if (condition)
            Write(ifTrue);
        return (T)this;
    }

    public T WriteIf(bool condition, string ifTrue, string ifFalse)
    {
        return condition
            ? Write(ifTrue)
            : Write(ifFalse);
    }

    public T WriteIf(bool condition,
        Action<T> ifTrue,
        Action<T> ifFalse)
    {
        if (condition)
            ifTrue((T)this);
        else
            ifFalse((T)this);
        return (T)this;
    }

    public T WriteList(List<string> list, string separator = " ")
    {
        list.ForEach(x =>
        {
            if (!string.IsNullOrEmpty(x))
                Write(list.IsLast(x) ? $"{x}" : $"{x}{separator}");
        });
        return (T)this;
    }

    //text is string with a char separator <, ; : space>
    public T WriteList(string text, string separator = " ")
    {
        var list = text.Split(new[] { ',', ';', ':', ' ' }, StringSplitOptions.RemoveEmptyEntries)
            .ToList();
        return WriteList(list, separator);
    }

    public T WriteLineFor(List<string> list, string separator = ",", string indent = "\t")
    {
        foreach (var p in list)
        {
            var comma = list.IsLast(p) ? "" : separator;
            PushIndent(indent)
                .WriteLine($"{p.Trim()} {comma}")
                .PopIndent();
        }

        return (T)this;
    }

    public T WriteLineFormatFor(List<string> list, string format)
    {
        foreach (var p in list) WriteLine(string.Format(format, p));
        return (T)this;
    }

    public T SaveToPocoStore(PocoStore ps)
    {
        return SaveToPocoStor(ps, "Poco", "", "Poco");
    }

    public T SaveToPocoStor(PocoStore ps,
        string name,
        string codenamespace = "",
        string fullName = "")
    {
        ps.Add(new PocoStoreEntry { Name = name, Namespace = codenamespace, FullName = fullName, Code = ToString() });
        return (T)this;
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        if (!string.IsNullOrEmpty(Header))
            sb.AppendLine(Header);
        sb.Append(GenerationText);
        if (!string.IsNullOrEmpty(Footer))
            sb.AppendLine().AppendLine(Footer);
        return sb.ToString();
    }


    /// <summary>
    ///     Utility class to produce culture-oriented representation of an object as a string.
    /// </summary>
    public class ToStringInstanceHelper
    {
        private IFormatProvider? _formatProvider = CultureInfo.InvariantCulture;

        /// <summary>
        ///     Gets or sets format provider to be used by ToStringWithCulture method.
        /// </summary>
        public IFormatProvider? FormatProvider
        {
            get => _formatProvider;
            set
            {
                if (value != null)
                    _formatProvider = value;
            }
        }
    }
}