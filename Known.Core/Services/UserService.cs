namespace Known.Core.Services;

class UserService(Context context) : ServiceBase(context), IUserService
{
    //User
    public Task<PagingResult<SysUser>> QueryUsersAsync(PagingCriteria criteria)
    {
        return Database.QueryUsersAsync(criteria);
    }

    public Task<SysUser> GetUserAsync(string id) => Database.QueryByIdAsync<SysUser>(id);

    public async Task<Result> UpdateAvatarAsync(AvatarInfo info)
    {
        var database = Database;
        var entity = await database.QueryByIdAsync<SysUser>(info.UserId);
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

    public async Task<Result> UpdateUserAsync(SysUser model)
    {
        if (model == null)
            return Result.Error(Language["Tip.NoUser"]);

        var vr = model.Validate(Context);
        if (!vr.IsValid)
            return vr;

        await Database.SaveAsync(model);
        return Result.Success(Language.Success(Language.Save), model);
    }
}