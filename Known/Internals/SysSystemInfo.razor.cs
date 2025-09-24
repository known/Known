namespace Known.Internals;

/// <summary>
/// 系统信息组件。
/// </summary>
public partial class SysSystemInfo
{
    private ISystemService Service;
    private SystemDataInfo Model = new();

    /// <inheritdoc />
    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        Service = await CreateServiceAsync<ISystemService>();
    }

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
        {
            Model = await Service.GetSystemDataAsync();
            StateChanged();
        }
    }

    private async Task OnSaveAppNameAsync(string value)
    {
        if (Model.System == null)
            return;

        Model.System.AppName = value;
        var result = await Service.SaveSystemAsync(Model.System);
        if (result.IsValid)
        {
            CurrentUser.AppName = value;
            await App?.StateChangedAsync();
        }
    }

    private async Task OnSaveProductKeyAsync(string value)
    {
        if (Model.System == null)
            return;

        Model.System.ProductKey = value;
        await Admin.SaveProductKeyAsync(new ActiveInfo
        {
            ProductId = Model.System.ProductId,
            ProductKey = Model.System.ProductKey
        });
        await StateChangedAsync();
    }
}