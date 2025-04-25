namespace Known.Components;

/// <summary>
/// 编辑输入框组件类。
/// </summary>
public partial class KEditInput
{
    private bool isEdit;

    /// <summary>
    /// 取得或设置文本值。
    /// </summary>
    [Parameter] public string Value { get; set; }

    /// <summary>
    /// 取得或设置保存文本委托方法。
    /// </summary>
    [Parameter] public Func<string, Task> OnSave { get; set; }

    private void OnValueChanged(string value)
    {
        Value = value;
    }

    private void OnSaveClick()
    {
        OnSave?.Invoke(Value);
        isEdit = false;
    }
}