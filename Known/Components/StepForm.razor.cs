﻿namespace Known.Components;

/// <summary>
/// 步骤表单组件类。
/// </summary>
public partial class StepForm
{
    /// <summary>
    /// 取得或设置步骤表单配置模型。
    /// </summary>
    [Parameter] public StepModel Model { get; set; }

    /// <summary>
    /// 取得或设置步骤表单是否是查看模式。
    /// </summary>
    [Parameter] public bool IsView { get; set; }

    /// <summary>
    /// 取得或设置步骤表单是否每一步都保存数据到后台。
    /// </summary>
    [Parameter] public bool IsStepSave { get; set; }

    /// <summary>
    /// 取得或设置步骤表单总步长。
    /// </summary>
    [Parameter] public int? StepCount { get; set; }

    /// <summary>
    /// 取得或设置步骤表单保存数据委托。
    /// </summary>
    [Parameter] public Func<bool, Task<bool>> OnSave { get; set; }

    /// <inheritdoc />
    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        StepCount ??= Model.Items.Count;
    }

    /// <summary>
    /// 设置步骤表单步长。
    /// </summary>
    /// <param name="count">步长。</param>
    public void SetStepCount(int count)
    {
        StepCount = count;
        StateChanged();
    }

    private async Task OnPrevClickAsync(MouseEventArgs arg)
    {
        if (IsStepSave)
        {
            if (!await SaveAsync())
                return;
        }

        Model.Current--;
    }

    private async Task OnNextClickAsync(MouseEventArgs arg)
    {
        if (IsStepSave)
        {
            if (!await SaveAsync())
                return;
        }

        Model.Current++;
    }

    private async Task OnCompleteAsync(MouseEventArgs arg)
    {
        await SaveAsync(true);
    }

    private async Task<bool> SaveAsync(bool isComplete = false)
    {
        if (IsView)
            return true;

        return await OnSave?.Invoke(isComplete);
    }
}