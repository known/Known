namespace Known.Blazor;

public class BasePicker<TItem> : BaseComponent where TItem : class, new()
{
    public BasePicker()
    {
        SelectedItems = [];
    }

    public virtual List<TItem> SelectedItems { get; }
    public string Title { get; set; }

    [Parameter] public double? Width { get; set; }
    [Parameter] public bool IsPick { get; set; }
    [Parameter] public bool IsMulti { get; set; }
    [Parameter] public bool AllowClear { get; set; }
    [Parameter] public string Value { get; set; }
    [Parameter] public Action<List<TItem>> ValueChanged { get; set; }

    protected override void BuildRender(RenderTreeBuilder builder)
    {
        if (IsPick)
        {
            BuildContent(builder);
            return;
        }

        BuildTextBox(builder);
        UI.BuildText(builder, new InputModel<string> { Value = Value, Disabled = true });

        if (!ReadOnly)
        {
            if (AllowClear)
                builder.Icon("fa fa-close kui-pick-clear", this.Callback<MouseEventArgs>(OnClear));
            builder.Icon("fa fa-ellipsis-h kui-pick", this.Callback<MouseEventArgs>(ShowModal));
        }
    }

    protected virtual void BuildTextBox(RenderTreeBuilder builder) { }
    protected virtual void BuildContent(RenderTreeBuilder builder) { }

    private void OnClear(MouseEventArgs args)
    {
        if (ReadOnly)
            return;

        Value = string.Empty;
        ValueChanged?.Invoke(null);
    }

    private void ShowModal(MouseEventArgs args)
    {
        if (ReadOnly)
            return;

        BasePicker<TItem> picker = null;
        var model = new DialogModel
        {
            Title = Title,
            Width = Width,
            Content = b =>
            {
                var parameters = new Dictionary<string, object> { { nameof(IsPick), true } };
                b.Component(this.GetType(), parameters, value => picker = (BasePicker<TItem>)value);
            }
        };
        model.OnOk = async () =>
        {
            var items = picker?.SelectedItems;
            if (items == null || items.Count == 0)
            {
                UI.Error(Language.SelectOneAtLeast);
                return;
            }

            ValueChanged?.Invoke(items);
            await model.CloseAsync();
        };
        UI.ShowDialog(model);
    }
}