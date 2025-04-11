namespace Known.Services;

/// <summary>
/// 框架平台数据服务接口。
/// </summary>
public partial interface IPlatformService : IService
{
    /// <summary>
    /// 异步设置呈现模式。
    /// </summary>
    /// <param name="mode">呈现模式。</param>
    /// <returns></returns>
    Task SetRenderModeAsync(string mode);
}

[Service]
partial class PlatformService(Context context) : ServiceBase(context), IPlatformService
{
    public Task SetRenderModeAsync(string mode)
    {
        Config.RenderMode = Utils.ConvertTo<RenderType>(mode);
        return Task.CompletedTask;
    }
}

[Client]
partial class PlatformClient(HttpClient http) : ClientBase(http), IPlatformService
{
    public Task SetRenderModeAsync(string mode)
    {
        return Http.PostAsync($"/Platform/SetRenderMode?mode={mode}");
    }
}