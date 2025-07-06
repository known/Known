using AntDesign;

namespace Known.Components;

/// <summary>
/// 扩展Ant开关组件类。
/// </summary>
public class AntSwitch : Switch
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