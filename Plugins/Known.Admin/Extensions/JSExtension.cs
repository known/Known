namespace Known.Extensions;

static class JSExtension
{
    private static readonly string KeyLoginInfo = "Known_LoginInfo";

    internal static Task<T> GetLoginInfoAsync<T>(this JSService js)
    {
        return js.GetLocalStorageAsync<T>(KeyLoginInfo);
    }

    internal static Task SetLoginInfoAsync(this JSService js, object value)
    {
        return js.SetLocalStorageAsync(KeyLoginInfo, value);
    }
}