namespace Known.Blazor;

public partial class JSService
{
    /// <summary>
    /// 异步调用浏览器打印组件内容。
    /// </summary>
    /// <typeparam name="T">组件类型。</typeparam>
    /// <param name="action">组件内容操作方法。</param>
    /// <returns></returns>
    public async Task PrintAsync<T>(Action<IPrintRenderer<T>> action) where T : Microsoft.AspNetCore.Components.IComponent
    {
        try
        {
            var services = new ServiceCollection();
            //services.AddScoped<JSService>();
            var provider = services.BuildServiceProvider();
            var component = new ComponentRenderer<T>().AddServiceProvider(provider);
            action?.Invoke(component);
            var content = component.Render();
            await PrintAsync(content);
        }
        catch (Exception ex)
        {
            ui.Error(ex.Message);
            Logger.Exception(LogTarget.FrontEnd, new UserInfo { Name = "Print" }, ex);
        }
    }

    /// <summary>
    /// 异步调用浏览器打印HTML内容。
    /// </summary>
    /// <param name="content">HTML内容。</param>
    /// <returns></returns>
    public Task PrintAsync(string content)
    {
        return InvokeAsync("KBlazor.printContent", content);
    }
}