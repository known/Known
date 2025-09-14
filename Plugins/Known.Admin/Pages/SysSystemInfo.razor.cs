namespace Known.Pages;

public partial class SysSystemInfo
{
    private SystemDataInfo Model = new();
    [CascadingParameter] private SysSystem Parent { get; set; }

    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        // 首个Tab组件初始化Model
        Parent.Model = await Parent.GetSystemDataAsync();
        Model = Parent.Model;
    }

    private async Task OnSaveAppNameAsync(string value)
    {
        if (Model.System == null)
            return;

        Model.System.AppName = value;
        var result = await Parent.SaveSystemAsync(Model.System);
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
        await Parent.SaveKeyAsync(Model.System);
        await StateChangedAsync();
    }
}