using AntDesign;

namespace Known.Components;

/// <summary>
/// 扩展Ant文本输入框组件类。
/// </summary>
public class AntInput : Input<string>
{
    [CascadingParameter] private DataItem Item { get; set; }

    /// <summary>
    /// 取得或设置表单容器对象。
    /// </summary>
    [CascadingParameter] protected IComContainer AntForm { get; set; }

    /// <summary>
    /// 取得或设置前缀图标。
    /// </summary>
    [Parameter] public string Icon { get; set; }

    /// <summary>
    /// 取得或设置回车键按下时的回调函数。
    /// </summary>
    [Parameter] public EventCallback<string> OnEnter { get; set; }

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        if (AntForm != null)
            Disabled = AntForm.IsView;
        if (Item != null)
            Item.Type = typeof(string);
        if (!string.IsNullOrWhiteSpace(Icon))
            Prefix = b => b.Icon(Icon);
        if (OnEnter.HasDelegate)
        {
            OnKeyUp = this.Callback<KeyboardEventArgs>(e =>
            {
                if (e.Key == "Enter")
                {
                    OnEnter.InvokeAsync(Value);
                }
            });
        }
        base.OnInitialized();
    }
}