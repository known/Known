namespace Known.Blazor;

partial class TableModel<TItem>
{
    private bool isShowView = false;

    /// <summary>
    /// 取得或设置表格左下角内容模板。
    /// </summary>
    public RenderFragment BottomLeft { get; set; }

    internal Func<Task> OnBottomChanged { get; set; }

    /// <summary>
    /// 刷新表格底部组件。
    /// </summary>
    /// <returns></returns>
    public Task RefreshBottomAsync()
    {
        if (OnBottomChanged == null)
            return Task.CompletedTask;

        return OnBottomChanged.Invoke();
    }

    /// <summary>
    /// 显示查看表单对话框。
    /// </summary>
    /// <param name="row">查看行绑定的对象。</param>
    public void ViewForm(TItem row) => ViewForm(FormViewType.View, row);

    internal void ViewForm(FormViewType type, TItem row)
    {
        if (isShowView)
            return;

        isShowView = true;
        var isShow = ShowForm(new FormModel<TItem>(this, IsAuto)
        {
            ViewType = type,
            IsView = true,
            Action = type.GetDescription(),
            Data = row
        });
        if (isShow)
            isShowView = false;
    }
}