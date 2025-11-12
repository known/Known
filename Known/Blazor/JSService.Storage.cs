namespace Known.Blazor;

public partial class JSService
{
    /// <summary>
    /// 异步获取浏览器加密存储的泛型对象。
    /// </summary>
    /// <typeparam name="T">泛型对象类型。</typeparam>
    /// <param name="key">对象存储键。</param>
    /// <returns>泛型对象。</returns>
    public async Task<T> GetLocalStorageAsync<T>(string key)
    {
        var value = await InvokeAsync<string>("KBlazor.getLocalStorage", key);
        if (string.IsNullOrWhiteSpace(value))
            return default;

        try
        {
            var json = DecryptString(value);
            return Utils.FromJson<T>(json);
        }
        catch
        {
            return default;
        }
    }

    /// <summary>
    /// 异步设置浏览器加密存储对象。
    /// </summary>
    /// <param name="key">对象存储键。</param>
    /// <param name="data">对象数据。</param>
    /// <returns></returns>
    public async Task SetLocalStorageAsync(string key, object data)
    {
        var value = EncryptString(data);
        await InvokeAsync("KBlazor.setLocalStorage", key, value);
    }

    /// <summary>
    /// 异步获取浏览器加密会话存储的泛型对象。
    /// </summary>
    /// <typeparam name="T">泛型对象类型。</typeparam>
    /// <param name="key">对象存储键。</param>
    /// <returns>泛型对象。</returns>
    public async Task<T> GetSessionStorageAsync<T>(string key)
    {
        var value = await InvokeAsync<string>("KBlazor.getSessionStorage", key);
        if (string.IsNullOrWhiteSpace(value))
            return default;

        var json = DecryptString(value);
        return Utils.FromJson<T>(json);
    }

    /// <summary>
    /// 异步设置浏览器加密会话存储对象。
    /// </summary>
    /// <param name="key">对象存储键。</param>
    /// <param name="data">对象数据。</param>
    /// <returns></returns>
    public async Task SetSessionStorageAsync(string key, object data)
    {
        var value = EncryptString(data);
        await InvokeAsync("KBlazor.setSessionStorage", key, value);
    }

    private static string EncryptString(object value)
    {
        if (value == null)
            return null;

        var json = Utils.ToJson(value);
        var bytes = Encoding.UTF8.GetBytes(json);
        return Convert.ToBase64String(bytes);
    }

    private static string DecryptString(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        var bytes = Convert.FromBase64String(value);
        return Encoding.UTF8.GetString(bytes);
    }
}