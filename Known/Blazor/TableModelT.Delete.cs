namespace Known.Blazor;

partial class TableModel<TItem>
{
    /// <summary>
    /// 批量删除表格数据。
    /// </summary>
    /// <param name="action">删除方法委托。</param>
    public void DeleteM(Func<List<TItem>, Task<Result>> action) => SelectRows(action, "Delete");

    /// <summary>
    /// 删除表格一行数据。
    /// </summary>
    /// <param name="action">删除一条数据方法委托。</param>
    /// <param name="row">删除行绑定的对象。</param>
    public void Delete(Func<TItem, Task<Result>> action, TItem row)
    {
        UI.Confirm(Language?["Tip.ConfirmDeleteRecord"], async () =>
        {
            var result = await action?.Invoke(row);
            UI.Result(result, PageRefreshAsync);
        });
    }

    /// <summary>
    /// 删除表格一行数据。
    /// </summary>
    /// <param name="action">删除多条数据方法委托。</param>
    /// <param name="row">删除行绑定的对象。</param>
    public void Delete(Func<List<TItem>, Task<Result>> action, TItem row)
    {
        UI.Confirm(Language?["Tip.ConfirmDeleteRecord"], async () =>
        {
            var result = await action?.Invoke([row]);
            UI.Result(result, PageRefreshAsync);
        });
    }
}