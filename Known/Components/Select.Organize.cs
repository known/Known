namespace Known.Components;

/// <summary>
/// 组织架构下拉树组件类。
/// </summary>
public class OrgDropdownTree : AntDropdownTree, ICustomField
{
    private IOrganizationService Service;

    /// <summary>
    /// 取得或设置字段组件是否只读。
    /// </summary>
    [Parameter] public bool ReadOnly { get; set; }

    /// <summary>
    /// 取得或设置字段关联的栏位配置信息。
    /// </summary>
    [Parameter] public ColumnInfo Column { get; set; }

    /// <inheritdoc />
    protected override async Task OnInitializeAsync()
    {
        await base.OnInitializeAsync();
        Service = await CreateServiceAsync<IOrganizationService>();
    }

    /// <inheritdoc />
    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();
        Disabled = ReadOnly;
    }

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
        {
            var items = await Service.GetOrganizationsAsync();
            Items = [.. items.Select(d => d.ToMenuInfo())];
            Tree.Data = Items.ToMenuItems(false);
            StateHasChanged();
        }
    }
}