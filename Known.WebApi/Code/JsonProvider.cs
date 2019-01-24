using Newtonsoft.Json;

namespace Known.WebApi
{
    public class JsonProvider : IJson
    {
        public string Serialize<T>(T value)
        {
            return JsonConvert.SerializeObject(value);
        }

        public T Deserialize<T>(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return default(T);

            try
            {
                var settings = new JsonSerializerSettings
                {
                    DateFormatString = "yyyy-MM-dd HH:mm:ss"
                };
                return JsonConvert.DeserializeObject<T>(json, settings);
            }
            catch
            {
                return default(T);
            }
        }
    }
}