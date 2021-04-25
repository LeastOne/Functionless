using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Functionless.Json
{
    public static class Serializer
    {
        public static JsonSerializerSettings Settings { get; set; } =
            new JsonSerializerSettings {
                Formatting = Formatting.Indented,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                Converters = new[] { new StringEnumConverter() }
            };

        public static JsonSerializer Default { get; set; } = JsonSerializer.Create(Settings);
    }
}
