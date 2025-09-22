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
    Task<Result> SetRenderModeAsync(string mode);
}

[Client]
partial class PlatformClient(HttpClient http) : ClientBase(http), IPlatformService
{
    public Task<Result> SetRenderModeAsync(string mode) => Http.PostAsync($"/Platform/SetRenderMode?mode={mode}");
}

[WebApi, Service]
partial class PlatformService(Context context) : ServiceBase(context), IPlatformService
{
    public Task<Result> SetRenderModeAsync(string mode)
    {
        Config.CurrentMode = Utils.ConvertTo<RenderType>(mode);
        return Result.SuccessAsync(Language.SetSuccess, Config.CurrentMode);
    }
}