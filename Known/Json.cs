using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using Newtonsoft.Json;

namespace Known
{
    public interface IJson
    {
        string Serialize<T>(T value);
        T Deserialize<T>(string json);
    }

    class JsonDefault : IJson
    {
        public string Serialize<T>(T value)
        {
            var serializer = GetJsonSerializer<T>();
            using (var stream = new MemoryStream())
            {
                serializer.WriteObject(stream, value);
                return Encoding.Default.GetString(stream.ToArray());
            }
        }

        public T Deserialize<T>(string json)
        {
            var serializer = GetJsonSerializer<T>();
            var bytes = Encoding.Default.GetBytes(json);
            using (var stream = new MemoryStream(bytes))
            {
                return (T)serializer.ReadObject(stream);
            }
        }

        private DataContractJsonSerializer GetJsonSerializer<T>()
        {
            var settings = new DataContractJsonSerializerSettings
            {
                DateTimeFormat = new DateTimeFormat("yyyy-MM-dd HH:mm:ss")
            };
            return new DataContractJsonSerializer(typeof(T), settings);
        }
    }

    public class JsonProvider : IJson
    {
        public string Serialize<T>(T value)
        {
            return JsonConvert.SerializeObject(value);
        }

        public T Deserialize<T>(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return default;

            var settings = new JsonSerializerSettings
            {
                DateFormatString = "yyyy-MM-dd HH:mm:ss"
            };
            return JsonConvert.DeserializeObject<T>(json, settings);
        }
    }
}
