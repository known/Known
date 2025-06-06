﻿namespace Known.Extensions;

static class AdminExtension
{
    internal static Task<Result> AddPageLogAsync(this IAdminService service, UIContext context)
    {
        if (!Config.IsAdminLog && context.CurrentUser.IsSystemAdmin())
            return Result.SuccessAsync("");

        return service?.AddLogAsync(new LogInfo
        {
            Type = nameof(LogType.Page),
            Target = context?.Current?.Name,
            Content = context?.Url
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