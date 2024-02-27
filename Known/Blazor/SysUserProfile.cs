using Known.Entities;
using Known.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace Known.Blazor;

class SysUserProfile : BasePage<SysUser>
{
    private SysUserProfileInfo info;
    private SysUserProfileTabs tabs;
    internal SysUser User { get; private set; }

    public override void StateChanged() => tabs?.StateChanged();

    protected override async Task OnInitPageAsync()
    {
        await base.OnInitPageAsync();

        User = await Platform.User.GetUserAsync(CurrentUser.Id);

        Page.Type = PageType.Column;
        Page.Spans = "28";
        Page.AddItem("kui-card", BuildUserInfo);
        Page.AddItem("kui-card", BuildUserTabs);
    }

    protected override void BuildPage(RenderTreeBuilder builder) => builder.Cascading(this, base.BuildPage);

    private void BuildUserInfo(RenderTreeBuilder builder) => builder.Div("p10", () => builder.Component<SysUserProfileInfo>().Build(value => info = value));
    private void BuildUserTabs(RenderTreeBuilder builder) => builder.Component<SysUserProfileTabs>().Build(value => tabs = value);

    internal void UpdateProfileInfo() => info?.StateChanged();
}

class SysUserProfileInfo : BaseComponent
{
    [CascadingParameter] private SysUserProfile Parent { get; set; }

    protected override void BuildRender(RenderTreeBuilder builder)
    {
        var User = Parent.User;
        builder.Div("kui-user-avatar", () => builder.Markup($"<img src=\"{CurrentUser?.AvatarUrl}\" />"));
        builder.Ul("kui-user-info", () =>
        {
            BuildUserInfoItem(builder, "user", $"{User?.Name}({User?.UserName})");
            BuildUserInfoItem(builder, "phone", User?.Phone);
            BuildUserInfoItem(builder, "mobile", User?.Mobile);
            BuildUserInfoItem(builder, "inbox", User?.Email);
            BuildUserInfoItem(builder, "team", User?.Role);
            BuildUserInfoItem(builder, "comment", User?.Note);
        });
    }

    private void BuildUserInfoItem(RenderTreeBuilder builder, string icon, string text)
    {
        builder.Li(() =>
        {
            UI.Icon(builder, icon);
            builder.Span(text);
        });
    }
}

class SysUserProfileTabs : BaseTabPage
{
    private SysUserProfileTabsInfo info;
    private SysUserProfileTabsSafe safe;

    protected override async Task OnInitPageAsync()
    {
        await base.OnInitPageAsync();
        //Tab.AddTab("TodoList", b => b.Component<SysSystemInfo>().Build());
        //Tab.AddTab("MyMessage", b => b.Component<SysSystemSafe>().Build());
        Tab.AddTab("MyProfile", b => b.Component<SysUserProfileTabsInfo>().Build(value => info = value));
        Tab.AddTab("SecuritySetting", b => b.Component<SysUserProfileTabsSafe>().Build(value => safe = value));
    }

    public override void StateChanged()
    {
        info?.StateChanged();
        safe?.StateChanged();
        base.StateChanged();
    }
}

class SysUserProfileTabsInfo : BaseForm<SysUser>
{
    private bool isEdit = false;
    [CascadingParameter] private SysUserProfile Parent { get; set; }

    protected override async Task OnInitFormAsync()
    {
        Model = new FormModel<SysUser>(Context, false)
        {
            LabelSpan = 4,
            WrapperSpan = 8,
            IsView = true,
            Data = Parent.User
        };
        Model.AddRow().AddColumn(c => c.UserName, c => c.ReadOnly = true);
        Model.AddRow().AddColumn(c => c.Name);
        Model.AddRow().AddColumn(c => c.EnglishName);
        Model.AddRow().AddColumn(c => c.Gender, c => c.Type = FieldType.RadioList);
        Model.AddRow().AddColumn(c => c.Phone);
        Model.AddRow().AddColumn(c => c.Mobile);
        Model.AddRow().AddColumn(c => c.Email);
        Model.AddRow().AddColumn(c => c.Note, c => c.Type = FieldType.TextArea);

        await base.OnInitFormAsync();
    }

    protected override void BuildForm(RenderTreeBuilder builder)
    {
        builder.FormPage(() =>
        {
            Model.IsView = !isEdit;
            base.BuildForm(builder);
            builder.FormPageButton(() =>
            {
                if (!isEdit)
                {
                    UI.Button(builder, Language.Edit, this.Callback<MouseEventArgs>(e => OnEdit(true)), "primary");
                }
                else
                {
                    UI.Button(builder, Language.Save, this.Callback<MouseEventArgs>(OnSaveAsync), "primary");
                    UI.Button(builder, Language.Cancel, this.Callback<MouseEventArgs>(e => OnEdit(false)), "default");
                }
            });
        });
    }

    private async void OnSaveAsync(MouseEventArgs arg)
    {
        if (!Model.Validate())
            return;

        var result = await Platform.Auth.UpdateUserAsync(Model.Data);
        UI.Result(result, () =>
        {
            Parent.UpdateProfileInfo();
            OnEdit(false);
        });
    }

    private void OnEdit(bool edit) => isEdit = edit;
}

class SysUserProfileTabsSafe : BaseForm<PwdFormInfo>
{
    protected override async Task OnInitFormAsync()
    {
        Model = new FormModel<PwdFormInfo>(Context)
        {
            LabelSpan = 4,
            WrapperSpan = 6,
            Data = new PwdFormInfo()
        };

        await base.OnInitFormAsync();
    }

    protected override void BuildForm(RenderTreeBuilder builder)
    {
        builder.FormPage(() =>
        {
            base.BuildForm(builder);
            builder.FormPageButton(() =>
            {
                UI.Button(builder, Language["Button.ConfirmUpdate"], this.Callback<MouseEventArgs>(OnSaveAsync), "primary");
            });
        });
    }

    private async void OnSaveAsync(MouseEventArgs arg)
    {
        if (!Model.Validate())
            return;

        var result = await Platform.Auth.UpdatePasswordAsync(Model.Data);
        UI.Result(result);
    }
}