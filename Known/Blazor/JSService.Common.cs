namespace Known.Blazor;

public partial class JSService
{
    internal Task InitFilesAsync()
    {
        var styles = KStyleSheet.Items;
        var scripts = KScript.Items;
        return InvokeVoidAsync("KBlazor.initStaticFile", styles, scripts);
    }

    internal Task SetUserSettingAsync(UserSettingInfo setting)
    {
        return InvokeVoidAsync("KBlazor.setUserSetting", setting);
    }

    /// <summary>
    /// 异步执行一段JS脚本，返回执行结果对象。
    /// </summary>
    /// <param name="script">JS脚本。</param>
    /// <returns>执行结果对象。</returns>
    public Task<object> RunAsync(string script)
    {
        return InvokeAsync<object>("KBlazor.runScript", script);
    }

    /// <summary>
    /// 异步执行一段JS脚本，无返回结果。
    /// </summary>
    /// <param name="script">JS脚本。</param>
    /// <returns></returns>
    public Task RunVoidAsync(string script)
    {
        return InvokeVoidAsync("KBlazor.runScriptVoid", script);
    }

    /// <summary>
    /// 异步单击前端指定ID的控件。
    /// </summary>
    /// <param name="clientId">前端控件ID。</param>
    /// <returns></returns>
    public Task ClickAsync(string clientId)
    {
        return InvokeVoidAsync("KBlazor.elemClick", clientId);
    }

    /// <summary>
    /// 异步将前端控件设为是否可用。
    /// </summary>
    /// <param name="clientId">前端控件ID。</param>
    /// <param name="enabled">是否可用。</param>
    /// <returns></returns>
    public Task EnabledAsync(string clientId, bool enabled)
    {
        return InvokeVoidAsync("KBlazor.elemEnabled", clientId, enabled);
    }

    /// <summary>
    /// 高亮显示代码。
    /// </summary>
    /// <param name="code">代码。</param>
    /// <param name="language">代码语言。</param>
    /// <returns>高亮代码。</returns>
    public Task<string> HighlightAsync(string code, string language)
    {
        return InvokeAsync<string>("KBlazor.highlight", code, language);
    }
}