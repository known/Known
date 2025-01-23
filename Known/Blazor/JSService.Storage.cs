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
            try
            {
                var data = Utils.FromJson<T>(value);
                await SetLocalStorageAsync(key, data);
                return data;
            }
            catch
            {
                return default;
            }
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
        await InvokeVoidAsync("KBlazor.setLocalStorage", key, value);
    }

    /// <summary>
    /// 异步设置样式文件。
    /// </summary>
    /// <param name="match">原匹配样式文件。</param>
    /// <param name="href">新样式文件。</param>
    /// <returns></returns>
    public Task SetStyleSheetAsync(string match, string href)
    {
        return InvokeVoidAsync("KBlazor.setStyleSheet", match, href);
    }

    /// <summary>
    /// 异步插入样式文件。
    /// </summary>
    /// <param name="match">原匹配样式文件。</param>
    /// <param name="href">新样式文件。</param>
    /// <returns></returns>
    public Task InsertStyleSheetAsync(string match, string href)
    {
        return InvokeVoidAsync("KBlazor.insertStyleSheet", match, href);
    }

    /// <summary>
    /// 异步添加样式文件。
    /// </summary>
    /// <param name="href">样式文件。</param>
    /// <returns></returns>
    public Task AddStyleSheetAsync(string href)
    {
        return InvokeVoidAsync("KBlazor.addStyleSheet", href);
    }

    /// <summary>
    /// 异步删除样式文件。
    /// </summary>
    /// <param name="href">样式文件。</param>
    /// <returns></returns>
    public Task RemoveStyleSheetAsync(string href)
    {
        return InvokeVoidAsync("KBlazor.removeStyleSheet", href);
    }

    internal Task SetThemeAsync(string theme)
    {
        return InvokeVoidAsync("KBlazor.setTheme", theme);
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
        await InvokeVoidAsync("KBlazor.setSessionStorage", key, value);
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