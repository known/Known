namespace Known.Razor;

partial class UIService
{
    public void InitForm() => InvokeVoidAsync("KRazor.initForm");
    internal void InitEditor(string id) => InvokeVoidAsync("KRazor.initEditor", id);
    internal void Captcha(string id, string code) => InvokeVoidAsync("KRazor.captcha", id, code);
}