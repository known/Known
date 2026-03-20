namespace Known.Reports;

/// <summary>
/// 报表设计表单组件类。
/// </summary>
public partial class ReportForm
{
    private IReportService Service;
    private ReportPreviewInfo Preview = new();
    private readonly List<string> SourceTypes = [nameof(ReportSourceType.Table), nameof(ReportSourceType.Sql)];

    /// <inheritdoc />
    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        Service = await CreateServiceAsync<IReportService>();
        Model.Data.Fields ??= [];
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

        Model.Data.Fields = fields;
        await StateChangedAsync();
    }

    private async Task OnPreviewAsync()
    {
        Preview = await Service.PreviewReportAsync(Model.Data) ?? new ReportPreviewInfo();
        await StateChangedAsync();
    }

    private void OnDelete(ReportFieldInfo row) => Model.Data.Fields.Remove(row);
    private void OnMoveUp(ReportFieldInfo row) => Model.Data.Fields.MoveRow(row, true);
    private void OnMoveDown(ReportFieldInfo row) => Model.Data.Fields.MoveRow(row, false);

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