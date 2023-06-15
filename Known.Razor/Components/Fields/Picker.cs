namespace Known.Razor.Components.Fields;

public class Picker : Field
{
    private bool isInitOK;

    [Parameter] public IPicker Pick { get; set; }
    [Parameter] public Action<object> OnPicked { get; set; }
    [Parameter] public bool CanEdit { get; set; }

    protected override Task OnInitializedAsync()
    {
        isInitOK = Pick != null && Pick.OnPicked != null;
        return base.OnInitializedAsync();
    }

    protected override void BuildInput(RenderTreeBuilder builder)
    {
        builder.Input(attr =>
        {
            attr.Type("text").Id(Id).Name(Id).Disabled(!CanEdit)
                .Value(Value).Required(Required)
                .Add("autocomplete", "off")
                .OnChange(CreateBinder());
        });

        if (!Enabled || Pick == null)
            return;

        builder.Icon("fa fa-ellipsis-h", attr =>
        {
            attr.OnClick(Callback(e =>
            {
                if (OnPicked != null)
                    Pick.OnPicked = OnFieldPicked;
                else if (!isInitOK)
                    Pick.OnPicked = OnFieldChanged;
                UI.Show(Pick);
            }));
        });
    }

    private void OnFieldPicked(object value)
    {
        OnPicked?.Invoke(value);
        SetValue(value);
        OnValueChange();
    }

    private void OnFieldChanged(object value)
    {
        SetValue(value);
        OnValueChange();
    }
}