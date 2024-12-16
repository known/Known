namespace Known.Extensions;

static class PlatformExtension
{
    internal static Task<Result> AddPageLogAsync(this IPlatformService service, UIContext context)
    {
        return service.AddLogAsync(new LogInfo
        {
            Type = LogType.Page,
            Target = context.Current.Name,
            Content = context.Url
        });
    }

    internal static Task<Result> SaveUserSettingAsync(this IPlatformService service, UserSettingInfo info)
    {
        return service.SaveUserSettingFormAsync(new SettingFormInfo
        {
            BizType = Constants.UserSetting,
            BizData = info
        });
    }
}