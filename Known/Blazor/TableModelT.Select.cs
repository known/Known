namespace Known.Blazor;

partial class TableModel<TItem>
{
    /// <summary>
    /// 取得或设置表格选中行绑定的数据列表。
    /// </summary>
    public IEnumerable<TItem> SelectedRows { get; set; }

    /// <summary>
    /// 选择表格一行数据操作。
    /// </summary>
    /// <param name="action">操作方法委托。</param>
    public void SelectRow(Action<TItem> action)
    {
        if (SelectedRows == null)
        {
            UI.Warning(Language?.SelectOne);
            return;
        }

        var rows = SelectedRows.ToList();
        if (rows.Count == 0 || rows.Count > 1)
        {
            UI.Warning(Language?.SelectOne);
            return;
        }

        action?.Invoke(rows[0]);
    }

    /// <summary>
    /// 选择表格一行数据，带确认对话框的操作。
    /// </summary>
    /// <param name="action">操作方法委托。</param>
    /// <param name="confirmText">确认对话框文本。</param>
    public void SelectRow(Func<TItem, Task<Result>> action, string confirmText = null)
    {
        SelectRow(async row =>
        {
            if (!string.IsNullOrWhiteSpace(confirmText))
            {
                UI.Confirm(GetConfirmText(confirmText), async () =>
                {
                    var result = await action?.Invoke(row);
                    UI.Result(result, PageRefreshAsync);
                });
            }
            else
            {
                var result = await action?.Invoke(row);
                UI.Result(result, PageRefreshAsync);
            }
        });
    }

    /// <summary>
    /// 选择表格多行数据操作。
    /// </summary>
    /// <param name="action">操作方法委托。</param>
    public void SelectRows(Action<List<TItem>> action)
    {
        if (SelectedRows == null)
        {
            UI.Warning(Language?.SelectOneAtLeast);
            return;
        }

        var rows = SelectedRows.ToList();
        if (rows.Count == 0)
        {
            UI.Warning(Language?.SelectOneAtLeast);
            return;
        }

        action?.Invoke(rows);
    }

    /// <summary>
    /// 选择表格多行数据，带确认对话框的操作。
    /// </summary>
    /// <param name="action">操作方法委托。</param>
    /// <param name="confirmText">确认对话框文本。</param>
    public void SelectRows(Func<List<TItem>, Task<Result>> action, string confirmText = null)
    {
        SelectRows(async rows =>
        {
            if (!string.IsNullOrWhiteSpace(confirmText))
            {
                UI.Confirm(GetConfirmText(confirmText), async () =>
                {
                    var result = await action?.Invoke(rows);
                    UI.Result(result, PageRefreshAsync);
                });
            }
            else
            {
                var result = await action?.Invoke(rows);
                UI.Result(result, PageRefreshAsync);
            }
        });
    }

    private string GetConfirmText(string text)
    {
        text = Language.GetText("Button", text);
        return Language?["Tip.ConfirmRecordName"]?.Replace("{text}", text);
    }
}