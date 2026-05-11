namespace Known.Plugins;

/// <summary>
/// 插件扩展方法类。
/// </summary>
public static class PluginExtension
{
    /// <summary>
    /// 显示插件配置表单。
    /// </summary>
    /// <typeparam name="T">表单类型。</typeparam>
    /// <param name="ui">UI 服务实例。</param>
    /// <param name="model">表单模型。</param>
    public static void ShowForm<T>(this UIService ui, FormModel<AutoPageInfo> model)
    {
        model.WrapClass = "kui-plugin-form";
        model.Class = "kui-small nopadding";
        model.Info = new FormInfo
        {
            Width = 1250,
            ShowFooter = true,
            Maximizable = true
        };
        model.Type = typeof(T);
        ui.ShowForm(model);
    }

    /// <summary>
    /// 显示快速添加数据表格
    /// </summary>
    /// <typeparam name="TTable">表格组件类型。</typeparam>
    /// <typeparam name="TItem">表格数据类型。</typeparam>
    /// <param name="service">UI 服务实例。</param>
    /// <param name="selectedItems">已选择的项。</param>
    /// <param name="onSelected">选择回调。</param>
    /// <param name="query">查询条件。</param>
    public static void ShowFastTable<TTable, TItem>(this UIService service, List<string> selectedItems, Action<List<TItem>> onSelected, string query = null) where TTable : IFastTable<TItem>
    {
        service.ShowFastTable<TTable, TItem>(Language.FastAdd, selectedItems, onSelected, query);
    }

    /// <summary>
    /// 显示快速添加数据表格。
    /// </summary>
    /// <typeparam name="TTable">表格组件类型。</typeparam>
    /// <typeparam name="TItem">表格数据类型。</typeparam>
    /// <param name="service">UI 服务实例。</param>
    /// <param name="title">对话框标题。</param>
    /// <param name="selectedItems">已选择的项。</param>
    /// <param name="onSelected">选择回调。</param>
    /// <param name="query">查询条件。</param>
    public static void ShowFastTable<TTable, TItem>(this UIService service, string title, List<string> selectedItems, Action<List<TItem>> onSelected, string query = null) where TTable : IFastTable<TItem>
    {
        TTable table = default;
        var model = new DialogModel { Title = title, Width = 800 };
        model.Content = b => b.Component<TTable>()
                              .Set(c => c.SelectedItems, selectedItems)
                              .Set(c => c.Query, query)
                              .Set(c => c.OnRowDoubleClick, row =>
                              {
                                  onSelected?.Invoke([row]);
                                  return model.CloseAsync();
                              })
                              .Build(value => table = value);
        model.OnOk = async () =>
        {
            var rows = table?.SelectedRows.ToList();
            if (rows == null || rows.Count == 0)
            {
                service.Error(Language.SelectOneAtLeast);
                return;
            }

            onSelected?.Invoke(rows);
            await model.CloseAsync();
        };
        service.ShowDialog(model);
    }

    internal static string GetScript(this ICodeGenerator generator, AutoPageInfo info)
    {
        var fields = info.Page.Columns.Select(CreateField).ToList();
        var entity = new EntityInfo { Id = info.Script, Fields = fields };
        generator.Model = new CodeModelInfo();
        return generator.GetScript(Config.DatabaseType, entity);
    }

    private static FieldInfo CreateField(PageColumnInfo info)
    {
        return new FieldInfo
        {
            Id = info.Id,
            Name = info.Name,
            Type = info.Type,
            Length = info.Length,
            Required = info.Required
        };
    }
}