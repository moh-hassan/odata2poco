using System;
using System.Collections.Generic;

namespace OData2Poco.Extensions
{
    public static class UpdateExtensions
    {

        //public delegate void Func<in TArg0>(TArg0 element);

        /// <summary>
        /// Executes an Update statement block on all elements in an IEnumerable<T> sequence.
        /// </summary>
        /// <typeparam name="TSource">The source element type.</typeparam>
        /// <param name="source">The source sequence.</param>
        /// <param name="update">The update statement to execute for each element.</param>
        /// <returns>The numer of records affected.</returns>
        public static int Update<TSource,TArg0>(this IEnumerable<TSource> source, Func<TSource,TArg0> update)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (update == null) throw new ArgumentNullException(nameof(update));
            if (typeof(TSource).IsValueType)
                throw new NotSupportedException("value type elements are not supported by update.");

            var count = 0;
            foreach (TSource element in source)
            {
                update(element);
                count++;
            }
            return count;
        }
    }
}