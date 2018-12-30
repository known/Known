namespace Known.Serialization
{
    public interface IJsonProvider
    {
        string Serialize<T>(T value);
        T Deserialize<T>(string json);
    }
}
