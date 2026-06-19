namespace Known.Reports;

/// <summary>
/// 报表块配置弹窗组件类。
/// </summary>
public partial class ConfigForm
{
    private IReportService Service;
    private bool isAdmin;
    private List<CodeInfo> dataSourceTypes = [];
    private List<CodeInfo> entities = [];
    private List<FieldInfo> entityFields = [];

    /// <summary>
    /// 取得或设置要配置的报表块。
    /// </summary>
    [Parameter] public ReportBlock Block { get; set; }

    /// <inheritdoc />
    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        Service = await CreateServiceAsync<IReportService>();
        isAdmin = CurrentUser.IsSystemAdmin();
        dataSourceTypes = GetAvailableDataSourceTypes();
    }

    /// <inheritdoc />
    protected override async Task OnRenderAsync(bool firstRender)
    {
        await base.OnRenderAsync(firstRender);
        if (firstRender)
        {
            entities = await Service.GetEntitiesAsync();
            await LoadEntityFieldsAsync();
            SetSourceType(Block.DataSource.SourceType);
            StateChanged();
        }
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

        entityFields = await Service.GetEntityFieldsAsync(Block.DataSource.EntityName);
    }

    private void OnSourceTypeChanged(CodeInfo item)
    {
        if (Enum.TryParse<DataSourceType>(item.Code, out var parsed))
            SetSourceType(parsed);
        StateChanged();
    }

    private void SetSourceType(DataSourceType sourceType)
    {
        Block.Chart ??= new ChartConfig();
        Block.Columns ??= [];
        if (sourceType == DataSourceType.Sample)
            AutoLoadSampleColumns();
        else
            ClearAutoLoadedFields();
    }

    private void AutoLoadSampleColumns()
    {
        if (Block.BlockType == ReportBlockType.Chart)
        {
            Block.Chart.XField = "month";
            Block.Chart.YField = "amount";
        }
        else if (Block.BlockType == ReportBlockType.Table)
        {
            Block.Columns =
            [
                new() { Field = "name", Title = "名称" },
                new() { Field = "category", Title = "分类" },
                new() { Field = "price", Title = "价格" },
                new() { Field = "stock", Title = "库存" },
            ];
        }
    }

    private void ClearAutoLoadedFields()
    {
        if (Block.BlockType == ReportBlockType.Chart)
        {
            Block.Chart.XField = null;
            Block.Chart.YField = null;
            Block.Chart.CategoryField = null;
        }
        else if (Block.BlockType == ReportBlockType.Table)
        {
            Block.Columns?.Clear();
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
            entityFields = await Service.GetEntityFieldsAsync(entityName);

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
        Block?.Columns?.Add(new ColumnConfig());
        StateChanged();
    }

    private void OnDeleteColumn(ColumnConfig column)
    {
        Block?.Columns?.Remove(column);
        StateChanged();
    }
}
