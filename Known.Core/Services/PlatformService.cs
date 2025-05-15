namespace Known.Services;

[WebApi, Service]
partial class PlatformService(Context context) : ServiceBase(context), IPlatformService
{
    public Task<Result> SetRenderModeAsync(string mode)
    {
        Config.CurrentMode = Utils.ConvertTo<RenderType>(mode);
        return Result.SuccessAsync(Language.SetSuccess, Config.CurrentMode);
    }
}