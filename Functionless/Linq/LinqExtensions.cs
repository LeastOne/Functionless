using System.Collections.Generic;

namespace System.Linq
{
    internal static class LinqExtensions
    {
        internal static IDictionary<T1, T2> ToDictionary<T1, T2>(this IEnumerable<(T1 Name, T2 Value)> source)
        {
            return source.ToDictionary(tuple => tuple.Name, tuple => tuple.Value);
        }
    }
}
