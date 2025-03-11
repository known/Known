namespace Known.Blazor;

partial class TableModel<TItem>
{
    private bool isShowView = false;

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
            Action = $"{type}",
            Data = row
        });
        if (isShow)
            isShowView = false;
    }
}