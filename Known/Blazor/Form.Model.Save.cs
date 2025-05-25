namespace Known.Blazor;

partial class FormModel<TItem>
{
    /// <summary>
    /// 取得或设置附件表单保存时调用的委托。
    /// </summary>
    public Func<UploadInfo<TItem>, Task<Result>> OnSaveFile { get; set; }

    /// <summary>
    /// 取得或设置表单保存时调用的委托。
    /// </summary>
    public Func<TItem, Task<Result>> OnSave { get; set; }

    /// <summary>
    /// 取得或设置表单保存前调用的委托。
    /// </summary>
    public Func<TItem, Task<bool>> OnSaving { get; set; }

    /// <summary>
    /// 取得或设置表单保存后调用的委托。
    /// </summary>
    public Action<TItem> OnSaved { get; set; }

    /// <summary>
    /// 取得或设置表单保存后调用的委托。
    /// </summary>
    public Func<TItem, Task> OnSavedAsync { get; set; }

    /// <summary>
    /// 异步保存表单数据。
    /// </summary>
    /// <param name="onSaved">保存后委托。</param>
    /// <param name="isClose">是否关闭对话框，默认是。</param>
    /// <returns></returns>
    public Task SaveAsync(Action<TItem> onSaved, bool isClose = true)
    {
        OnSaved = onSaved;
        return SaveAsync(isClose);
    }

    /// <summary>
    /// 异步保存表单数据。
    /// </summary>
    /// <param name="onSavedAsync ">异步保存后委托。</param>
    /// <param name="isClose">是否关闭对话框，默认是。</param>
    /// <returns></returns>
    public Task SaveAsync(Func<TItem, Task> onSavedAsync, bool isClose = true)
    {
        OnSavedAsync = onSavedAsync;
        return SaveAsync(isClose);
    }

    /// <summary>
    /// 保存表单数据。
    /// </summary>
    /// <param name="isClose">是否关闭对话框，默认是。</param>
    /// <returns></returns>
    public Task SaveAsync(bool isClose = true) => OnSaveAsync(isClose, false);

    /// <summary>
    /// 保存表单数据继续添加新数据。
    /// </summary>
    /// <returns></returns>
    public Task SaveContinueAsync() => OnSaveAsync(false, true);

    /// <summary>
    /// 保存表单数据。
    /// </summary>
    /// <param name="isClose">是否关闭对话框。</param>
    /// <param name="isContinue">是否继续添加新数据。</param>
    /// <returns></returns>
    public async Task OnSaveAsync(bool isClose, bool isContinue)
    {
        if (!Validate())
            return;

        if (OnSaving != null)
        {
            if (!await OnSaving.Invoke(Data))
                return;
        }

        var confirmText = ConfirmText;
        if (string.IsNullOrWhiteSpace(confirmText))
            confirmText = OnConfirmText?.Invoke();

        if (string.IsNullOrWhiteSpace(confirmText))
        {
            await OnSaveDataAsync(isClose, isContinue);
            return;
        }

        UI.Confirm(confirmText, async () => await OnSaveDataAsync(isClose, isContinue));
    }

    private async Task OnSaveDataAsync(bool isClose, bool isContinue)
    {
        var result = Result.Error("No save action.");
        try
        {
            if (OnSaveFile != null)
            {
                var info = new UploadInfo<TItem>(Data);
                foreach (var file in Files)
                {
                    info.Files[file.Key] = file.Value;
                }
                result = await OnSaveFile.Invoke(info);
            }
            else if (OnSave != null)
            {
                result = await OnSave.Invoke(Data);
            }
        }
        catch (Exception ex)
        {
            Logger.Exception(LogTarget.FrontEnd, Context.CurrentUser, ex);
            result = Result.Error(ex.Message);
        }
        HandleResult(result, isClose, isContinue);
    }
}