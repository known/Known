namespace Known.Components;

/// <summary>
/// 扩展Ant数值框组件类。
/// </summary>
/// <typeparam name="TValue"></typeparam>
public class AntNumber<TValue> : AntDesign.InputNumber<TValue>
{
    [CascadingParameter] private IAntForm AntForm { get; set; }
    [CascadingParameter] private DataItem Item { get; set; }

    /// <summary>
    /// 初始化组件。
    /// </summary>
    protected override void OnInitialized()
    {
        if (AntForm != null)
            Disabled = AntForm.IsView;
        if (Item != null)
            Item.Type = typeof(TValue);
        base.OnInitialized();
    }
}

/// <summary>
/// 扩展Ant整数输入组件类。
/// </summary>
public class AntInteger : AntDesign.InputNumber<int?>
{
    [CascadingParameter] private IAntForm AntForm { get; set; }
    [CascadingParameter] private DataItem Item { get; set; }

    /// <summary>
    /// 初始化组件。
    /// </summary>
    protected override void OnInitialized()
    {
        if (AntForm != null)
            Disabled = AntForm.IsView;
        if (Item != null)
            Item.Type = typeof(int?);
        base.OnInitialized();
    }
}

/// <summary>
/// 扩展Ant小数输入框组件类。
/// </summary>
public class AntDecimal : AntDesign.InputNumber<decimal?>
{
    [CascadingParameter] private IAntForm AntForm { get; set; }
    [CascadingParameter] private DataItem Item { get; set; }

    /// <summary>
    /// 初始化组件。
    /// </summary>
    protected override void OnInitialized()
    {
        if (AntForm != null)
            Disabled = AntForm.IsView;
        if (Item != null)
            Item.Type = typeof(decimal?);
        base.OnInitialized();
    }
}