namespace Known.Blazor;

partial class TableModel<TItem>
{
    /// <summary>
    /// 异步弹窗显示导入表单。
    /// </summary>
    /// <param name="param">与后端对应的导入参数。</param>
    /// <returns></returns>
    public Task ShowImportAsync(string param = null)
    {
        var info = new ImportInfo
        {
            PageId = Context.Current.Id,
            PageName = PageName,
            EntityType = typeof(TItem),
            IsDictionary = IsDictionary,
            Param = param
        };
        var model = new DialogModel { Title = Language.GetImportTitle(PageName) };
        info.OnSuccess = async () =>
        {
            await model.CloseAsync();
            await RefreshAsync();
        };
        model.Content = builder => UIConfig.ImportForm?.Invoke(builder, info);
        UI.ShowDialog(model);
        return Task.CompletedTask;
    }
}