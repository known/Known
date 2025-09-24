namespace Known.Pages;

/// <summary>
/// 用户管理页面组件类。
/// </summary>
/// <param name="page">页面扩展。</param>
[Route("/sys/users")]
[Menu(Constants.System, "用户管理", "user", 3)]
//[PagePlugin("用户管理", "user", PagePluginType.Module, AdminLanguage.SystemManage, Sort = 6)]
public class SysUserList(IUserPage page) : BaseTablePage<UserDataInfo>
{
    private IUserService Service;

    /// <summary>
    /// 取得或设置当前部门。
    /// </summary>
    public string CurrentOrg { get; set; }

    /// <inheritdoc />
    protected override async Task OnInitPageAsync()
    {
        await base.OnInitPageAsync();
        Service = await CreateServiceAsync<IUserService>();
        if (page != null)
            await page.OnInitAsync(this);

        Table = new TableModel<UserDataInfo>(this)
        {
            FormType = UIConfig.UserFormTabs.Count > 0 ? typeof(UserTabForm) : typeof(UserForm),
            Form = new FormInfo { Width = 800, SmallLabel = true, ShowFooter = UIConfig.UserFormShowFooter },
            RowKey = r => r.Id,
            OnQuery = OnQueryUsersAsync
        };
        Table.AdvSearch = UIConfig.IsAdvAdmin;
        Table.EnableFilter = UIConfig.IsAdvAdmin;
        Table.Column(c => c.Gender).Tag();
    }

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
        {
            if (page != null)
                await page.OnAfterRenderAsync();
        }
    }

    /// <inheritdoc />
    protected override void BuildPage(RenderTreeBuilder builder)
    {
        var isBuild = page?.BuildPage(builder);
        if (isBuild == true)
            return;

        base.BuildPage(builder);
    }

    private Task<PagingResult<UserDataInfo>> OnQueryUsersAsync(PagingCriteria criteria)
    {
        criteria.Parameters[nameof(UserInfo.OrgNo)] = CurrentOrg;
        return Service.QueryUserDatasAsync(criteria);
    }

    /// <summary>
    /// 新增用户。
    /// </summary>
    [Action] public void New() => Table.NewForm(Service.SaveUserAsync, new UserDataInfo { OrgNo = CurrentOrg });

    /// <summary>
    /// 编辑用户。
    /// </summary>
    /// <param name="row">用户信息。</param>
    [Action] public void Edit(UserDataInfo row) => Table.EditForm(Service.SaveUserAsync, row);

    /// <summary>
    /// 删除用户。
    /// </summary>
    /// <param name="row">用户信息。</param>
    [Action] public void Delete(UserDataInfo row) => Table.Delete(Service.DeleteUsersAsync, row);

    /// <summary>
    /// 批量删除用户。
    /// </summary>
    [Action] public void DeleteM() => Table.DeleteM(Service.DeleteUsersAsync);

    /// <summary>
    /// 重置默认密码。
    /// </summary>
    [Action] public void ResetPassword() => Table.SelectRows(Service.SetUserPwdsAsync, Language.Reset);

    /// <summary>
    /// 改变用户部门。
    /// </summary>
    [Action]
    public void ChangeDepartment() => Table.SelectRows(rows => page?.OnChangeDepartment(Service.ChangeDepartmentAsync, rows));

    /// <summary>
    /// 启用用户。
    /// </summary>
    [Action] public void Enable() => Table.SelectRows(Service.EnableUsersAsync, Language.Enable);

    /// <summary>
    /// 禁用用户。
    /// </summary>
    [Action] public void Disable() => Table.SelectRows(Service.DisableUsersAsync, Language.Disable);

    /// <summary>
    /// 导入用户。
    /// </summary>
    /// <returns></returns>
    [Action] public Task Import() => Table.ShowImportAsync();

    /// <summary>
    /// 导出用户。
    /// </summary>
    /// <returns></returns>
    [Action] public Task Export() => Table.ExportDataAsync();
}

/// <summary>
/// 用户管理页面扩展接口。
/// </summary>
public interface IUserPage
{
    /// <summary>
    /// 异步初始化。
    /// </summary>
    /// <param name="list">用户管理页面。</param>
    /// <returns></returns>
    Task OnInitAsync(SysUserList list);

    /// <summary>
    /// 异步呈现后。
    /// </summary>
    /// <returns></returns>
    Task OnAfterRenderAsync();

    /// <summary>
    /// 构建页面。
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    bool BuildPage(RenderTreeBuilder builder);

    /// <summary>
    /// 更换用户部门。
    /// </summary>
    /// <param name="onChange">更换委托。</param>
    /// <param name="rows">用户列表。</param>
    void OnChangeDepartment(Func<List<UserDataInfo>, Task<Result>> onChange, List<UserDataInfo> rows);
}

class UserTabForm : BaseTabForm
{
    [Parameter] public FormModel<UserDataInfo> Model { get; set; }

    protected override async Task OnInitFormAsync()
    {
        await base.OnInitFormAsync();

        Tab.AddTab(Language.BasicInfo, b => b.Component<UserForm>().Set(c => c.Model, Model).Build());
        foreach (var item in UIConfig.UserFormTabs.OrderBy(t => t.Value.Id))
        {
            if (item.Value.Parameters == null)
                item.Value.Parameters = [];
            item.Value.Parameters[nameof(UserFormTab.IsView)] = Model.IsView;
            item.Value.Parameters[nameof(UserFormTab.User)] = Model.Data;
            Tab.AddTab(item.Key, b => b.DynamicComponent(item.Value));
        }
    }
}

class UserForm : BaseForm<UserDataInfo>
{
    private IUserService Service;
    private string defaultPassword;

    protected override async Task OnInitFormAsync()
    {
        await base.OnInitFormAsync();
        Service = await CreateServiceAsync<IUserService>();

        if (!UIConfig.UserFormShowFooter)
        {
            SaveClose = UIConfig.UserFormTabs.Count == 0;
            ShowAction = UIConfig.UserFormTabs.Count > 0;
        }
        //Model.Header = b => b.Alert();
        Model.Field(f => f.UserName).ReadOnly(!Model.Data.IsNew);
        Model.AddRow().AddColumn(c => c.RoleIds, c => c.Type = FieldType.CheckList);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
        {
            var user = await Service.GetUserDataAsync(Model.Data.Id);
            defaultPassword = user.DefaultPassword;
            if (Model.IsNew)
                Model.Data.Password = defaultPassword;
            var pwdTips = Language[Language.TipUserDefaultPwd].Replace("{password}", defaultPassword);
            Model.Field(f => f.Password).Tooltip(pwdTips);
            Model.Data.RoleIds = user.RoleIds;
            Model.Codes["Roles"] = user.Roles;
        }
    }
}