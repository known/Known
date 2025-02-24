namespace Known.Helpers;

class UserHelper
{
    internal static Task<Result> OnDeletingAsync(Database db, List<UserInfo> infos)
    {
        if (CoreConfig.UserHandler == null)
            return Result.SuccessAsync("");

        return CoreConfig.UserHandler.OnDeletingAsync(db, infos);
    }

    internal static async Task OnDeletedAsync(Database db, UserInfo info)
    {
        if (CoreConfig.UserHandler != null)
            await CoreConfig.UserHandler.OnDeletedAsync(db, info);
    }

    internal static Task<Result> OnChangingDepartmentAsync(Database db, List<UserInfo> infos)
    {
        if (CoreConfig.UserHandler == null)
            return Result.SuccessAsync("");

        return CoreConfig.UserHandler.OnChangingDepartmentAsync(db, infos);
    }

    internal static async Task OnChangedDepartmentAsync(Database db, SysUser info)
    {
        if (CoreConfig.UserHandler != null)
            await CoreConfig.UserHandler.OnChangedDepartmentAsync(db, info);
    }

    internal static Task<Result> OnEnablingAsync(Database db, List<UserInfo> infos)
    {
        if (CoreConfig.UserHandler == null)
            return Result.SuccessAsync("");

        return CoreConfig.UserHandler.OnEnablingAsync(db, infos);
    }

    internal static async Task OnEnabledAsync(Database db, SysUser info)
    {
        if (CoreConfig.UserHandler != null)
            await CoreConfig.UserHandler.OnEnabledAsync(db, info);
    }

    internal static Task<Result> OnDisablingAsync(Database db, List<UserInfo> infos)
    {
        if (CoreConfig.UserHandler == null)
            return Result.SuccessAsync("");

        return CoreConfig.UserHandler.OnDisablingAsync(db, infos);
    }

    internal static async Task OnDisabledAsync(Database db, SysUser info)
    {
        if (CoreConfig.UserHandler != null)
            await CoreConfig.UserHandler.OnDisabledAsync(db, info);
    }

    internal static Task<Result> OnSavingAsync(Database db, UserInfo info)
    {
        if (CoreConfig.UserHandler == null)
            return Result.SuccessAsync("");

        return CoreConfig.UserHandler.OnSavingAsync(db, info);
    }

    internal static async Task OnSavedAsync(Database db, SysUser info)
    {
        if (CoreConfig.UserHandler != null)
            await CoreConfig.UserHandler.OnSavedAsync(db, info);
    }
}