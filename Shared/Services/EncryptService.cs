namespace Known.Services;

/// <summary>
/// 数据加密服务接口。
/// </summary>
public interface IEncryptService
{
    /// <summary>
    /// 取得或设置加密密码。
    /// </summary>
    string Password { get; set; }

    /// <summary>
    /// 加密字符串。
    /// </summary>
    /// <param name="plainText">原文字符串。</param>
    /// <returns>加密字符串。</returns>
    string Encrypt(string plainText);

    /// <summary>
    /// 解密字符串。
    /// </summary>
    /// <param name="cipherText">加密字符串。</param>
    /// <returns>原文字符串。</returns>
    string Decrypt(string cipherText);
}

[Service(ServiceLifetime.Singleton)]
class EncryptService : IEncryptService
{
    public string Password { get; set; }

    public string Encrypt(string plainText)
    {
        if (string.IsNullOrWhiteSpace(plainText))
            return string.Empty;

        var buffer = Encoding.UTF8.GetBytes(plainText);
        var pwd = GetPassword();
        if (pwd <= 0)
            return Convert.ToBase64String(buffer);

        var bytes = buffer.Select(b => (byte)(b + pwd)).ToArray();
        return Convert.ToBase64String(bytes);
    }

    public string Decrypt(string cipherText)
    {
        if (string.IsNullOrWhiteSpace(cipherText))
            return string.Empty;

        var buffer = Convert.FromBase64String(cipherText);
        var pwd = GetPassword();
        if (pwd <= 0)
            return Encoding.UTF8.GetString(buffer);

        var bytes = buffer.Select(b => (byte)(b - pwd)).ToArray();
        return Encoding.UTF8.GetString(bytes);
    }

    private int GetPassword()
    {
        var pwd = Password ?? Config.App.WebApi.DataPassword;
        if (string.IsNullOrWhiteSpace(pwd))
            return 0;

        var bytes = Encoding.UTF8.GetBytes(pwd);
        return bytes.Sum(b => b);
    }
}