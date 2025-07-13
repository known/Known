using AntDesign;

namespace Known.Components;

/// <summary>
/// 扩展Ant模态对话框组件类。
/// </summary>
public class AntModal : Modal
{
    /// <summary>
    /// 取得或设置UI上下文对象级联值实例。
    /// </summary>
    [CascadingParameter] public UIContext Context { get; set; }

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        base.OnInitialized();
        MaskClosable = false;
        Draggable = true;
    }
}