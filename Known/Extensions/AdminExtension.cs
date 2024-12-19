namespace Known.Extensions;

static class AdminExtension
{
    internal static Task<Result> AddPageLogAsync(this IAdminService service, UIContext context)
    {
        return service.AddLogAsync(new LogInfo
        {
            Type = LogType.Page,
            Target = context.Current.Name,
            Content = context.Url
        });
    }

    internal static Task<Result> SaveUserSettingAsync(this IAdminService service, UserSettingInfo info)
    {
        return service.SaveUserSettingFormAsync(new SettingFormInfo
        {
            BizType = Constants.UserSetting,
            BizData = info
        });
    }
}