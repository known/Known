namespace Known.Services;

public partial interface IAdminService
{
    /// <summary>
    /// 异步设置呈现模式。
    /// </summary>
    /// <param name="mode">呈现模式。</param>
    /// <returns></returns>
    Task<Result> SetRenderModeAsync(string mode);
}

partial class AdminClient
{
    public Task<Result> SetRenderModeAsync(string mode) => Http.PostAsync($"/Admin/SetRenderMode?mode={mode}");
}

partial class AdminService
{
    public Task<Result> SetRenderModeAsync(string mode)
    {
        Config.CurrentMode = Utils.ConvertTo<RenderType>(mode);
        return Result.SuccessAsync(Language.SetSuccess, Config.CurrentMode);
    }
}