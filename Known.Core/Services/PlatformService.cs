namespace Known.Services;

[WebApi, Service]
partial class PlatformService(Context context) : ServiceBase(context), IPlatformService
{
    public Task SetRenderModeAsync(string mode)
    {
        Config.RenderMode = Utils.ConvertTo<RenderType>(mode);
        return Task.CompletedTask;
    }
}