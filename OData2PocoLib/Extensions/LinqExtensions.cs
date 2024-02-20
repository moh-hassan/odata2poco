// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco.Extensions;

public static class LinqExtensions
{
    /// <summary>
    ///     Executes an Update statement block on all elements in an IEnumerable T sequence.
    /// </summary>
    /// <typeparam name="TSource">The source element type.</typeparam>
    /// <typeparam name="TArg">The Func return type</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <param name="update">The update statement to execute for each element.</param>
    /// <exception cref="ArgumentNullException">Argument is Null</exception>
    /// <returns>The numer of records affected.</returns>
    public static int Update<TSource, TArg>(this IEnumerable<TSource> source, Func<TSource, TArg> update)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (update == null)
        {
            throw new ArgumentNullException(nameof(update));
        }

        if (typeof(TSource).IsValueType)
        {
            throw new NotSupportedException("Value types (structs) are not supported.");
        }

        var count = 0;
        foreach (var element in source)
        {
            update(element);
            count++;
        }

        return count;
    }

    public static bool IsLast(this List<string> list, string item)
    {
        _ = list ?? throw new ArgumentNullException(nameof(list));
        return list.IndexOf(item) == list.Count - 1;
    }

    public static bool IsNullOrEmpty<T>(this IEnumerable<T>? enumerable)
    {
        return enumerable?.Any() != true;
    }

    public static void Deconstruct<T>(this IList<T> list, out T? head, out IList<T> tail)
    {
        list ??= [];
        if (!list.Any())
        {
            head = default;
            tail = [];
            return;
        }

        head = list.Count > 0 ? list[0] : default;
        tail = list.Skip(1).ToList();
    }

    public static void Deconstruct<T>(this IList<T> list, out T? first, out T? second, out IList<T> rest)
    {
        list ??= [];
        if (!list.Any())
        {
            first = second = default;
            rest = [];
            return;
        }

        first = list.Count > 0 ? list[0] : default;
        second = list.Count > 1 ? list[1] : default;
        rest = list.Skip(2).ToList();
    }

    public static void Deconstruct<TKey, TValue>(
        this KeyValuePair<TKey, TValue> kvp, out TKey key, out TValue value)
    {
        key = kvp.Key;
        value = kvp.Value;
    }

#if !NET
    public static TValue GetValueOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue name)
    {
        _ = dict ?? throw new ArgumentNullException(nameof(dict));
        return dict.TryGetValue(key, out var value) ? value : name;
    }

    public static bool TryAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
    {
        _ = dictionary ?? throw new ArgumentNullException(nameof(dictionary));
        if (!dictionary.ContainsKey(key))
        {
            dictionary.Add(key, value);
            return true;
        }

        return false;
    }

#endif
}
