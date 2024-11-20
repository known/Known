namespace Known.Extensions;

static class PlatformExtension
{
    internal static Task<Result> SaveUserSettingAsync(this IPlatformService service, UserSettingInfo info)
    {
        return service.SaveUserSettingFormAsync(new SettingFormInfo
        {
            BizType = Constants.UserSetting,
            BizData = info
        });
    }
}