namespace Known.Components;

/// <summary>
/// 树选择器组件类。
/// </summary>
public class TreePicker : BasePicker<MenuInfo>
{
    private TreeModel Model { get; set; }

    /// <summary>
    /// 取得或设置树数据源。
    /// </summary>
    [Parameter] public List<MenuInfo> Items { get; set; }

    /// <summary>
    /// 取得或设置选择节点事件委托。
    /// </summary>
    [Parameter] public Action<MenuInfo> OnChanged { get; set; }

    /// <inheritdoc />
    protected override Dictionary<string, object> GetPickParameters()
    {
        var parameters = base.GetPickParameters();
        parameters[nameof(Items)] = Items;
        return parameters;
    }

    /// <inheritdoc />
    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        AllowClear = true;
        Model = new TreeModel
        {
            ExpandRoot = true,
            Data = Items,
            OnNodeClick = n =>
            {
                SelectedItems.Clear();
                SelectedItems.Add(n);
                return Task.CompletedTask;
            }
        };
    }

    /// <inheritdoc />
    protected override void BuildContent(RenderTreeBuilder builder)
    {
        builder.Div().Style("padding:10px 0;").Child(() => builder.Tree(Model));
    }

    /// <inheritdoc />
    protected override void OnValueChanged(List<MenuInfo> items)
    {
        var item = items?.FirstOrDefault();
        Text = item?.Name;
        Value = item?.Id;
        if (ValueChanged.HasDelegate)
            ValueChanged.InvokeAsync(Value);
        OnChanged?.Invoke(item);
    }
}