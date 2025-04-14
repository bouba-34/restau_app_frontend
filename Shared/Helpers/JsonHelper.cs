using System.Text.Json;
using System.Text.Json.Serialization;

namespace Shared.Helpers
{
    public static class JsonHelper
    {
        private static readonly JsonSerializerOptions Options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters = { new JsonStringEnumConverter() }
        };

        public static string Serialize<T>(T obj)
        {
            return JsonSerializer.Serialize(obj, Options);
        }

        public static T Deserialize<T>(string json)
        {
            return JsonSerializer.Deserialize<T>(json, Options);
        }

        public static T DeserializeOrDefault<T>(string json) where T : new()
        {
            if (string.IsNullOrEmpty(json))
                return new T();

            try
            {
                return Deserialize<T>(json);
            }
            catch
            {
                return new T();
            }
        }
    }
}