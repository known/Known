namespace Known.Razor;

partial class UIService
{
    public void InitForm() => InvokeVoidAsync("KRazor.initForm");
    public void Captcha(string id, string code) => InvokeVoidAsync("KRazor.captcha", id, code);
}