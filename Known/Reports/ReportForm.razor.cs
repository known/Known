namespace Known.Reports;

/// <summary>
/// 报表设计表单组件类。
/// </summary>
public partial class ReportForm
{
    private IReportService Service;
    private ReportPreviewInfo Preview = new();
    private readonly List<string> SourceTypes = [nameof(ReportSourceType.Table), nameof(ReportSourceType.Sql)];
    private readonly List<string> ViewTypes = [nameof(ReportViewType.Table), nameof(ReportViewType.Chart), nameof(ReportViewType.Both)];
    private readonly List<string> ChartTypes = [nameof(ReportChartType.Bar), nameof(ReportChartType.Line), nameof(ReportChartType.Pie)];
    private readonly List<string> SortTypes = [string.Empty, "asc", "desc"];
    private readonly string DesignerId = $"reportDesigner_{Utils.GetGuid()}";
    private DotNetObjectReference<ReportForm> invoker;
    private bool designerReady;
    private List<ReportFieldInfo> SourceFields = [];

    /// <inheritdoc />
    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        Service = await CreateServiceAsync<IReportService>();
        invoker = DotNetObjectReference.Create(this);
        Model.Data.Fields ??= [];
        await LoadSourceFieldsAsync(false);
    }

    /// <inheritdoc />
    protected override async Task OnRenderAsync(bool firstRender)
    {
        await base.OnRenderAsync(firstRender);
        if (!Model.IsView)
        {
            await JSRuntime.InvokeJsAsync("KReport.initDesigner", DesignerId, invoker);
            designerReady = true;
        }
    }

    /// <inheritdoc />
    protected override Task OnDisposeAsync()
    {
        invoker?.Dispose();
        return base.OnDisposeAsync();
    }

    private void OnAddField()
    {
        Model.Data.Fields.Add(new ReportFieldInfo
        {
            Type = FieldType.Text,
            IsVisible = true,
            IsSort = true
        });
    }

    private async Task OnLoadFieldsAsync()
    {
        if (string.IsNullOrWhiteSpace(Model.Data.Source))
        {
            UI.Error("请先设置数据源！");
            return;
        }

        var fields = await Service.GetReportFieldsAsync(Model.Data.SourceType, Model.Data.Source);
        if (fields == null || fields.Count == 0)
        {
            UI.Info("当前数据源没有可加载字段，请手工维护字段定义。");
            return;
        }

        SourceFields = [.. fields.Select(CloneField)];
        Model.Data.Fields = [.. fields.Select(CloneField)];
        designerReady = false;
        await StateChangedAsync();
    }

    private async Task OnPreviewAsync()
    {
        Preview = await Service.PreviewReportAsync(Model.Data) ?? new ReportPreviewInfo();
        await StateChangedAsync();
    }

    private async Task OnRemoveFieldAsync(string fieldId)
    {
        var item = Model.Data.Fields.FirstOrDefault(f => f.Id == fieldId);
        if (item != null)
        {
            Model.Data.Fields.Remove(item);
            await StateChangedAsync();
        }
    }

    private void OnDelete(ReportFieldInfo row) => Model.Data.Fields.Remove(row);
    private void OnMoveUp(ReportFieldInfo row) => Model.Data.Fields.MoveRow(row, true);
    private void OnMoveDown(ReportFieldInfo row) => Model.Data.Fields.MoveRow(row, false);

    [JSInvokable]
    public async Task UpdateDesignerFields(string[] ids)
    {
        if (ids == null)
            return;

        var newFields = new List<ReportFieldInfo>();
        foreach (var id in ids)
        {
            if (string.IsNullOrWhiteSpace(id))
                continue;

            var exist = Model.Data.Fields.FirstOrDefault(f => f.Id == id);
            if (exist != null)
            {
                newFields.Add(exist);
                continue;
            }

            var source = SourceFields.FirstOrDefault(f => f.Id == id);
            if (source != null)
                newFields.Add(CloneField(source));
        }

        Model.Data.Fields = newFields;
        await StateChangedAsync();
    }

    private async Task LoadSourceFieldsAsync(bool refreshState = true)
    {
        if (string.IsNullOrWhiteSpace(Model.Data.Source))
        {
            SourceFields = [.. Model.Data.Fields.Select(CloneField)];
            return;
        }

        var fields = await Service.GetReportFieldsAsync(Model.Data.SourceType, Model.Data.Source);
        if (fields != null && fields.Count > 0)
            SourceFields = [.. fields.Select(CloneField)];
        else
            SourceFields = [.. Model.Data.Fields.Select(CloneField)];

        if (refreshState)
            await StateChangedAsync();
    }

    private string GetPreviewText(ReportFieldInfo field, Dictionary<string, object> row)
    {
        var value = row?.GetValue(field.Id);
        if (value == null)
            return string.Empty;
        if (!string.IsNullOrWhiteSpace(field.Category))
            return Cache.GetCodeName(field.Category, value.ToString());
        if (field.Type == FieldType.Date)
            return Utils.ConvertTo<DateTime?>(value)?.ToString(Config.DateFormat);
        if (field.Type == FieldType.DateTime)
            return Utils.ConvertTo<DateTime?>(value)?.ToString(Config.DateTimeFormat);
        if (field.Type == FieldType.Switch || field.Type == FieldType.CheckBox)
            return Utils.ConvertTo<bool>(value) ? "是" : "否";
        return value.ToString();
    }

    private static ReportFieldInfo CloneField(ReportFieldInfo field)
    {
        return new ReportFieldInfo
        {
            Id = field.Id,
            Name = field.Name,
            Expression = field.Expression,
            Type = field.Type,
            Category = field.Category,
            Width = field.Width,
            Align = field.Align,
            IsQuery = field.IsQuery,
            IsVisible = field.IsVisible,
            IsSort = field.IsSort,
            DefaultSort = field.DefaultSort,
            Unit = field.Unit,
            Note = field.Note
        };
    }
}

/// <summary>
/// 报表表单组件类。
/// </summary>
public class ReportTypeForm : AntForm<SysReport> { }

/// <summary>
/// 报表字段表格组件类。
/// </summary>
public class ReportFieldTable : AntTable<ReportFieldInfo> { }

/// <summary>
/// 报表字段类型选择组件类。
/// </summary>
public class ReportFieldTypeSelect : AntSelectEnum<FieldType> { }