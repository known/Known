using Known.Entities;
using Known.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace Known.Blazor;

class SysUserProfile : BasePage<SysUser>
{
    internal SysUser User { get; private set; }

    protected override async Task OnInitPageAsync()
    {
        await base.OnInitPageAsync();

        User = await Platform.User.GetUserAsync(CurrentUser.Id);

        Page.Type = PageType.Column;
        Page.Spans = [4, 20];
        Page.Contents = [BuildUserInfo, BuildUserTabs];
    }

    private void BuildUserInfo(RenderTreeBuilder builder) => builder.Div("p10", () => builder.Component<SysUserProfileInfo>().Build());
    private void BuildUserTabs(RenderTreeBuilder builder) => builder.Component<SysUserProfileTabs>().Build();

    protected override void BuildRenderTree(RenderTreeBuilder builder) => builder.Cascading(this, base.BuildRenderTree);
}

class SysUserProfileInfo : BaseComponent
{
    [CascadingParameter] private SysUserProfile Parent { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        var User = Parent.User;
        builder.Div("avatar", () => builder.Markup($"<img src=\"{CurrentUser?.AvatarUrl}\" />"));
        builder.Ul("userInfo", () =>
        {
            builder.Li(() =>
            {
                UI.BuildIcon(builder, "user");
                builder.Span($"{User?.Name}({User?.UserName}");
            });
            builder.Li(() =>
            {
                UI.BuildIcon(builder, "phone");
                builder.Span(User?.Phone);
            });
            builder.Li(() =>
            {
                UI.BuildIcon(builder, "mobile");
                builder.Span(User?.Mobile);
            });
            builder.Li(() =>
            {
                UI.BuildIcon(builder, "inbox");
                builder.Span(User?.Email);
            });
            builder.Li(() =>
            {
                UI.BuildIcon(builder, "team");
                builder.Span(User?.Role);
            });
            builder.Li(() =>
            {
                UI.BuildIcon(builder, "comment");
                builder.Span(User?.Note);
            });
        });
    }
}

class SysUserProfileTabs : BaseTabPage
{
    protected override async Task OnInitPageAsync()
    {
        await base.OnInitPageAsync();
        //Tab.Items.Add(new ItemModel("待办事项") { Content = builder => builder.Component<SysSystemInfo>().Build() });
        //Tab.Items.Add(new ItemModel("我的消息") { Content = builder => builder.Component<SysSystemSafe>().Build() });
        Tab.Items.Add(new ItemModel("我的信息") { Content = builder => builder.Component<SysUserProfileTabsInfo>().Build() });
        Tab.Items.Add(new ItemModel("安全设置") { Content = builder => builder.Component<SysUserProfileTabsSafe>().Build() });
    }
}

class SysUserProfileTabsInfo : BaseForm<SysUser>
{
    private bool isEdit = false;
    [CascadingParameter] private SysUserProfile Parent { get; set; }

    protected override async Task OnInitFormAsync()
    {
        await base.OnInitFormAsync();

        Model = new FormModel<SysUser>(UI)
        {
            LabelSpan = 4,
            WrapperSpan = 8,
            IsView = true,
            Data = Parent.User
        };
        /*
         <FormItem Label="用户名">
            @context.UserName
        </FormItem>
        <FormItem>
            <Input @bind-Value="@context.Name" Disabled="@(!IsEdit)" />
        </FormItem>
        <FormItem>
            <Input @bind-Value="@context.EnglishName" Disabled="@(!IsEdit)" />
        </FormItem>
        <FormItem>
            <RadioGroup @bind-Value="@context.Gender" Disabled="@(!IsEdit)">
                <Radio Value="@("男")">男</Radio>
                <Radio Value="@("女")">女</Radio>
            </RadioGroup>
        </FormItem>
        <FormItem>
            <Input @bind-Value="@context.Phone" Disabled="@(!IsEdit)" />
        </FormItem>
        <FormItem>
            <Input @bind-Value="@context.Mobile" Disabled="@(!IsEdit)" />
        </FormItem>
        <FormItem>
            <Input @bind-Value="@context.Email" Disabled="@(!IsEdit)" />
        </FormItem>
        <FormItem Label="角色">
            @context.Role
        </FormItem>
        <FormItem>
            <TextArea @bind-Value="@context.Note" Disabled="@(!IsEdit)" />
        </FormItem>
        <FormItem WrapperColOffset="labelCol" WrapperColSpan="valueCol">
            @if (!IsEdit)
            {
                <Button Type="@ButtonType.Primary" OnClick="e=>OnEdit(true)">编辑</Button>
            }
            else
            {
                <Button Type="@ButtonType.Primary" OnClick="e=>OnSaveUserInfo()">保存</Button>
                <Button Type="@ButtonType.Default" OnClick="e=>OnEdit(false)">取消</Button>
            }
        </FormItem>
         */
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Div("form-company", () =>
        {
            Model.IsView = !isEdit;
            base.BuildRenderTree(builder);
            builder.Div("col-offset-4", () =>
            {
                if (!isEdit)
                {
                    UI.Button(builder, "编辑", Callback<MouseEventArgs>(e => OnEdit(true)), "primary");
                }
                else
                {
                    UI.Button(builder, "保存", Callback<MouseEventArgs>(e => OnSave()), "primary");
                    UI.Button(builder, "取消", Callback<MouseEventArgs>(e => OnEdit(false)), "default");
                }
            });
        });
    }

    private async void OnSave()
    {
        if (!Model.Validate())
            return;

        var result = await Platform.Auth.UpdateUserAsync(Model.Data);
        UI.Result(result, () => OnEdit(false));
    }

    private void OnEdit(bool edit) => isEdit = edit;
}

class SysUserProfileTabsSafe : BaseForm<PwdFormInfo>
{
    protected override async Task OnInitFormAsync()
    {
        await base.OnInitFormAsync();

        Model = new FormModel<PwdFormInfo>(UI)
        {
            LabelSpan = 4,
            WrapperSpan = 6,
            Data = new PwdFormInfo()
        };
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        base.BuildRenderTree(builder);
        builder.Div("col-offset-4", () =>
        {
            UI.Button(builder, "确定修改", Callback<MouseEventArgs>(e => OnSave()), "primary");
        });
    }

    private async void OnSave()
    {
        if (!Model.Validate())
            return;

        var result = await Platform.Auth.UpdatePasswordAsync(Model.Data);
        UI.Result(result);
    }
}