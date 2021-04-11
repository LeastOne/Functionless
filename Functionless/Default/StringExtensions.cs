using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace System
{
    internal static class StringExtensions
    {
        private static JsonSerializerSettings JsonSerializerSettings =
            new JsonSerializerSettings {
                Formatting = Formatting.Indented,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                Converters = new[] { new StringEnumConverter() }
            };

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
            return JsonConvert.SerializeObject(value, formatting, JsonSerializerSettings);
        }

        internal static T FromJson<T>(this string value)
        {
            return value == null ? default : JsonConvert.DeserializeObject<T>(value);
        }
    }
}
