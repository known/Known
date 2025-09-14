namespace Known;

public class UserHandler
{
    public virtual Task<Result> OnDeletingAsync(Database db, List<UserInfo> infos) => Result.SuccessAsync("");
    public virtual Task OnDeletedAsync(Database db, UserInfo info) => Task.CompletedTask;
    public virtual Task<Result> OnChangingDepartmentAsync(Database db, List<UserInfo> infos) => Result.SuccessAsync("");
    public virtual Task OnChangedDepartmentAsync(Database db, SysUser info) => Task.CompletedTask;
    public virtual Task<Result> OnEnablingAsync(Database db, List<UserInfo> infos) => Result.SuccessAsync("");
    public virtual Task OnEnabledAsync(Database db, SysUser info) => Task.CompletedTask;
    public virtual Task<Result> OnDisablingAsync(Database db, List<UserInfo> infos) => Result.SuccessAsync("");
    public virtual Task OnDisabledAsync(Database db, SysUser info) => Task.CompletedTask;
    public virtual Task<Result> OnSavingAsync(Database db, UserInfo info) => Result.SuccessAsync("");
    public virtual Task OnSavedAsync(Database db, SysUser info) => Task.CompletedTask;
}