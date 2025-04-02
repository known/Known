namespace Known.Blazor;

partial class FormModel<TItem>
{
    internal string Action { get; set; }

    /// <summary>
    /// 取得表单操作按钮信息列表，用于扩展表单底部按钮。
    /// </summary>
    public List<ActionInfo> Actions { get; } = [];

    /// <summary>
    /// 取得表单是否是新增表单，当Action为New时。
    /// </summary>
    public bool IsNew => Action == "New";

    /// <summary>
    /// 取得或设置表单保存确认提示框信息。
    /// </summary>
    public string ConfirmText { get; set; }

    /// <summary>
    /// 取得或设置表单保存确认提示框信息回调方法。
    /// </summary>
    public Func<string> OnConfirmText { get; set; }

    /// <summary>
    /// 取得或设置表单验证委托，当呈现抽象UI表单赋值。
    /// </summary>
    internal Func<bool> OnValidate { get; set; }

    /// <summary>
    /// 取得或设置表单对话框关闭委托，显示对话框时赋值。
    /// </summary>
    internal Func<Task> OnClose { get; set; }

    /// <summary>
    /// 取得或设置表单关闭操作时调用的委托。
    /// </summary>
    public Action OnClosed { get; set; }

    /// <summary>
    /// 验证表单字段。
    /// </summary>
    /// <returns>是否通过。</returns>
    public bool Validate()
    {
        if (OnValidate == null)
            return true;

        return OnValidate.Invoke();
    }

    /// <summary>
    /// 关闭表单对话框。
    /// </summary>
    /// <returns></returns>
    public async Task CloseAsync()
    {
        if (OnClose != null)
            await OnClose.Invoke();
        OnClosed?.Invoke();
    }

    /// <summary>
    /// 处理调用后端返回的结果信息。
    /// </summary>
    /// <param name="result">结果信息。</param>
    /// <param name="isClose">是否关闭弹窗，默认关闭。</param>
    /// <param name="isContinue">是否继续添加表单，默认否。</param>
    public void HandleResult(Result result, bool isClose = true, bool isContinue = false)
    {
        UI.Result(result, async () =>
        {
            var data = result.DataAs<TItem>();
            if (OnSaved != null)
                OnSaved?.Invoke(data);
            else if (OnSavedAsync != null)
                await OnSavedAsync.Invoke(data);

            if (isClose && result.IsClose)
                await CloseAsync();
            if (Table != null)
                await Table.PageRefreshAsync();
            else if (Page != null)
                await Page.RefreshAsync();
            if (isContinue)
                await LoadDataAsync(null);
        });
    }

    /// <summary>
    /// 添加操作列按钮。
    /// </summary>
    /// <param name="idOrName">按钮ID或名称。</param>
    /// <param name="onClick">点击事件委托。</param>
    public void AddAction(string idOrName, EventCallback<MouseEventArgs> onClick)
    {
        Actions?.Add(new ActionInfo(idOrName) { OnClick = onClick });
    }
}