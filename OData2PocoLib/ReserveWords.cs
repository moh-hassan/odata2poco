// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

using OData2Poco.Extensions;

namespace OData2Poco;

public static partial class ReserveWords
{
    private static readonly List<string> CsReserveWords = new()
    {
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
    };

    public static string ChangeReservedWord(this string name)
    {
        if (string.IsNullOrEmpty(name))
            return name;

        return CsReserveWords.Contains(name) ? name.ToggleFirstLetter() : name;
    }

    public static bool IsCSharpReservedWord(this string name)
    {
        return CsReserveWords.Contains(name);
    }

    public static bool IsVbReservedWord(this string name)
    {
        return VbReserveWords.Contains(name);
    }
}