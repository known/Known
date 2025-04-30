namespace Known.Blazor;

partial class TableModel<TItem>
{
    private bool isShowEdit = false;

    /// <summary>
    /// 显示编辑表单对话框。
    /// </summary>
    /// <param name="onSave">编辑保存方法委托。</param>
    /// <param name="row">编辑行绑定的对象。</param>
    public void EditForm(Func<TItem, Task<Result>> onSave, TItem row)
    {
        if (isShowEdit)
            return;

        isShowEdit = true;
        var isShow = ShowForm(new FormModel<TItem>(this, IsAuto)
        {
            Action = Language.Edit,
            Data = row,
            OnSave = onSave
        });
        if (isShow)
            isShowEdit = false;
    }

    /// <summary>
    /// 显示带有附件的编辑表单对话框。
    /// </summary>
    /// <param name="onSave">编辑保存方法委托。</param>
    /// <param name="row">编辑行绑定的对象。</param>
    public void EditForm(Func<UploadInfo<TItem>, Task<Result>> onSave, TItem row)
    {
        if (isShowEdit)
            return;

        isShowEdit = true;
        var isShow = ShowForm(new FormModel<TItem>(this, IsAuto)
        {
            Action = Language.Edit,
            Data = row,
            OnSaveFile = onSave
        });
        if (isShow)
            isShowEdit = false;
    }
}