namespace Known.Extensions;

public static class UIExtension
{
    #region Form
    public static void FormPage(this RenderTreeBuilder builder, Action child) => builder.Div("kui-form-page", child);
    public static void FormPageButton(this RenderTreeBuilder builder, Action child) => builder.Div("kui-form-page-button", child);
    internal static void FormButton(this RenderTreeBuilder builder, Action child) => builder.Div("kui-form-button", child);
    public static void FormAction(this RenderTreeBuilder builder, Action child) => builder.Div("kui-form-action", child);
    public static void FormTitle(this RenderTreeBuilder builder, string text) => builder.Component<KTitle>().Set(c => c.Text, text).Build();
    #endregion

    #region Page
    public static void Table<TItem>(this RenderTreeBuilder builder, TableModel<TItem> model) where TItem : class, new()
    {
        builder.Component<TablePage<TItem>>().Set(c => c.Model, model).Build();
    }

    internal static void Build404Page(this IUIService service, RenderTreeBuilder builder, string pageId)
    {
        service.BuildResult(builder, "404", $"{service.Language["Tip.Page404"]}PageId={pageId}");
    }

    public static Task QueryDataAsync(this BaseLayout app, Func<Task> action)
    {
        return app?.ShowSpinAsync(app?.Language["Tip.DataQuering"], action);
    }

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
    public static void Alert(this RenderTreeBuilder builder, string text, StyleType type = StyleType.Info)
    {
        builder.Component<KAlert>()
               .Set(c => c.Text, text)
               .Set(c => c.Type, type)
               .Build();
    }

    public static void Tag(this RenderTreeBuilder builder, string text, string color = null)
    {
        builder.Component<KTag>()
               .Set(c => c.Text, text)
               .Set(c => c.Color, color)
               .Build();
    }

    public static void Icon(this RenderTreeBuilder builder, string icon, EventCallback<MouseEventArgs>? onClick = null)
    {
        if (string.IsNullOrWhiteSpace(icon))
            return;

        builder.Component<KIcon>()
               .Set(c => c.Icon, icon)
               .Set(c => c.OnClick, onClick)
               .Build();
    }

    public static void GroupBox(this RenderTreeBuilder builder, string title, Action child)
    {
        builder.Div("kui-group-box", () =>
        {
            builder.Label().Class("legend").Text(title).Close();
            builder.Div("body", child);
        });
    }
    #endregion

    #region Button
    public static void Button(this RenderTreeBuilder builder, ActionInfo action, EventCallback<MouseEventArgs> onClick, string style = null)
    {
        action.OnClick = onClick;
        action.Style = style;
        builder.Component<KButton>().Set(c => c.Action, action).Build();
    }

    public static void Button(this RenderTreeBuilder builder, string name, EventCallback<MouseEventArgs> onClick, string style = null)
    {
        var action = new ActionInfo { Name = name, OnClick = onClick, Style = style };
        builder.Component<KButton>().Set(c => c.Action, action).Build();
    }
    #endregion

    #region Toast
    public static async void Info(this IUIService service, string message) => await service.InfoAsync(message);
    public static Task InfoAsync(this IUIService service, string message) => service.Toast(message, StyleType.Info);
    public static async void Warning(this IUIService service, string message) => await service.WarningAsync(message);
    public static Task WarningAsync(this IUIService service, string message) => service.Toast(message, StyleType.Warning);
    public static async void Error(this IUIService service, string message) => await service.ErrorAsync(message);
    public static Task ErrorAsync(this IUIService service, string message) => service.Toast(message, StyleType.Error);
    public static async void Result(this IUIService service, Result result, Func<Task> onSuccess = null) => await service.ResultAsync(result, onSuccess);
    public static async Task ResultAsync(this IUIService service, Result result, Func<Task> onSuccess = null)
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