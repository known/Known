using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Known
{
    public interface IJsonProvider
    {
        string Serialize<T>(T value);
        T Deserialize<T>(string json);
    }

    public class DefaultJsonProvider : IJsonProvider
    {
        public string Serialize<T>(T value)
        {
            var serializer = new DataContractJsonSerializer(typeof(T));
            var stream = new MemoryStream();
            serializer.WriteObject(stream, value);
            var bytes = new byte[stream.Length];
            stream.Position = 0;
            stream.Read(bytes, 0, (int)stream.Length);
            return Encoding.UTF8.GetString(bytes);
        }

        public T Deserialize<T>(string json)
        {
            var serializer = new DataContractJsonSerializer(typeof(T));
            var bytes = Encoding.Default.GetBytes(json);
            var stream = new MemoryStream(bytes);
            return (T)serializer.ReadObject(stream);
        }
    }
}
