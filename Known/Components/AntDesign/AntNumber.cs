namespace Known.Components;

/// <summary>
/// 扩展Ant数值框组件类。
/// </summary>
/// <typeparam name="TValue"></typeparam>
public class AntNumber<TValue> : AntDesign.InputNumber<TValue>
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
            Item.Type = typeof(TValue);
        base.OnInitialized();
    }
}

/// <summary>
/// 扩展Ant整数输入组件类。
/// </summary>
public class AntInteger : AntDesign.InputNumber<int?>
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
            Item.Type = typeof(int?);
        base.OnInitialized();
    }
}

/// <summary>
/// 扩展Ant小数输入框组件类。
/// </summary>
public class AntDecimal : AntDesign.InputNumber<decimal?>
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
            Item.Type = typeof(decimal?);
        base.OnInitialized();
    }
}