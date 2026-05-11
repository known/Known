namespace Known.Plugins;

/// <summary>
/// 表格插件基类，提供表格的基本功能，如查询、编辑、删除、导入导出等。
/// </summary>
/// <typeparam name="TForm">配置表单类型。</typeparam>
public class TablePlugin<TForm> : PluginBase<AutoPageInfo>
{
    private IAutoService Service;
    private readonly Dictionary<string, object> defaultData = [];
    private TableModel<Dictionary<string, object>> Table;

    /// <inheritdoc />
    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        Service = await CreateServiceAsync<IAutoService>();
        AddAction("setting", "设置", OnSetting);
        Table = new TableModel<Dictionary<string, object>>(this);
        Table.OnQuery = OnQueryModelsAsync;
    }

    /// <inheritdoc />
    protected override void BuildPlugin(RenderTreeBuilder builder)
    {
        var user = CurrentUser;
        var info = Parameter;
        if (info != null && info.Form != null && info.Form.Fields != null)
        {
            foreach (var item in info.Form.Fields)
            {
                defaultData[item.Id] = item.GetDefaultValue(user);
            }
        }

        Table.PluginId = Info?.Id;
        Table.Initialize(info);
        builder.PageTable(Table);
    }

    /// <inheritdoc />
    public override Task RefreshAsync() => Table.RefreshAsync();

    /// <summary>
    /// 新增功能。
    /// </summary>
    public void New() => Table.NewForm(SaveModelAsync, defaultData);

    /// <summary>
    /// 批量删除功能。
    /// </summary>
    public void DeleteM() => Table.DeleteM(DeleteModelsAsync);

    /// <summary>
    /// 编辑功能。
    /// </summary>
    /// <param name="row">实体对象。</param>
    public void Edit(Dictionary<string, object> row) => Table.EditForm(SaveModelAsync, row);

    /// <summary>
    /// 删除功能。
    /// </summary>
    /// <param name="row">实体对象。</param>
    public void Delete(Dictionary<string, object> row) => Table.Delete(DeleteModelsAsync, row);
    
    /// <summary>
    /// 导入功能。
    /// </summary>
    /// <returns></returns>
    public Task Import() => Table.ShowImportAsync();

    /// <summary>
    /// 导出功能。
    /// </summary>
    /// <returns></returns>
    public Task Export() => Table.ExportDataAsync();

    /// <summary>
    /// 取得插件标题。
    /// </summary>
    protected virtual string Title { get; }

    /// <summary>
    /// 取得插件默认配置。
    /// </summary>
    protected virtual AutoPageInfo Default => new() { PageType = AutoPageType.NewTable };

    /// <summary>
    /// 异步分页查询表格数据。
    /// </summary>
    /// <param name="criteria">分页查询条件。</param>
    /// <returns>分页查询结果。</returns>
    protected virtual Task<PagingResult<Dictionary<string, object>>> OnQueryModelsAsync(PagingCriteria criteria)
    {
        criteria.Parameters[nameof(AutoInfo<object>.PageId)] = AutoPage?.PageId;
        criteria.Parameters[nameof(AutoInfo<object>.PluginId)] = Info?.Id;
        return Service.QueryModelsAsync(criteria);
    }

    private Task<Result> DeleteModelsAsync(List<Dictionary<string, object>> models)
    {
        var info = new AutoInfo<List<Dictionary<string, object>>>
        {
            PageId = AutoPage?.PageId,
            PluginId = Info?.Id,
            Data = models
        };
        return Service.DeleteModelsAsync(info);
    }

    private Task<Result> SaveModelAsync(UploadInfo<Dictionary<string, object>> info)
    {
        info.PageId = AutoPage?.PageId;
        info.PluginId = Info?.Id;
        return Service.SaveModelAsync(info);
    }

    private void OnSetting()
    {
        var model = new FormModel<AutoPageInfo>(this)
        {
            Title = Title,
            Data = Parameter ?? Default,
            OnSave = async data =>
            {
                var result = await SaveParameterAsync(data);
                result.Data = data;
                return result;
            },
            OnSaved = d => StateChanged()
        };
        UI.ShowForm<TForm>(model);
    }
}

class AutoTablePlugin { }

[PagePlugin("无代码表格", "table", PagePluginType.Table, Sort = 1)]
class NoCodeTablePlugin : TablePlugin<NoCodeTableForm>
{
    protected override string Title => "无代码表格设置";
}