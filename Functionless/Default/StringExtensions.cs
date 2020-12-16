using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace System
{
    public static class StringExtensions
    {
        private static JsonSerializerSettings JsonSerializerSettings =
            new JsonSerializerSettings {
                Formatting = Formatting.Indented,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                Converters = new[] { new StringEnumConverter() }
            };

        public static string Join(this IEnumerable<string> values, string separator = "")
        {
            return string.Join(separator, values);
        }

        public static string Format(this string format, params object[] args)
        {
            return string.Format(format, args);
        }

        public static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        public static bool IsNullOrWhiteSpace(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        public static string ToJson<T>(this T value, bool indented = true)
        {
            var formatting = indented ? Formatting.Indented : Formatting.None;
            return JsonConvert.SerializeObject(value, formatting, JsonSerializerSettings);
        }

        public static T FromJson<T>(this string value)
        {
            return value == null ? default : JsonConvert.DeserializeObject<T>(value);
        }
    }
}
