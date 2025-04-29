using System.Diagnostics.CodeAnalysis;

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
            services.AddScoped<IJSRuntime, PrintJSRuntime>();
            services.AddScoped<JSService>();
            //services.AddHttpContextAccessor();
            var provider = services.BuildServiceProvider();
            var component = new ComponentRenderer<T>().AddServiceProvider(provider);
            action?.Invoke(component);
            var content = component.Render();
            await PrintAsync(content);
        }
        catch (Exception ex)
        {
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
        return InvokeVoidAsync("KBlazor.printContent", content);
    }
}

class PrintJSRuntime : IJSRuntime
{
    public ValueTask<TValue> InvokeAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.PublicProperties)] TValue>(string identifier, object[] args)
    {
        throw new NotImplementedException();
    }

    public ValueTask<TValue> InvokeAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.PublicProperties)] TValue>(string identifier, CancellationToken cancellationToken, object[] args)
    {
        throw new NotImplementedException();
    }
}