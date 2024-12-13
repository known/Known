namespace Known.Services;

/// <summary>
/// 身份认证服务接口。
/// </summary>
public interface IAuthService : IService
{
    /// <summary>
    /// 异步修改系统用户头像。
    /// </summary>
    /// <param name="info">用户头像信息。</param>
    /// <returns>修改结果。</returns>
    Task<Result> UpdateAvatarAsync(AvatarInfo info);

    /// <summary>
    /// 异步修改系统用户信息。
    /// </summary>
    /// <param name="info">系统用户信息。</param>
    /// <returns>修改结果。</returns>
    Task<Result> UpdateUserAsync(UserInfo info);

    /// <summary>
    /// 异步修改用户密码。
    /// </summary>
    /// <param name="info">修改密码对象。</param>
    /// <returns>修改结果。</returns>
    Task<Result> UpdatePasswordAsync(PwdFormInfo info);
}

class AuthService(Context context) : ServiceBase(context), IAuthService
{
    public async Task<Result> UpdateAvatarAsync(AvatarInfo info)
    {
        var database = Database;
        var entity = await database.QueryAsync<SysUser>(d => d.Id == info.UserId);
        if (entity == null)
            return Result.Error(Language["Tip.NoUser"]);

        var attach = new AttachFile(info.File, CurrentUser);
        attach.FilePath = @$"Avatars\{entity.Id}{attach.ExtName}";
        await attach.SaveAsync();

        var url = Config.GetFileUrl(attach.FilePath);
        entity.SetExtension(nameof(UserInfo.AvatarUrl), url);
        await database.SaveAsync(entity);
        return Result.Success(Language.Success(Language.Save), url);
    }

    public async Task<Result> UpdateUserAsync(UserInfo info)
    {
        if (info == null)
            return Result.Error(Language["Tip.NoUser"]);

        var result = await Database.SaveUserAsync(info);
        if (!result.IsValid)
            return result;

        return Result.Success(Language.Success(Language.Save), info);
    }

    public async Task<Result> UpdatePasswordAsync(PwdFormInfo info)
    {
        var user = CurrentUser;
        if (user == null)
            return Result.Error(Language["Tip.NoLogin"]);

        var errors = new List<string>();
        if (string.IsNullOrEmpty(info.OldPwd))
            errors.Add(Language["Tip.CurPwdRequired"]);
        if (string.IsNullOrEmpty(info.NewPwd))
            errors.Add(Language["Tip.NewPwdRequired"]);
        if (string.IsNullOrEmpty(info.NewPwd1))
            errors.Add(Language["Tip.ConPwdRequired"]);
        if (info.NewPwd != info.NewPwd1)
            errors.Add(Language["Tip.PwdNotEqual"]);

        if (errors.Count > 0)
            return Result.Error(string.Join(Environment.NewLine, errors));

        var database = Database;
        var entity = await database.QueryByIdAsync<SysUser>(user.Id);
        if (entity == null)
            return Result.Error(Language["Tip.NoUser"]);

        info.OldPwd = Utils.ToMd5(info.NewPwd);
        if (entity.Password != info.OldPwd)
            return Result.Error(Language["Tip.CurPwdInvalid"]);

        entity.Password = Utils.ToMd5(info.NewPwd);
        await database.SaveAsync(entity);
        return Result.Success(Language.Success(Language["Button.Update"]), entity.Id);
    }
}