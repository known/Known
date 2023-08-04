namespace Known.Razor;

public enum StorageType { Server, AliOSS, TencentCOS }

public class StorageOption
{
    public StorageType Type { get; set; }
    public string KeyId { get; set; }
    public string KeySecret { get; set; }
    public string Region { get; set; }
    public string Bucket { get; set; }
}