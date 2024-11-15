namespace Known.Extensions;

/// <summary>
/// UI服务扩展类。
/// </summary>
public static class UIExtension
{
    #region Form
    /// <summary>
    /// 呈现表单页面内容。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="child">子内容委托。</param>
    public static void FormPage(this RenderTreeBuilder builder, Action child) => builder.Div("kui-form-page", child);

    /// <summary>
    /// 呈现表单页面操作按钮。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="child">子内容委托。</param>
    public static void FormPageButton(this RenderTreeBuilder builder, Action child) => builder.Div("kui-form-page-button", child);

    /// <summary>
    /// 呈现表单操作按钮。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="child">子内容委托。</param>
    public static void FormButton(this RenderTreeBuilder builder, Action child) => builder.Div("kui-form-button", child);

    /// <summary>
    /// 呈现表单操作按钮。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="child">子内容委托。</param>
    public static void FormAction(this RenderTreeBuilder builder, Action child) => builder.Div("kui-form-action", child);

    /// <summary>
    /// 呈现表单标题。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="text">表单标题。</param>
    public static void FormTitle(this RenderTreeBuilder builder, string text) => builder.Component<KTitle>().Set(c => c.Text, text).Build();
    #endregion

    #region Page
    /// <summary>
    /// 呈现表格组件。
    /// </summary>
    /// <typeparam name="TItem">表格数据类型。</typeparam>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="model">表格配置模型。</param>
    public static void Table<TItem>(this RenderTreeBuilder builder, TableModel<TItem> model) where TItem : class, new()
    {
        builder.Component<TablePage<TItem>>().Set(c => c.Model, model).Build();
    }

    internal static void Build404Page(this UIService service, RenderTreeBuilder builder, string pageId)
    {
        service.BuildResult(builder, "404", $"{service.Language["Tip.Page404"]}PageId={pageId}");
    }

    /// <summary>
    /// 异步查询数据，显示Loading提示。
    /// </summary>
    /// <param name="app">模板基类实例。</param>
    /// <param name="action">查询数据委托。</param>
    /// <returns></returns>
    public static Task QueryDataAsync(this BaseLayout app, Func<Task> action)
    {
        return app?.ShowSpinAsync(app?.Language["Tip.DataQuering"], action);
    }

    /// <summary>
    /// 异步导出表格数据，默认按查询结果导出。
    /// </summary>
    /// <typeparam name="TItem">导出数据类型。</typeparam>
    /// <param name="app">模板基类实例。</param>
    /// <param name="table">导出表格模型对象实例。</param>
    /// <param name="name">导出文件名。</param>
    /// <param name="mode">导出模式（单页，查询结果，全部）。</param>
    /// <returns></returns>
    public static async Task ExportDataAsync<TItem>(this BaseLayout app, TableModel<TItem> table, string name, ExportMode mode = ExportMode.Query) where TItem : class, new()
    {
        await app?.ShowSpinAsync(app?.Language["Tip.DataExporting"], async () =>
        {
            table.Criteria.ExportMode = mode;
            table.Criteria.ExportColumns = table.GetExportColumns();
            var result = await table.OnQuery?.Invoke(table.Criteria);
            table.Criteria.ExportMode = ExportMode.None;
            var bytes = result.ExportData;
            if (bytes != null && bytes.Length > 0)
            {
                var stream = new MemoryStream(bytes);
                await app.JS.DownloadFileAsync($"{name}.xlsx", stream);
            }
        });
    }

    private static List<ExportColumnInfo> GetExportColumns<TItem>(this TableModel<TItem> table) where TItem : class, new()
    {
        var columns = new List<ExportColumnInfo>();
        if (table.Columns == null || table.Columns.Count == 0)
            return columns;

        foreach (var item in table.Columns)
        {
            columns.Add(new ExportColumnInfo
            {
                Id = item.Id,
                Name = item.Name,
                Category = item.Category,
                Type = item.Type
            });
        }
        return columns;
    }
    #endregion

    #region Form
    /// <summary>
    /// 呈现一个提示框。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="text">提示文本。</param>
    /// <param name="type">提示框类型，默认Info。</param>
    public static void Alert(this RenderTreeBuilder builder, string text, StyleType type = StyleType.Info)
    {
        builder.Component<KAlert>()
               .Set(c => c.Text, text)
               .Set(c => c.Type, type)
               .Build();
    }

    /// <summary>
    /// 呈现一个标签。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="text">标签文本。</param>
    /// <param name="color">标签颜色。</param>
    public static void Tag(this RenderTreeBuilder builder, string text, string color = null)
    {
        builder.Component<KTag>()
               .Set(c => c.Text, text)
               .Set(c => c.Color, color)
               .Build();
    }

    /// <summary>
    /// 呈现一个图标。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="icon">图标。</param>
    /// <param name="onClick">图标单击事件。</param>
    public static void Icon(this RenderTreeBuilder builder, string icon, EventCallback<MouseEventArgs>? onClick = null)
    {
        if (string.IsNullOrWhiteSpace(icon))
            return;

        builder.Component<KIcon>()
               .Set(c => c.Icon, icon)
               .Set(c => c.OnClick, onClick)
               .Build();
    }

    /// <summary>
    /// 呈现一个分组框。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="title">分组框标题。</param>
    /// <param name="child">子内容委托。</param>
    public static void GroupBox(this RenderTreeBuilder builder, string title, Action child)
    {
        builder.Div("kui-group-box", () =>
        {
            builder.Label().Class("legend").Text(title).Close();
            builder.Div("body", child);
        });
    }

    /// <summary>
    /// 呈现附件超链接。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="item">系统附件对象。</param>
    public static void FileLink(this RenderTreeBuilder builder, AttachInfo item)
    {
        builder.Component<FileLink>().Set(c => c.Item, item).Build();
    }
    #endregion

    #region Button
    /// <summary>
    /// 呈现一个按钮。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="action">操作信息。</param>
    /// <param name="onClick">按钮单击事件。</param>
    public static void Button(this RenderTreeBuilder builder, ActionInfo action, EventCallback<MouseEventArgs> onClick)
    {
        action.OnClick = onClick;
        builder.Component<KButton>().Set(c => c.Action, action).Build();
    }

    /// <summary>
    /// 呈现一个按钮。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="name">按钮名称。</param>
    /// <param name="onClick">按钮单击事件。</param>
    /// <param name="style">按钮样式，默认primary。</param>
    public static void Button(this RenderTreeBuilder builder, string name, EventCallback<MouseEventArgs> onClick, string style = "primary")
    {
        var action = new ActionInfo { Name = name, OnClick = onClick, Style = style };
        builder.Component<KButton>().Set(c => c.Action, action).Build();
    }
    #endregion

    #region Toast
    /// <summary>
    /// 呈现信息提示。
    /// </summary>
    /// <param name="service">UI服务。</param>
    /// <param name="message">提示文本。</param>
    public static async void Info(this UIService service, string message) => await service.InfoAsync(message);

    /// <summary>
    /// 异步呈现信息提示。
    /// </summary>
    /// <param name="service">UI服务。</param>
    /// <param name="message">提示文本。</param>
    /// <returns></returns>
    public static Task InfoAsync(this UIService service, string message) => service.Toast(message, StyleType.Info);

    /// <summary>
    /// 呈现警告提示。
    /// </summary>
    /// <param name="service">UI服务。</param>
    /// <param name="message">警告文本。</param>
    public static async void Warning(this UIService service, string message) => await service.WarningAsync(message);

    /// <summary>
    /// 异步呈现警告提示。
    /// </summary>
    /// <param name="service">UI服务。</param>
    /// <param name="message">警告文本。</param>
    /// <returns></returns>
    public static Task WarningAsync(this UIService service, string message) => service.Toast(message, StyleType.Warning);

    /// <summary>
    /// 呈现错误提示。
    /// </summary>
    /// <param name="service">UI服务。</param>
    /// <param name="message">错误文本。</param>
    public static async void Error(this UIService service, string message) => await service.ErrorAsync(message);

    /// <summary>
    /// 异步呈现错误提示。
    /// </summary>
    /// <param name="service">UI服务。</param>
    /// <param name="message">错误文本。</param>
    /// <returns></returns>
    public static Task ErrorAsync(this UIService service, string message) => service.Toast(message, StyleType.Error);

    /// <summary>
    /// 显示后端返回的操作结果。
    /// </summary>
    /// <param name="service">UI服务。</param>
    /// <param name="result">后端操作结果。</param>
    /// <param name="onSuccess">后端操作成功回调委托。</param>
    public static async void Result(this UIService service, Result result, Func<Task> onSuccess = null) => await service.ResultAsync(result, onSuccess);

    /// <summary>
    /// 异步显示后端返回的操作结果。
    /// </summary>
    /// <param name="service">UI服务。</param>
    /// <param name="result">后端操作结果。</param>
    /// <param name="onSuccess">后端操作成功回调委托。</param>
    /// <returns></returns>
    public static async Task ResultAsync(this UIService service, Result result, Func<Task> onSuccess = null)
    {
        if (!result.IsValid)
        {
            await service.ErrorAsync(result.Message);
            return;
        }

        if (onSuccess != null)
            await onSuccess.Invoke();
        await service.Toast(result.Message);
    }
    #endregion
}