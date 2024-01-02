using Known.Entities;
using Known.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace Known.Blazor;

class SysUserProfile : BasePage<SysUser>
{
    private SysUserProfileInfo info;
    internal SysUser User { get; private set; }

    protected override async Task OnInitPageAsync()
    {
        await base.OnInitPageAsync();

        User = await Platform.User.GetUserAsync(CurrentUser.Id);

        Page.Type = PageType.Column;
        Page.Spans = [6, 18];
        Page.Contents = [BuildUserInfo, BuildUserTabs];
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder) => builder.Cascading(this, base.BuildRenderTree);

    private void BuildUserInfo(RenderTreeBuilder builder) => builder.Div("p10", () => builder.Component<SysUserProfileInfo>().Build(value => info = value));
    private void BuildUserTabs(RenderTreeBuilder builder) => builder.Component<SysUserProfileTabs>().Build();

    internal void UpdateProfileInfo() => info?.StateChanged();
}

class SysUserProfileInfo : BaseComponent
{
    [CascadingParameter] private SysUserProfile Parent { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
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
            UI.BuildIcon(builder, icon);
            builder.Span(text);
        });
    }
}

class SysUserProfileTabs : BaseTabPage
{
    protected override async Task OnInitPageAsync()
    {
        await base.OnInitPageAsync();
        //Tab.Items.Add(new ItemModel(Context.Language["Title.TodoList"]) { Content = builder => builder.Component<SysSystemInfo>().Build() });
        //Tab.Items.Add(new ItemModel(Context.Language["Title.MyMessage"]) { Content = builder => builder.Component<SysSystemSafe>().Build() });
        Tab.Items.Add(new ItemModel(Context.Language["Title.MyProfile"]) { Content = builder => builder.Component<SysUserProfileTabsInfo>().Build() });
        Tab.Items.Add(new ItemModel(Context.Language["Title.SecuritySetting"]) { Content = builder => builder.Component<SysUserProfileTabsSafe>().Build() });
    }
}

class SysUserProfileTabsInfo : BaseForm<SysUser>
{
    private bool isEdit = false;
    [CascadingParameter] private SysUserProfile Parent { get; set; }

    protected override async Task OnInitFormAsync()
    {
        Model = new FormModel<SysUser>(UI, false)
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

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.FormPage(() =>
        {
            Model.IsView = !isEdit;
            base.BuildRenderTree(builder);
            builder.FormPageButton(() =>
            {
                if (!isEdit)
                {
                    UI.Button(builder, Context.Language.Edit, this.Callback<MouseEventArgs>(e => OnEdit(true)), "primary");
                }
                else
                {
                    UI.Button(builder, Context.Language.Save, this.Callback<MouseEventArgs>(OnSaveAsync), "primary");
                    UI.Button(builder, Context.Language.Cancel, this.Callback<MouseEventArgs>(e => OnEdit(false)), "default");
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
        Model = new FormModel<PwdFormInfo>(UI)
        {
            LabelSpan = 4,
            WrapperSpan = 6,
            Data = new PwdFormInfo()
        };

        await base.OnInitFormAsync();
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.FormPage(() =>
        {
            base.BuildRenderTree(builder);
            builder.FormPageButton(() =>
            {
                UI.Button(builder, Context.Language["Button.ConfirmUpdate"], this.Callback<MouseEventArgs>(OnSaveAsync), "primary");
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