using System.ServiceModel.Dispatcher;

namespace Known.Serialization
{
    public class JsonQueryProvider : IJsonProvider
    {
        private JsonQueryStringConverter converter = new JsonQueryStringConverter();

        public string Serialize<T>(T value)
        {
            return converter.ConvertValueToString(value, typeof(T));
        }

        public T Deserialize<T>(string json)
        {
            return (T)converter.ConvertStringToValue(json, typeof(T));
        }
    }
}
