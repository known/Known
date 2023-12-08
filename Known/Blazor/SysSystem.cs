using Known.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Blazor;

class SysSystem : BaseTabPage
{
    internal SystemInfo Data { get; private set; }

    protected override async Task OnInitPageAsync()
    {
        await base.OnInitPageAsync();
		Data = await Platform.System.GetSystemAsync();

		Tab.Items.Add(new ItemModel("系统信息") { Content = builder => builder.Component<SysSystemInfo>().Build() });
		Tab.Items.Add(new ItemModel("安全设置") { Content = builder => builder.Component<SysSystemSafe>().Build() });
    }

	protected override void BuildRenderTree(RenderTreeBuilder builder) => builder.Cascading(this, base.BuildRenderTree);
}

class SysSystemInfo : BaseForm<SystemInfo>
{
    [CascadingParameter] private SysSystem Parent { get; set; }

    protected override async Task OnInitFormAsync()
    {
        await base.OnInitFormAsync();

        Model = new FormModel<SystemInfo>(UI)
        {
            LabelSpan = 4,
            WrapperSpan = 10,
            Data = Parent.Data
        };
        //TODO：添加表单列模板
        //Model.AddRow().AddColumn(c => c.CompNo);
        /*
         <FormItem Label="企业名称">
            @($"{context.CompNo}-{context.CompName}")
        </FormItem>
        <FormItem Label="系统名称">
            <EditInput Value="@context.AppName" OnSave="OnSaveAppName" />
        </FormItem>
        <FormItem Label="系统版本">
            @Config.Version.AppVersion
        </FormItem>
        <FormItem Label="软件版本">
            @Config.Version.SoftVersion
        </FormItem>
        <FormItem Label="框架版本">
            @Config.Version.FrameVersion
        </FormItem>
        @if (!Config.App.IsPlatform && !string.IsNullOrWhiteSpace(Config.App.ProductId))
        {
            <FormItem Label="产品ID">
                @Config.App.ProductId
            </FormItem>
            <FormItem Label="产品密钥">
                <EditInput Value="@context.ProductKey" OnSave="OnSaveProductKey" />
            </FormItem>
        }
        <FormItem Label="版权信息">
            @context.Copyright
        </FormItem>
        <FormItem Label="软件许可">
            @context.SoftTerms
        </FormItem>
         */
    }

    private async void OnSaveAppName(string value)
	{
		Model.Data.AppName = value;
		var result = await Platform.System.SaveSystemAsync(Model.Data);
		if (result.IsValid)
		{
			CurrentUser.AppName = value;
			Context.RefreshPage();
		}
	}

	private async void OnSaveProductKey(string value)
	{
		Model.Data.ProductKey = value;
		await Platform.System.SaveSystemAsync(Model.Data);
	}
}

class SysSystemSafe : BaseForm<SystemInfo>
{
	[CascadingParameter] private SysSystem Parent { get; set; }

    protected override async Task OnInitFormAsync()
    {
        await base.OnInitFormAsync();

        Model = new FormModel<SystemInfo>(UI)
        {
            LabelSpan = 4,
            WrapperSpan = 10,
            Data = Parent.Data
        };
        //Model.AddRow().AddColumn(c => c.CompNo);
        /*
         <FormItem Label="默认密码">
                <EditInput Value="@context.UserDefaultPwd" OnSave="OnSaveDefaultPwd" />
            </FormItem>
         */
    }

    private async void OnSaveDefaultPwd(string value)
	{
		Model.Data.UserDefaultPwd = value;
		await Platform.System.SaveSystemAsync(Model.Data);
	}
}