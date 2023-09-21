namespace Known.Razor.Components.Fields;

public class Picker : Field
{
    private bool isInitOK;

    [Parameter] public IPicker Pick { get; set; }
    [Parameter] public string TextField { get; set; }
    [Parameter] public Action<object> OnPicked { get; set; }
    [Parameter] public bool CanEdit { get; set; }

    protected override Task OnInitializedAsync()
    {
        isInitOK = Pick != null && Pick.OnPicked != null;
        return base.OnInitializedAsync();
    }

    protected override void BuildInput(RenderTreeBuilder builder)
    {
        if (IsInput)
        {
            var sb = StyleBuilder.Default.Width(Width).Height(Height).Build();
            var className = CssBuilder.Default("form-input")
                                      .AddClass("readonly", IsReadOnly)
                                      .Build();
            builder.Div(className, attr =>
            {
                attr.Style(sb);
                BuildInputPicker(builder);
            });
        }
        else
        {
            BuildInputPicker(builder);
        }
    }

    private void BuildInputPicker(RenderTreeBuilder builder)
    {
        if (string.IsNullOrWhiteSpace(TextField))
        {
            builder.Input(attr =>
            {
                attr.Type("text").Id(Id).Name(Id).Disabled(!CanEdit)
                    .Value(Value).Required(Required)
                    .Add("autocomplete", "off")
                    .OnChange(CreateBinder());
            });
        }
        else
        {
            builder.Hidden(Id, Value);
            var text = FieldContext?.DicModel?.GetValue<string>(TextField);
            builder.Field<Text>(TextField, Required).IsInput(true).Value(text).Enabled(Enabled && CanEdit).Build();
        }

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