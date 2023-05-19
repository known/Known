namespace Known.Clients;

public class RoleClient : ClientBase
{
    public RoleClient(Context context) : base(context) { }

    public Task<PagingResult<SysRole>> QueryRolesAsync(PagingCriteria criteria) => Context.QueryAsync<SysRole>("Role/QueryRoles", criteria);
    public Task<Result> DeleteRolesAsync(List<SysRole> models) => Context.PostAsync("Role/DeleteRoles", models);
    public Task<RoleFormInfo> GetRoleAsync(string roleId) => Context.GetAsync<RoleFormInfo>($"Role/GetRole?roleId={roleId}");
    public Task<Result> SaveRoleAsync(RoleFormInfo info) => Context.PostAsync("Role/SaveRole", info);
}