using Known.Blazor;

namespace Known.Extensions;

static class JSExtension
{
    private static readonly string KeyLanguage = "Known_Language";
    private static readonly string KeyLoginInfo = "Known_LoginInfo";

    internal static Task<string> GetCurrentLanguage(this JSService service) => service.GetLocalStorage<string>(KeyLanguage);
    internal static void SetCurrentLanguage(this JSService service, string language) => service.SetLocalStorage(KeyLanguage, language);

    internal static Task<T> GetLoginInfo<T>(this JSService service) => service.GetLocalStorage<T>(KeyLoginInfo);
    internal static void SetLoginInfo(this JSService service, object value) => service.SetLocalStorage(KeyLoginInfo, value);
}