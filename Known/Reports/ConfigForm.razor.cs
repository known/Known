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

    private bool isAdmin;
    private List<CodeInfo> dataSourceTypes = [];
    private List<CodeInfo> entities = [];
    private List<FieldInfo> entityFields = [];

    /// <inheritdoc />
    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        isAdmin = CurrentUser.IsSystemAdmin();
        dataSourceTypes = GetAvailableDataSourceTypes();
        var service = await CreateServiceAsync<IReportService>();
        entities = await service.GetEntitiesAsync();
        await LoadEntityFieldsAsync();
    }

    private List<CodeInfo> GetAvailableDataSourceTypes()
    {
        var types = new List<CodeInfo>
        {
            new(nameof(DataSourceType.Sample), "示例数据"),
        };

        if (isAdmin)
        {
            types.Add(new(nameof(DataSourceType.SQL), "SQL语句"));
            types.Add(new(nameof(DataSourceType.Api), "API接口"));
        }

        types.Add(new(nameof(DataSourceType.Entity), "系统实体"));
        return types;
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

    private async Task OnSourceTypeChanged(CodeInfo item)
    {
        Block.Chart ??= new ChartConfig();
        Block.Table ??= new TableConfig();
        if (Enum.TryParse<DataSourceType>(item.Code, out var parsed))
        {
            if (parsed == DataSourceType.Sample)
                AutoLoadSampleColumns();
        }
        StateChanged();
    }

    private void AutoLoadSampleColumns()
    {
        if (Block.BlockType == ReportBlockType.Chart)
        {
            Block.Chart.XField = "month";
            Block.Chart.YField = "amount";
            Block.Chart.Title = Block.Title;
        }
        else if (Block.BlockType == ReportBlockType.Table)
        {
            Block.Table.Columns =
            [
                new() { Field = "name", Title = "名称" },
                new() { Field = "category", Title = "分类" },
                new() { Field = "price", Title = "价格" },
                new() { Field = "stock", Title = "库存" },
            ];
        }
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
