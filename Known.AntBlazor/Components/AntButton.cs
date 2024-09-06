namespace Known.AntBlazor.Components;

/// <summary>
/// 自定义Ant按钮组件类，可自动提示加载。
/// </summary>
public class AntButton : BaseComponent
{
    private bool isLoad;

    /// <summary>
    /// 取得或设置是否是块级按钮。
    /// </summary>
    [Parameter] public bool Block { get; set; }

    /// <summary>
    /// 取得或设置按钮图标。
    /// </summary>
    [Parameter] public string Icon { get; set; }

    /// <summary>
    /// 取得或设置按钮类型（如：ButtonType.Primary）。
    /// </summary>
    [Parameter] public string Type { get; set; }

    /// <summary>
    /// 取得或设置按钮CSS类名。
    /// </summary>
    [Parameter] public string Class { get; set; }

    /// <summary>
    /// 取得或设置按钮单击事件方法。
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs> OnClick { get; set; }

    /// <summary>
    /// 呈现按钮组件。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    protected override void BuildRender(RenderTreeBuilder builder)
    {
        var isDanger = Type == "danger";
        if (isDanger)
            Type = ButtonType.Primary;
        builder.Component<Button>()
               .Set(c => c.Block, Block)
               .Set(c => c.Icon, Icon)
               .Set(c => c.Type, Type)
               .Set(c => c.Class, Class)
               .Set(c => c.Danger, isDanger)
               .Set(c => c.Disabled, !Enabled)
               .Set(c => c.Loading, isLoad)
               .Set(c => c.OnClick, this.Callback<MouseEventArgs>(OnButtonClick))
               .Set(c => c.ChildContent, b => b.Text(Name))
               .Build();
    }

    private async void OnButtonClick(MouseEventArgs args)
    {
        if (isLoad || !OnClick.HasDelegate)
            return;

        isLoad = true;
        await OnClick.InvokeAsync(args);
        isLoad = false;
    }
}