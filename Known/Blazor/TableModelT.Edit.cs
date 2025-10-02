namespace Known.Blazor;

partial class TableModel<TItem>
{
    private bool isShowEdit = false;

    /// <summary>
    /// 显示编辑表单对话框。
    /// </summary>
    /// <param name="onSave">编辑保存方法委托。</param>
    /// <param name="row">编辑行绑定的对象。</param>
    public void EditForm(Func<TItem, Task<Result>> onSave, TItem row) => EditForm<TItem>(onSave, row);

    /// <summary>
    /// 显示编辑表单对话框。
    /// </summary>
    /// <typeparam name="T">表单类型。</typeparam>
    /// <param name="onSave">编辑保存方法委托。</param>
    /// <param name="row">编辑行绑定的对象。</param>
    public void EditForm<T>(Func<T, Task<Result>> onSave, T row) where T : class, new() => EditForm(onSave, row, Language.Edit);

    /// <summary>
    /// 显示编辑表单对话框。
    /// </summary>
    /// <param name="onSave">编辑保存方法委托。</param>
    /// <param name="row">编辑行绑定的对象。</param>
    /// <param name="action">操作名称。</param>
    public void EditForm(Func<TItem, Task<Result>> onSave, TItem row, string action) => EditForm<TItem>(onSave, row, action);

    /// <summary>
    /// 显示编辑表单对话框。
    /// </summary>
    /// <typeparam name="T">表单类型。</typeparam>
    /// <param name="onSave">编辑保存方法委托。</param>
    /// <param name="row">编辑行绑定的对象。</param>
    /// <param name="action">操作名称。</param>
    public void EditForm<T>(Func<T, Task<Result>> onSave, T row, string action) where T : class, new()
    {
        if (isShowEdit)
            return;

        isShowEdit = true;
        var isShow = ShowForm(new FormModel<T>(this, IsAuto) { Action = action, Data = row, OnSave = onSave });
        if (isShow)
            isShowEdit = false;
    }

    /// <summary>
    /// 显示带有附件的编辑表单对话框。
    /// </summary>
    /// <param name="onSave">编辑保存方法委托。</param>
    /// <param name="row">编辑行绑定的对象。</param>
    public void EditForm(Func<UploadInfo<TItem>, Task<Result>> onSave, TItem row) => EditForm<TItem>(onSave, row, Language.Edit);

    /// <summary>
    /// 显示带有附件的编辑表单对话框。
    /// </summary>
    /// <typeparam name="T">表单类型。</typeparam>
    /// <param name="onSave">编辑保存方法委托。</param>
    /// <param name="row">编辑行绑定的对象。</param>
    public void EditForm<T>(Func<UploadInfo<T>, Task<Result>> onSave, T row) where T : class, new() => EditForm(onSave, row, Language.Edit);

    /// <summary>
    /// 显示带有附件的编辑表单对话框。
    /// </summary>
    /// <param name="onSave">编辑保存方法委托。</param>
    /// <param name="row">编辑行绑定的对象。</param>
    /// <param name="action">操作名称。</param>
    public void EditForm(Func<UploadInfo<TItem>, Task<Result>> onSave, TItem row, string action) => EditForm<TItem>(onSave, row, action);

    /// <summary>
    /// 显示带有附件的编辑表单对话框。
    /// </summary>
    /// <typeparam name="T">表单类型。</typeparam>
    /// <param name="onSave">编辑保存方法委托。</param>
    /// <param name="row">编辑行绑定的对象。</param>
    /// <param name="action">操作名称。</param>
    public void EditForm<T>(Func<UploadInfo<T>, Task<Result>> onSave, T row, string action) where T : class, new()
    {
        if (isShowEdit)
            return;

        isShowEdit = true;
        var isShow = ShowForm(new FormModel<T>(this, IsAuto) { Action = action, Data = row, OnSaveFile = onSave });
        if (isShow)
            isShowEdit = false;
    }
}