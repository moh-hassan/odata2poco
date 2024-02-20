// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco;

using Extensions;

public static partial class ReserveWords
{
    private static readonly List<string> s_csReserveWords =
    [
        "abstract",
        "as",
        "base",
        "bool",
        "break",
        "byte",
        "case",
        "catch",
        "char",
        "checked",
        "class",
        "const",
        "continue",
        "decimal",
        "default",
        "delegate",
        "do",
        "double",
        "else",
        "enum",
        "event",
        "explicit",
        "extern",
        "false",
        "finally",
        "fixed",
        "float",
        "for",
        "foreach",
        "goto",
        "if",
        "implicit",
        "in",
        "int",
        "interface",
        "internal",
        "is",
        "lock",
        "long",
        "namespace",
        "new",
        "null",
        "object",
        "operator",
        "out",
        "override",
        "params",
        "private",
        "protected",
        "public",
        "readonly",
        "ref",
        "return",
        "sbyte",
        "sealed",
        "short",
        "sizeof",
        "stackalloc",
        "static",
        "string",
        "struct",
        "switch",
        "this",
        "throw",
        "true",
        "try",
        "typeof",
        "uint",
        "ulong",
        "unchecked",
        "unsafe",
        "ushort",
        "using",
        "using",
        "static",
        "virtual",
        "void",
        "volatile",
        "while"
    ];

    public static string ChangeReservedWord(this string name)
    {
        var changeReservedWord = s_csReserveWords.Contains(name) ? name.ToggleFirstLetter() : name;
        return string.IsNullOrEmpty(name) ? name : changeReservedWord;
    }

    public static bool IsCSharpReservedWord(this string name)
    {
        return s_csReserveWords.Contains(name);
    }

    public static bool IsVbReservedWord(this string name)
    {
        return s_vbReserveWords.Contains(name);
    }
}
