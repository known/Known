namespace Known.Extensions;

/// <summary>
/// 布局方法扩展类。
/// </summary>
public static class LayoutExtension
{
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
}