namespace Known.Blazor;

partial class TableModel<TItem>
{
    private bool isShowNew = false;

    /// <summary>
    /// 显示新增表单对话框。
    /// </summary>
    /// <param name="onSave">新增保存方法委托。</param>
    /// <param name="row">新增默认对象。</param>
    public void NewForm(Func<TItem, Task<Result>> onSave, TItem row = null) => NewForm<TItem>(onSave, row);

    /// <summary>
    /// 显示新增表单对话框。
    /// </summary>
    /// <typeparam name="T">表单类型。</typeparam>
    /// <param name="onSave">新增保存方法委托。</param>
    /// <param name="row">新增默认对象。</param>
    public void NewForm<T>(Func<T, Task<Result>> onSave, T row = null) where T : class, new()
    {
        if (isShowNew)
            return;

        isShowNew = true;
        var model = new FormModel<T>(this, IsAuto) { Action = Language.New, DefaultData = row, OnSave = onSave };
        model.LoadDefaultData();
        var isShow = ShowForm(model);
        if (isShow)
            isShowNew = false;
    }

    /// <summary>
    /// 显示带有附件的新增表单对话框。
    /// </summary>
    /// <param name="onSave">新增保存方法委托。</param>
    /// <param name="row">新增默认对象。</param>
    public void NewForm(Func<UploadInfo<TItem>, Task<Result>> onSave, TItem row = null) => NewForm<TItem>(onSave, row);

    /// <summary>
    /// 显示带有附件的新增表单对话框。
    /// </summary>
    /// <typeparam name="T">表单类型。</typeparam>
    /// <param name="onSave">新增保存方法委托。</param>
    /// <param name="row">新增默认对象。</param>
    public void NewForm<T>(Func<UploadInfo<T>, Task<Result>> onSave, T row = null) where T : class, new()
    {
        if (isShowNew)
            return;

        isShowNew = true;
        var model = new FormModel<T>(this, IsAuto) { Action = Language.New, DefaultData = row, OnSaveFile = onSave };
        model.LoadDefaultData();
        var isShow = ShowForm(model);
        if (isShow)
            isShowNew = false;
    }

    /// <summary>
    /// 显示新增表单对话框。
    /// </summary>
    /// <param name="onSave">新增保存方法委托。</param>
    /// <param name="row">异步请求默认对象委托。</param>
    public Task NewFormAsync(Func<TItem, Task<Result>> onSave, Func<Task<TItem>> row) => NewFormAsync<TItem>(onSave, row);

    /// <summary>
    /// 显示新增表单对话框。
    /// </summary>
    /// <typeparam name="T">表单类型。</typeparam>
    /// <param name="onSave">新增保存方法委托。</param>
    /// <param name="row">异步请求默认对象委托。</param>
    public async Task NewFormAsync<T>(Func<T, Task<Result>> onSave, Func<Task<T>> row) where T : class, new()
    {
        if (isShowNew)
            return;

        isShowNew = true;
        var model = new FormModel<T>(this, IsAuto) { Action = Language.New, DefaultDataAction = row, OnSave = onSave };
        await model.LoadDefaultDataAsync();
        var isShow = ShowForm(model);
        if (isShow)
            isShowNew = false;
    }

    /// <summary>
    /// 显示带有附件的新增表单对话框。
    /// </summary>
    /// <param name="onSave">新增保存方法委托。</param>
    /// <param name="row">异步请求默认对象委托。</param>
    public Task NewFormAsync(Func<UploadInfo<TItem>, Task<Result>> onSave, Func<Task<TItem>> row) => NewFormAsync<TItem>(onSave, row);

    /// <summary>
    /// 显示带有附件的新增表单对话框。
    /// </summary>
    /// <typeparam name="T">表单类型。</typeparam>
    /// <param name="onSave">新增保存方法委托。</param>
    /// <param name="row">异步请求默认对象委托。</param>
    public async Task NewFormAsync<T>(Func<UploadInfo<T>, Task<Result>> onSave, Func<Task<T>> row) where T : class, new()
    {
        if (isShowNew)
            return;

        isShowNew = true;
        var model = new FormModel<T>(this, IsAuto) { Action = Language.New, DefaultDataAction = row, OnSaveFile = onSave };
        await model.LoadDefaultDataAsync();
        var isShow = ShowForm(model);
        if (isShow)
            isShowNew = false;
    }
}