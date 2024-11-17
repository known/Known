namespace Known.Blazor;

partial class TableModel<TItem>
{
    /// <summary>
    /// 异步弹窗显示导入表单。
    /// </summary>
    /// <param name="param">与后端对应的导入参数。</param>
    /// <returns></returns>
    public async Task ShowImportAsync(string param = null)
    {
        var type = typeof(TItem);
        var id = $"{type.Name}Import";
        if (!string.IsNullOrWhiteSpace(param))
            id += $"_{param}";
        if (IsDictionary)
            id += $"_{Context.Current.Id}";
        var importTitle = Language.GetImportTitle(PageName);
        var info = await Page?.Data?.GetImportAsync(id);
        info.Name = PageName;
        info.BizName = importTitle;
        var model = new DialogModel { Title = importTitle };
        model.Content = builder =>
        {
            builder.Component<Importer>()
                   .Set(c => c.Model, info)
                   .Set(c => c.OnSuccess, async () =>
                   {
                       await model.CloseAsync();
                       await RefreshAsync();
                   })
                   .Build();
        };
        UI.ShowDialog(model);
    }
}