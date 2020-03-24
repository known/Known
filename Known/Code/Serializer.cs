using System;
using Newtonsoft.Json;

namespace Known
{
    public sealed class Serializer
    {
        public static string ToJson(object value, string dateFormat = "yyyy-MM-dd HH:mm:ss")
        {
            if (value == null)
                return string.Empty;

            var settings = new JsonSerializerSettings { DateFormatString = dateFormat };
            return JsonConvert.SerializeObject(value, settings);
        }

        public static T FromJson<T>(string json, string dateFormat = "yyyy-MM-dd HH:mm:ss")
        {
            if (string.IsNullOrWhiteSpace(json))
                return default;

            var settings = new JsonSerializerSettings { DateFormatString = dateFormat };
            return JsonConvert.DeserializeObject<T>(json, settings);
        }

        public static object FromJson(Type type, string json, string dateFormat = "yyyy-MM-dd HH:mm:ss")
        {
            if (string.IsNullOrWhiteSpace(json))
                return null;

            var settings = new JsonSerializerSettings { DateFormatString = dateFormat };
            return JsonConvert.DeserializeObject(json, type, settings);
        }
    }
}
