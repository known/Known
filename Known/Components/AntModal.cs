using AntDesign;

namespace Known.Components;

/// <summary>
/// 扩展Ant模态对话框组件类。
/// </summary>
public class AntModal : Modal
{
    /// <summary>
    /// 初始化组件。
    /// </summary>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        MaskClosable = false;
        Draggable = true;
    }
}