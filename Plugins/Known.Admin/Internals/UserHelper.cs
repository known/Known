namespace Known.Helpers;

class UserHelper
{
    internal static Task<Result> OnDeletingAsync(Database db, List<UserInfo> infos)
    {
        if (AdminConfig.UserHandler == null)
            return Result.SuccessAsync("");

        return AdminConfig.UserHandler.OnDeletingAsync(db, infos);
    }

    internal static async Task OnDeletedAsync(Database db, UserInfo info)
    {
        if (AdminConfig.UserHandler != null)
            await AdminConfig.UserHandler.OnDeletedAsync(db, info);
    }

    internal static Task<Result> OnChangingDepartmentAsync(Database db, List<UserInfo> infos)
    {
        if (AdminConfig.UserHandler == null)
            return Result.SuccessAsync("");

        return AdminConfig.UserHandler.OnChangingDepartmentAsync(db, infos);
    }

    internal static async Task OnChangedDepartmentAsync(Database db, SysUser info)
    {
        if (AdminConfig.UserHandler != null)
            await AdminConfig.UserHandler.OnChangedDepartmentAsync(db, info);
    }

    internal static Task<Result> OnEnablingAsync(Database db, List<UserInfo> infos)
    {
        if (AdminConfig.UserHandler == null)
            return Result.SuccessAsync("");

        return AdminConfig.UserHandler.OnEnablingAsync(db, infos);
    }

    internal static async Task OnEnabledAsync(Database db, SysUser info)
    {
        if (AdminConfig.UserHandler != null)
            await AdminConfig.UserHandler.OnEnabledAsync(db, info);
    }

    internal static Task<Result> OnDisablingAsync(Database db, List<UserInfo> infos)
    {
        if (AdminConfig.UserHandler == null)
            return Result.SuccessAsync("");

        return AdminConfig.UserHandler.OnDisablingAsync(db, infos);
    }

    internal static async Task OnDisabledAsync(Database db, SysUser info)
    {
        if (AdminConfig.UserHandler != null)
            await AdminConfig.UserHandler.OnDisabledAsync(db, info);
    }

    internal static Task<Result> OnSavingAsync(Database db, UserInfo info)
    {
        if (AdminConfig.UserHandler == null)
            return Result.SuccessAsync("");

        return AdminConfig.UserHandler.OnSavingAsync(db, info);
    }

    internal static async Task OnSavedAsync(Database db, SysUser info)
    {
        if (AdminConfig.UserHandler != null)
            await AdminConfig.UserHandler.OnSavedAsync(db, info);
    }
}