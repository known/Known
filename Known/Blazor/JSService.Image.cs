namespace Known.Blazor;

public partial class JSService
{
    /// <summary>
    /// 异步绘制验证码组件。
    /// </summary>
    /// <param name="id">验证码控件ID。</param>
    /// <param name="code">验证码字符串。</param>
    /// <returns></returns>
    public Task CaptchaAsync(string id, string code)
    {
        return InvokeAsync("KBlazor.captcha", id, code);
    }
}