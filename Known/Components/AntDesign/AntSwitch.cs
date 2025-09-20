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

    /// <summary>
    /// 取得或设置UI上下文对象级联值实例。
    /// </summary>
    [CascadingParameter] public UIContext Context { get; set; }

    /// <summary>
    /// 取得或设置显示文本，逗号分隔，前面为选中时显示，后面为未选中时显示。
    /// </summary>
    [Parameter] public string ShowTexts { get; set; }

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        if (AntForm != null)
            Disabled = AntForm.IsView;
        if (Item != null)
            Item.Type = typeof(bool);

        if (!string.IsNullOrWhiteSpace(ShowTexts))
        {
            var texts = ShowTexts.Split(',', '，');
            if (texts.Length > 0)
                CheckedChildren = Context?.Language?[texts[0].Trim()];
            if (texts.Length > 1)
                UnCheckedChildren = Context?.Language[texts[1].Trim()];
        }
        base.OnInitialized();
    }
}