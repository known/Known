namespace Known.Razor.Components.Fields;

public class Picker : Field
{
    private bool isInitOK;
    private string text;

    [Parameter] public IPicker Pick { get; set; }
    [Parameter] public string TextField { get; set; }
    [Parameter] public string PickSeparator { get; set; }
    [Parameter] public string PickIdKey { get; set; }
    [Parameter] public string PickTextKey { get; set; }
    [Parameter] public Action<object> OnPicked { get; set; }
    [Parameter] public bool CanEdit { get; set; }

    protected override Task OnInitializedAsync()
    {
        isInitOK = Pick != null && Pick.OnPicked != null;
        return base.OnInitializedAsync();
    }

    protected override Task OnParametersSetAsync()
    {
        if (!string.IsNullOrWhiteSpace(TextField))
            text = FieldContext?.DicModel?.GetValue<string>(TextField);
        return base.OnParametersSetAsync();
    }

    protected override void BuildText(RenderTreeBuilder builder)
    {
        if (string.IsNullOrWhiteSpace(TextField))
            base.BuildText(builder);
        else
            builder.Span("text", text);
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
            builder.Field<Hidden>(Id).Value(Value);
            builder.Field<Text>(TextField, Required).IsInput(true).Value(text).Enabled(Enabled && CanEdit).Build();
        }

        if (!Enabled || ReadOnly || Pick == null)
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
        OnFieldChanged(value);
    }

    private void OnFieldChanged(object value)
    {
        if (string.IsNullOrWhiteSpace(TextField))
        {
            SetFieldValue(value);
        }
        else
        {
            var value1 = GetValue(value);
            text = value1.Item2;
            FieldContext?.SetField(TextField, text);
            SetFieldValue(value1.Item1);
        }

        OnValueChange();
        StateChanged();
    }

    private Tuple<string, string> GetValue(object value)
    {
        var id = string.Empty;
        var text = string.Empty;
        if (value == null)
            return new Tuple<string, string>(id, text);

        if (!string.IsNullOrWhiteSpace(PickSeparator))
        {
            var values = value.ToString().Split(PickSeparator);
            id = values[0];
            text = values.Length > 1 ? values[1] : id;
        }
        else
        {
            id = TypeHelper.GetValue<string>(value, PickIdKey);
            text = TypeHelper.GetValue<string>(value, PickTextKey);
        }

        return new Tuple<string, string>(id, text);
    }
}