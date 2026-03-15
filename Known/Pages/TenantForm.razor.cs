namespace Known.Pages;

/// <summary>
/// 租户表单组件类。
/// </summary>
public partial class TenantForm
{
    private ICompanyService Service;

    /// <inheritdoc />
    protected override async Task OnInitFormAsync()
    {
        await base.OnInitFormAsync();
        Service = await CreateServiceAsync<ICompanyService>();
    }

    /// <inheritdoc />
    protected override async Task OnRenderAsync(bool firstRender)
    {
        await base.OnRenderAsync(firstRender);
        if (firstRender)
        {
            Model.Data = await Service.GetTenantAsync(Model.Data.Id);
            StateChanged();
        }
    }

    private async Task<List<AttachInfo>> OnLogoLoad(string value)
    {
        var path = value.Replace("UploadFiles/", "");
        var info = new AttachInfo { Id = Model.Data.Code, Path = path, ThumbPath = path };
        return [info];
    }

    private Task OnFilesChanged(List<FileDataInfo> files)
    {
        Model.Files[nameof(SystemInfo.LogoPath)] = files;
        return Task.CompletedTask;
    }
}

/// <summary>
/// 租户类型表单组件类。
/// </summary>
public class CompanyTypeForm : AntForm<SysCompany> { }