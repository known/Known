namespace Known.Internals;

class TreePicker : BasePicker<MenuInfo>
{
    private TreeModel Model { get; set; }

    [Parameter] public List<MenuInfo> Items { get; set; }
    [Parameter] public Action<MenuInfo> OnChanged { get; set; }

    protected override Dictionary<string, object> GetPickParameters()
    {
        var parameters = base.GetPickParameters();
        parameters[nameof(Items)] = Items;
        return parameters;
    }

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

    protected override void BuildContent(RenderTreeBuilder builder) => builder.Tree(Model);

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