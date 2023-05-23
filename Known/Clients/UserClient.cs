namespace Known.Clients;

public class UserClient : ClientBase
{
    public UserClient(Context context) : base(context) { }

    //User
    public Task<PagingResult<SysUser>> QueryUsersAsync(PagingCriteria criteria) => Context.QueryAsync<SysUser>("User/QueryUsers", criteria);
    public Task<Result> DeleteUsersAsync(List<SysUser> models) => Context.PostAsync("User/DeleteUsers", models);
    public Task<Result> SetUserPwdsAsync(List<SysUser> models) => Context.PostAsync("User/SetUserPwds", models);
    public Task<Result> SaveUserAsync(object model) => Context.PostAsync("User/SaveUser", model);
    public Task<Result> UpdateUserAsync(object model) => Context.PostAsync("User/UpdateUser", model);
    public Task<UserAuthInfo> GetUserAuthAsync(string userId) => Context.GetAsync<UserAuthInfo>($"User/GetUserAuth?userId={userId}");
    
    //Account
    public Task<Result> SignInAsync(LoginFormInfo info) => Context.PostAsync("User/SignIn", info);
    public Task<Result> SignOutAsync(string token) => Context.PostAsync($"User/SignOut/{token}");
    public Task<AdminInfo> GetAdminAsync() => Context.GetAsync<AdminInfo>("User/GetAdmin");
    public Task<Result> UpdatePasswordAsync(PwdFormInfo info) => Context.PostAsync("User/UpdatePassword", info);
    public Task<Result> SaveSettingAsync(SettingFormInfo info) => Context.PostAsync("User/SaveSetting", info);

    //Message
    public Task<PagingResult<SysMessage>> QueryMessagesAsync(PagingCriteria criteria) => Context.QueryAsync<SysMessage>("User/QueryMessages", criteria);
    public Task<Result> DeleteMessagesAsync(List<SysMessage> models) => Context.PostAsync("User/DeleteMessages", models);
    public Task<Result> SaveMessageAsync(object model) => Context.PostAsync("User/SaveMessage", model);
}