﻿namespace Known.Components;

/// <summary>
/// 布局组件类。
/// </summary>
public partial class KLayout
{
    /// <summary>
    /// 取得或设置子组件内容。
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// 取得或设置加载按钮委托。
    /// </summary>
    [Parameter] public Action<List<MenuInfo>> OnLoadMenus { get; set; }

    /// <summary>
    /// 取得或设置重新加载页面委托。
    /// </summary>
    [Parameter] public Action OnReloadPage { get; set; }

    /// <inheritdoc />
    public override async Task ShowSpinAsync(string text, Func<Task> action)
    {
        if (action == null)
            return;

        await JS.ShowSpinAsync(Language[text]);
        await Task.Run(async () =>
        {
            try
            {
                await action?.Invoke();
            }
            catch (Exception ex)
            {
                await OnErrorAsync(ex);
            }
            await JS.HideSpinAsync();
        });
    }

    /// <inheritdoc />
    public override void ReloadPage()
    {
        OnReloadPage?.Invoke();
    }

    /// <summary>
    /// 显示设置表单。
    /// </summary>
    public void ShowSetting()
    {
        UI.ShowDrawer(new DrawerModel
        {
            Title = Language.SystemSetting,
            Width = "340",
            Content = b => b.Component<SettingForm>()
                            .Set(c => c.OnSave, OnSaveSetting)
                            .Set(c => c.OnReset, OnResetSetting)
                            .Set(c => c.OnChange, StateChanged)
                            .Build()
        });
    }

    internal override void LoadMenus()
    {
        OnLoadMenus?.Invoke(UserMenus);
    }

    private async Task OnSaveSetting()
    {
        var result = await Admin.SaveUserSettingAsync(Context.UserSetting);
        if (result.IsValid)
            await OnThemeColorAsync();
    }

    private async Task OnResetSetting()
    {
        var result = await Admin.ResetUserSettingAsync();
        if (result.IsValid)
        {
            Context.UserSetting = result.DataAs<UserSettingInfo>();
            await OnThemeColorAsync();
        }
    }
}