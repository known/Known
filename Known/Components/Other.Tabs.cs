using AntDesign;

namespace Known.Components;

/// <summary>
/// 多标签表单，扩展AntDesign的Tabs组件。
/// </summary>
public class KTabForm : Tabs
{
    /// <inheritdoc />
    protected override void OnInitialized()
    {
        base.OnInitialized();
        Animated = true;
        Class = "kui-tab-form";
    }
}