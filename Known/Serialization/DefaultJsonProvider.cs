namespace Known.Serialization
{
    public class DefaultJsonProvider : IJsonProvider
    {
        private IJsonProvider provider = new DataContractProvider();

        public string Serialize<T>(T value)
        {
            return provider.Serialize(value);
        }

        public T Deserialize<T>(string json)
        {
            return provider.Deserialize<T>(json);
        }
    }
}
