namespace Known.Razor;

partial class UIService
{
    internal void InitAdminTab() => InvokeVoidAsync("KRazor.initAdminTab");
    internal void InitPage(string id) => InvokeVoidAsync("KRazor.initPage", id);
}