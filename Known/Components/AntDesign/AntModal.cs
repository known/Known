using AntDesign;

namespace Known.Components;

/// <summary>
/// 扩展Ant模态对话框组件类。
/// </summary>
public class AntModal : Modal
{
    /// <inheritdoc />
    protected override void OnInitialized()
    {
        base.OnInitialized();
        MaskClosable = false;
        Draggable = true;
    }
}