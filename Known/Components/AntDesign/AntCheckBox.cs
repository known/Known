using AntDesign;

namespace Known.Components;

/// <summary>
/// 扩展Ant文本输入框组件类。
/// </summary>
public class AntCheckBox : Checkbox
{
    [CascadingParameter] private DataItem Item { get; set; }

    /// <summary>
    /// 取得或设置表单容器对象。
    /// </summary>
    [CascadingParameter] protected IComContainer AntForm { get; set; }

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        if (AntForm != null)
            Disabled = AntForm.IsView;
        if (Item != null)
            Item.Type = typeof(bool);
        base.OnInitialized();
    }
}