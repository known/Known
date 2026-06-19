namespace Known.Reports;

/// <summary>
/// 报表块配置弹窗组件类。
/// </summary>
public partial class ConfigForm
{
    /// <summary>
    /// 取得或设置要配置的报表块。
    /// </summary>
    [Parameter] public ReportBlock Block { get; set; }

    private List<CodeInfo> entities = [];
    private List<FieldInfo> entityFields = [];

    /// <inheritdoc />
    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        var service = await CreateServiceAsync<IReportService>();
        entities = await service.GetEntitiesAsync();
        await LoadEntityFieldsAsync();
    }

    private async Task LoadEntityFieldsAsync()
    {
        if (Block?.DataSource == null || string.IsNullOrWhiteSpace(Block.DataSource.EntityName))
        {
            entityFields = [];
            return;
        }

        var service = await CreateServiceAsync<IReportService>();
        entityFields = await service.GetEntityFieldsAsync(Block.DataSource.EntityName);
    }

    private async Task OnEntityChanged(string entityName)
    {
        if (Block?.DataSource == null)
            return;

        Block.DataSource.EntityName = entityName;
        var entity = entities.FirstOrDefault(e => e.Code == entityName);
        Block.DataSource.EntityDisplayName = entity?.Name ?? entityName;
        Block.DataSource.EntityFields?.Clear();
        entityFields = [];

        if (!string.IsNullOrWhiteSpace(entityName))
        {
            var service = await CreateServiceAsync<IReportService>();
            entityFields = await service.GetEntityFieldsAsync(entityName);
        }

        StateChanged();
    }

    private void OnAddEntityField()
    {
        if (Block?.DataSource?.EntityFields == null)
            return;

        Block.DataSource.EntityFields.Add(new EntityFieldConfig());
        StateChanged();
    }

    private void OnDeleteEntityField(EntityFieldConfig field)
    {
        Block?.DataSource?.EntityFields?.Remove(field);
        StateChanged();
    }

    private void OnAddColumn()
    {
        Block?.Table?.Columns?.Add(new ColumnConfig());
        StateChanged();
    }

    private void OnDeleteColumn(ColumnConfig column)
    {
        Block?.Table?.Columns?.Remove(column);
        StateChanged();
    }
}
