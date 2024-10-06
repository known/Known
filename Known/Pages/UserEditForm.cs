namespace Known.Pages;

/// <summary>
/// 用户信息表单组件类。
/// </summary>
[StreamRendering]
[Route("/profile/user")]
public class UserEditForm : BaseEditForm<SysUser>
{
    private IUserService Service;
    [CascadingParameter] private SysUserProfile Parent { get; set; }

    /// <summary>
    /// 异步初始化表单。
    /// </summary>
    /// <returns></returns>
    protected override async Task OnInitFormAsync()
    {
        await base.OnInitFormAsync();
        Service = await CreateServiceAsync<IUserService>();
        var data = Parent?.User;
        data ??= await Service.GetUserAsync(CurrentUser.Id);
        Model = new FormModel<SysUser>(this) { Data = data };
        Model.AddRow().AddColumn(c => c.UserName, c => c.ReadOnly = true);
        Model.AddRow().AddColumn(c => c.Name);
        Model.AddRow().AddColumn(c => c.EnglishName);
        Model.AddRow().AddColumn(c => c.Gender, c => c.Type = FieldType.RadioList);
        Model.AddRow().AddColumn(c => c.Phone);
        Model.AddRow().AddColumn(c => c.Mobile);
        Model.AddRow().AddColumn(c => c.Email);
        Model.AddRow().AddColumn(c => c.Note, c => c.Type = FieldType.TextArea);
    }

    /// <summary>
    /// 构建表单内容。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    protected override void BuildFormContent(RenderTreeBuilder builder)
    {
        builder.FormPage(() =>
        {
            builder.Div("kui-user-form", () =>
            {
                UI.BuildForm(builder, Model);
                builder.FormButton(() => BuildAction(builder));
            });
        });
    }

    /// <summary>
    /// 异步保存用户表单信息。
    /// </summary>
    /// <param name="model">用户信息。</param>
    /// <returns>保存结果。</returns>
    protected override Task<Result> OnSaveAsync(SysUser model)
    {
        return Service.UpdateUserAsync(model);
    }

    /// <summary>
    /// 用户保存成功后，设置当前用户信息，刷新页面。
    /// </summary>
    /// <param name="result">报错结果对象。</param>
    protected override void OnSuccess(Result result)
    {
        var entity = result.DataAs<SysUser>();
        if (entity != null)
        {
            var user = CurrentUser;
            user.Name = entity.Name;
            user.EnglishName = entity.EnglishName;
            user.Gender = entity.Gender;
            user.Phone = entity.Phone;
            user.Mobile = entity.Mobile;
            user.Email = entity.Email;
            user.Note = entity.Note;
            App?.SetCurrentUserAsync(user);
        }
        Navigation.Refresh(true);
    }
}