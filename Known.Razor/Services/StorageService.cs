namespace Known.Razor;

partial class UIService
{
    public async Task<T> GetLocalStorage<T>(string key)
    {
        var value = await InvokeAsync<string>("KRazor.getLocalStorage", key);
        return Utils.FromJson<T>(value);
    }

    public void SetLocalStorage(string key, object value) => InvokeVoidAsync("KRazor.setLocalStorage", key, value);

    public async Task<T> GetSessionStorage<T>(string key)
    {
        var value = await InvokeAsync<string>("KRazor.getSessionStorage", key);
        return Utils.FromJson<T>(value);
    }

    public void SetSessionStorage(string key, object value) => InvokeVoidAsync("KRazor.setSessionStorage", key, value);
}