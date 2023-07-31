namespace Known.Razor;

partial class UIService
{
    public void InitForm() => InvokeVoidAsync("KRazor.initForm");
    internal Task<IJSObjectReference> InitEditor(string id, object option) => InvokeAsync<IJSObjectReference>("KRazor.initEditor", id, option);
    internal void Captcha(string id, string code) => InvokeVoidAsync("KRazor.captcha", id, code);
}