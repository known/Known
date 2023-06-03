namespace Known.Razor.Components;

public abstract class Field : BaseComponent
{
    private string error;
    private object orgValue;

    [Parameter] public string Label { get; set; }
    [Parameter] public string Value { get; set; }
    [Parameter] public string Style { get; set; }
    [Parameter] public string Tips { get; set; }
    [Parameter] public bool Required { get; set; }
    [Parameter] public bool IsEdit { get; set; }
    [Parameter] public bool IsInput { get; set; }

    [Parameter] public int RowSpan { get; set; }
    [Parameter] public int ColSpan { get; set; }
    [Parameter] public int? Height { get; set; }

    [Parameter] public Action<RenderTreeBuilder> Child { get; set; }
    [Parameter] public Action<string> ValueChanged { get; set; }
    [Parameter] public Action<FieldContext> OnValueChanged { get; set; }
    [Parameter] public Action<string> OnSave { get; set; }

    [CascadingParameter] protected FieldContext FieldContext { get; set; }

    protected void SetError(bool isError) => error = isError ? "error" : "";

    public T GetValue<T>() => Utils.ConvertTo<T>(Value);
    public virtual object GetValue() => Value;

    public virtual bool Validate()
    {
        error = string.Empty;
        if (string.IsNullOrEmpty(Value) && Required)
        {
            error = "error";
            return false;
        }

        return true;
    }

    public void Clear()
    {
        SetInputValue(orgValue);
        StateChanged();
    }

    public virtual void SetValue(object value)
    {
        SetInputValue(value);
        StateChanged();
    }

    public void SetRequired(bool required)
    {
        Required = required;
        StateChanged();
    }

    public void SetReadOnly(bool readOnly)
    {
        ReadOnly = readOnly;
        StateChanged();
    }

    public void SetEnabled(bool enabled)
    {
        Enabled = enabled;
        StateChanged();
    }

    public void SetVisible(bool visible)
    {
        Visible = visible;
        StateChanged();
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        orgValue = Value;
        if (FieldContext != null && !string.IsNullOrWhiteSpace(Id))
        {
            FieldContext.Fields[Id] = this;

            var model = FieldContext.DicModel;
            if (model != null && model.ContainsKey(Id))
            {
                orgValue = model[Id];
                SetInputValue(orgValue);
            }
        }
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (!Visible)
            return;

        builder.Label(attr =>
        {
            attr.For(Id).Class(error);
            if (!string.IsNullOrWhiteSpace(Label))
                builder.Text(Label);

            BuildInput(builder);
            Child?.Invoke(builder);
        });
    }

    protected virtual string FormatValue(object value) => value?.ToString();
    protected virtual void BuildInput(RenderTreeBuilder builder) { }
    protected virtual void SetInputValue(object value) => Value = FormatValue(value);
    protected virtual void SetFieldContext(FieldContext context) { }

    internal void BuildRadio(RenderTreeBuilder builder, string type, string text, string value, bool enabled, bool isChecked, Action<bool, string> action = null, int? columnCount = null)
    {
        builder.Label("radio", attr =>
        {
            if (columnCount != null && columnCount > 0)
            {
                var width = Utils.Round(100.0 / columnCount.Value, 2);
                attr.Style($"width:{width}%;margin-right:0;");
            }
            builder.Input(attr =>
            {
                attr.Type(type).Name(Id).Disabled(!enabled)
                    .Value(value).Checked(isChecked);
                if (type == "checkbox")
                {
                    attr.OnChange(EventCallback.Factory.CreateBinder<bool>(this, v =>
                    {
                        action?.Invoke(v, value);
                        OnValueChange();
                    }, isChecked));
                }
                else
                {
                    attr.OnChange(CreateBinder());
                }
            });
            builder.Text(text);
        });
    }

    internal EventCallback<ChangeEventArgs> CreateBinder(Action<DateTime?> action = null)
    {
        return EventCallback.Factory.CreateBinder(this, value =>
        {
            Value = value;
            if (action != null)
            {
                DateTime? date = null;
                if (!string.IsNullOrWhiteSpace(value))
                    date = DateTime.Parse(value);
                action.Invoke(date);
            }
            OnValueChange();
        }, Value);
    }
    
    protected void OnValueChange()
    {
        ValueChanged?.Invoke(Value);
        if (FieldContext != null)
        {
            var value = GetValue();
            FieldContext.FieldId = Id;
            FieldContext.FieldValue = value;
            FieldContext.SetModel(Id, value);
            SetFieldContext(FieldContext);
            OnValueChanged?.Invoke(FieldContext);
        }
    }
}