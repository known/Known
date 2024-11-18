namespace Known.Pages;

/// <summary>
/// 用户信息表单组件类。
/// </summary>
[StreamRendering]
[Route("/profile/user")]
public class UserEditForm : BaseEditForm<UserInfo>
{
    private IAuthService Service;

    /// <summary>
    /// 异步初始化表单。
    /// </summary>
    /// <returns></returns>
    protected override async Task OnInitFormAsync()
    {
        await base.OnInitFormAsync();
        Service = await CreateServiceAsync<IAuthService>();
        Model = new FormModel<UserInfo>(this) { Data = CurrentUser };
        Model.AddRow().AddColumn(c => c.UserName, c => c.ReadOnly = true);
        Model.AddRow().AddColumn(c => c.Name);
        Model.AddRow().AddColumn(c => c.EnglishName);
        Model.AddRow().AddColumn(c => c.Gender, c =>
        {
            c.Category = nameof(GenderType);
            c.Type = FieldType.RadioList;
        });
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
                builder.Form(Model);
                builder.FormButton(() => BuildAction(builder));
            });
        });
    }

    /// <summary>
    /// 异步保存用户表单信息。
    /// </summary>
    /// <param name="model">用户信息。</param>
    /// <returns>保存结果。</returns>
    protected override Task<Result> OnSaveAsync(UserInfo model)
    {
        return Service.UpdateUserAsync(model);
    }

    /// <summary>
    /// 用户保存成功后，设置当前用户信息，刷新页面。
    /// </summary>
    /// <param name="result">报错结果对象。</param>
    protected override void OnSuccess(Result result)
    {
        App?.SetCurrentUserAsync(CurrentUser);
        Navigation.Refresh(true);
    }
}