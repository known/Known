using System.Web.Script.Serialization;

namespace Known.Serialization
{
    public class JavaScriptProvider : IJsonProvider
    {
        private JavaScriptSerializer serializer = new JavaScriptSerializer();

        public string Serialize<T>(T value)
        {
            return serializer.Serialize(value);
        }

        public T Deserialize<T>(string json)
        {
            return serializer.Deserialize<T>(json);
        }
    }
}
