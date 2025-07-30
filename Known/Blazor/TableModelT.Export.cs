namespace Known.Blazor;

partial class TableModel<TItem>
{
    /// <summary>
    /// 根据导出模式异步导出表格数据，默认按查询结果导出。
    /// </summary>
    /// <param name="mode">导出模式（单页，查询结果，全部）。</param>
    /// <returns></returns>
    public Task ExportDataAsync(ExportMode mode = ExportMode.Query)
    {
        return ExportDataAsync(Name, mode);
    }

    /// <summary>
    /// 异步导出表格数据，默认按查询结果导出。
    /// </summary>
    /// <param name="name">导出文件名。</param>
    /// <param name="mode">导出模式（单页，查询结果，全部）。</param>
    /// <returns></returns>
    public Task ExportDataAsync(string name, ExportMode mode = ExportMode.Query)
    {
        if (mode != ExportMode.Select)
            return Page?.App?.ExportDataAsync(this, name, mode);

        SelectRows(async rows =>
        {
            Criteria.Parameters[nameof(ExportMode.Select)] = rows;
            await Page?.App?.ExportDataAsync(this, name, mode);
        });
        return Task.CompletedTask;
    }
}