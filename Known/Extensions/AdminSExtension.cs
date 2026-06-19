namespace Known.Extensions;

static class AdminSExtension
{
    internal static Task<Result> AddPageLogAsync(this IAdminService service, UIContext context)
    {
        if (context.CurrentUser == null)
            return Result.SuccessAsync("");

        if (!Config.IsAdminLog && context.CurrentUser.IsSystemAdmin())
            return Result.SuccessAsync("");

        return service?.AddLogAsync(new LogInfo
        {
            Type = nameof(LogType.Page),
            Target = context?.Current?.Name,
            Content = context?.Url
        });
    }

    internal static Task<Result> AddActionLogAsync(this IAdminService service, UIContext context, ActionInfo info)
    {
        if (context.CurrentUser == null)
            return Result.SuccessAsync("");

        if (!Config.IsAdminLog && context.CurrentUser.IsSystemAdmin())
            return Result.SuccessAsync("");

        return service?.AddLogAsync(new LogInfo
        {
            Type = nameof(LogType.Operate),
            Target = context?.Current?.Name,
            Content = $"{context?.Url}, Action: {info?.Id}-{info?.Name}"
        });
    }

    internal static Task<Result> SaveUserSettingAsync(this IAdminService service, UserSettingInfo info)
    {
        return service?.SaveUserSettingAsync(new SettingFormInfo
        {
            BizType = Constants.UserSetting,
            BizData = info
        });
    }
}