using System.Collections.Generic;

using Functionless.Json;

using Newtonsoft.Json;

namespace System
{
    internal static class StringExtensions
    {
        internal static string Join(this IEnumerable<string> values, string separator = "")
        {
            return string.Join(separator, values);
        }

        internal static bool IsNullOrWhiteSpace(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        internal static string ToJson<T>(this T value, bool indented = true)
        {
            var formatting = indented ? Formatting.Indented : Formatting.None;
            return JsonConvert.SerializeObject(value, formatting, Serializer.Settings);
        }

        internal static T FromJson<T>(this string value)
        {
            return value == null ? default : JsonConvert.DeserializeObject<T>(value);
        }
    }
}
