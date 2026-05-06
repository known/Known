namespace Known.Plugins;

/// <summary>
/// 单表无代码增删改查导表格插件。
/// </summary>
[PagePlugin("单表无代码表格", "table", PagePluginType.Table, Sort = 1)]
public class NoCodeTablePlugin : PluginBase<AutoPageInfo>
{
    private IAutoService service;
    private readonly Dictionary<string, object> defaultData = [];
    private TableModel<Dictionary<string, object>> table;

    /// <inheritdoc />
    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        service = await CreateServiceAsync<IAutoService>();
        AddAction("setting", "设置", OnSetting);

        table = new TableModel<Dictionary<string, object>>(this)
        {
            PluginId = Info?.Id
        };
        table.OnQuery = QueryModelsAsync;
    }

    /// <inheritdoc />
    protected override void BuildPlugin(RenderTreeBuilder builder)
    {
        var info = GetParameter();
        BuildDefaultData(info);

        table.PluginId = Info?.Id;
        table.Initialize(info);
        builder.PageTable(table);
    }

    /// <inheritdoc />
    public override Task RefreshAsync() => table?.RefreshAsync() ?? Task.CompletedTask;

    /// <summary>
    /// 新增一条数据。
    /// </summary>
    public void New() => table.NewForm(SaveModelAsync, defaultData);

    /// <summary>
    /// 编辑一条数据。
    /// </summary>
    /// <param name="row">当前行数据。</param>
    public void Edit(Dictionary<string, object> row) => table.EditForm(SaveModelAsync, row);

    /// <summary>
    /// 删除一条数据。
    /// </summary>
    /// <param name="row">当前行数据。</param>
    public void Delete(Dictionary<string, object> row) => table.Delete(DeleteModelsAsync, row);

    /// <summary>
    /// 批量删除数据。
    /// </summary>
    public void DeleteM() => table.DeleteM(DeleteModelsAsync);

    /// <summary>
    /// 导入数据。
    /// </summary>
    public Task Import() => table.ShowImportAsync();

    /// <summary>
    /// 导出数据。
    /// </summary>
    public Task Export() => table.ExportDataAsync();

    private AutoPageInfo GetParameter()
    {
        var info = Parameter ?? CreateDefault();
        info.Page ??= new PageInfo();
        info.Form ??= new FormInfo();
        info.Page.Tools ??= [];
        info.Page.Actions ??= [];
        info.Page.Columns ??= [];
        info.Form.Fields ??= [];
        info.Id ??= AutoPage?.PageId;
        info.Name ??= Page?.Menu?.Name;
        info.PageUrl ??= Page?.Menu?.Url;
        return info;
    }

    private AutoPageInfo CreateDefault()
    {
        var info = new AutoPageInfo
        {
            Id = AutoPage?.PageId,
            Name = Page?.Menu?.Name,
            PageUrl = Page?.Menu?.Url,
            PageType = AutoPageType.NewTable,
            Page = new PageInfo(),
            Form = new FormInfo { Width = 900, ShowFooter = true, Maximizable = true }
        };

        info.Page.Tools =
        [
            new ActionInfo("New"),
            new ActionInfo("DeleteM"),
            new ActionInfo("Import"),
            new ActionInfo("Export")
        ];
        info.Page.Actions =
        [
            new ActionInfo("Edit"),
            new ActionInfo("Delete")
        ];

        return info;
    }

    private void BuildDefaultData(AutoPageInfo info)
    {
        defaultData.Clear();
        foreach (var item in info.Form.Fields)
        {
            defaultData[item.Id] = item.GetDefaultValue(CurrentUser);
        }
    }

    private Task<PagingResult<Dictionary<string, object>>> QueryModelsAsync(PagingCriteria criteria)
    {
        criteria.Parameters[nameof(AutoInfo<object>.PageId)] = AutoPage?.PageId;
        criteria.Parameters[nameof(AutoInfo<object>.PluginId)] = Info?.Id;
        return service.QueryModelsAsync(criteria);
    }

    private Task<Result> DeleteModelsAsync(List<Dictionary<string, object>> models)
    {
        var info = new AutoInfo<List<Dictionary<string, object>>>
        {
            PageId = AutoPage?.PageId,
            PluginId = Info?.Id,
            Data = models
        };
        return service.DeleteModelsAsync(info);
    }

    private Task<Result> SaveModelAsync(UploadInfo<Dictionary<string, object>> info)
    {
        info.PageId = AutoPage?.PageId;
        info.PluginId = Info?.Id;
        return service.SaveModelAsync(info);
    }

    private void OnSetting()
    {
        var model = new FormModel<AutoPageInfo>(this)
        {
            Title = "单表无代码表格设置",
            Data = CloneParameter(GetParameter()),
            Type = typeof(NoCodeTablePluginForm),
            Info = new FormInfo
            {
                Width = 1200,
                ShowFooter = true,
                Maximizable = true,
                DefaultMaximized = true,
                WrapClass = "kui-plugin-form"
            },
            OnSave = async data =>
            {
                Normalize(data);
                var result = await SaveParameterAsync(data);
                result.Data = data;
                return result;
            },
            OnSavedAsync = async _ => await StateChangedAsync()
        };
        UI.ShowForm(model);
    }

    private static AutoPageInfo CloneParameter(AutoPageInfo info)
    {
        return Utils.FromJson<AutoPageInfo>(Utils.ToJson(info));
    }

    private static void Normalize(AutoPageInfo info)
    {
        info.Page ??= new PageInfo();
        info.Form ??= new FormInfo();
        info.Page.Tools ??= [];
        info.Page.Actions ??= [];
        info.Page.Columns ??= [];
        info.Form.Fields ??= [];

        info.Page.Columns = [.. info.Page.Columns.Where(c => !string.IsNullOrWhiteSpace(c.Id))];
        info.Form.Fields = [.. info.Form.Fields.Where(f => !string.IsNullOrWhiteSpace(f.Id))];

        foreach (var column in info.Page.Columns)
        {
            column.Name ??= column.Id;
        }

        foreach (var field in info.Form.Fields)
        {
            field.Name ??= field.Id;
        }
    }
}