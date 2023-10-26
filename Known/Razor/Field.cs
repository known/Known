namespace Known.Razor;

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
    [Parameter] public int? Width { get; set; }
    [Parameter] public int? Height { get; set; }

    [Parameter] public Action<RenderTreeBuilder> InputTemplate { get; set; }
    [Parameter] public Action<string> ValueChanged { get; set; }
    [Parameter] public Action<FieldContext> OnValueChanged { get; set; }
    [Parameter] public Action<string> OnSave { get; set; }

    [CascadingParameter] internal TableContext Table { get; set; }
    [CascadingParameter] protected FieldContext FieldContext { get; set; }

    protected bool IsReadOnly => ReadOnly || FieldContext != null && FieldContext.ReadOnly;

    protected void SetError(bool isError) => error = isError ? "error" : "";

    internal virtual void SetFieldVisible(bool visible) => Visible = visible;
    internal virtual void SetFieldReadOnly(bool readOnly) => ReadOnly = readOnly;
    internal virtual void SetFieldEnabled(bool enabled) => Enabled = enabled;
    internal virtual object GetFieldValue() => Value;

    internal virtual void SetFieldValue(object value)
    {
        Value = FormatValue(value);
        SetFieldContext();
    }

    internal virtual void ClearFieldValue()
    {
        Value = FormatValue(orgValue);
        SetFieldContext();
    }

    public T ValueAs<T>()
    {
        var value = GetFieldValue();
        if (value is T val)
            return val;

        return Utils.ConvertTo<T>(value);
    }

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
        ClearFieldValue();
        StateChanged();
    }

    public void SetValue(object value)
    {
        SetFieldValue(value);
        StateChanged();
    }

    public void SetRequired(bool required)
    {
        Required = required;
        StateChanged();
    }

    public void SetVisible(bool visible)
    {
        SetFieldVisible(visible);
        StateChanged();
    }

    public void SetEnabled(bool enabled)
    {
        SetFieldEnabled(enabled);
        StateChanged();
    }

    public void SetReadOnly(bool readOnly)
    {
        SetFieldReadOnly(readOnly);
        StateChanged();
    }

    internal void ShowError(bool isError)
    {
        error = isError ? "error" : "";
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
                Value = FormatValue(orgValue);
            }
        }
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (!Visible)
            return;

        if (IsInput)
        {
            if (IsReadOnly)
                BuildText(builder);
            else
                BuildInput(builder);
            return;
        }

        if (Table != null)
            BuildTableField(builder);
        else
            BuildDivField(builder);
    }

    protected virtual string FormatValue(object value) => value?.ToString();
    protected virtual void BuildText(RenderTreeBuilder builder) => builder.Span("text", Value);
    protected virtual void BuildInput(RenderTreeBuilder builder) { }
    protected virtual void SetContext(FieldContext context) { }

    internal EventCallback<ChangeEventArgs> CreateBinder()
    {
        return EventCallback.Factory.CreateBinder(this, value =>
        {
            Value = FormatValue(value);
            OnValueChange();
        }, Value);
    }

    protected void OnValueChange()
    {
        ValueChanged?.Invoke(Value);
        if (FieldContext != null)
        {
            SetFieldContext();
            OnValueChanged?.Invoke(FieldContext);
        }
    }

    private void SetFieldContext()
    {
        if (FieldContext == null)
            return;

        var value = GetFieldValue();
        FieldContext.FieldId = Id;
        FieldContext.FieldValue = value;
        FieldContext.SetField(Id, value);
        SetContext(FieldContext);
    }

    private void BuildTableField(RenderTreeBuilder builder)
    {
        var required = Required && !IsReadOnly ? "required" : "";
        builder.Th(required, attr =>
        {
            if (RowSpan > 1)
                attr.RowSpan(RowSpan);
            builder.Label(attr =>
            {
                attr.For(Id);
                builder.Text(Label);
            });
        });
        builder.Td(Style, attr =>
        {
            if (RowSpan > 1)
                attr.RowSpan(RowSpan);
            if (ColSpan > 1)
                attr.ColSpan(ColSpan);
            BuildFormInput(builder);
        });
    }

    private void BuildDivField(RenderTreeBuilder builder)
    {
        if (string.IsNullOrWhiteSpace(Label))
        {
            BuildFormInput(builder, Style);
        }
        else
        {
            var css = CssBuilder.Default("form-item").AddClass(Style).Build();
            builder.Div(css, attr =>
            {
                var css1 = CssBuilder.Default("form-label").AddClass("required", Required && !IsReadOnly).Build();
                builder.Label(css1, attr =>
                {
                    attr.For(Id);
                    builder.Text(Label);
                });
                BuildFormInput(builder);
            });
        }
    }

    private bool isEdit = false;
    private void BuildFormInput(RenderTreeBuilder builder, string style = null)
    {
        var isCheck = false;
        if (FieldContext != null)
        {
            var checkFields = FieldContext.CheckFields;
            isCheck = !string.IsNullOrWhiteSpace(checkFields) && checkFields.Contains(Id);
        }

        var sb = StyleBuilder.Default.Width(Width).Height(Height).Build();
        var className = CssBuilder.Default("form-input")
                                  .AddClass(error)
                                  .AddClass(style)
                                  .AddClass("readonly", IsReadOnly)
                                  .AddClass("check", isCheck)
                                  .Build();
        builder.Div(className, attr =>
        {
            attr.Style(sb);
            if (InputTemplate != null)
            {
                InputTemplate?.Invoke(builder);
            }
            else
            {
                if (IsReadOnly && !isEdit)
                {
                    BuildText(builder);
                    if (IsEdit)
                        BuildEditButton(builder);
                }
                else
                {
                    BuildInput(builder);
                }
            }

            if (isCheck)
                BuildCheck(builder);

            if (isEdit)
                BuildEditAction(builder);
        });

        if (!string.IsNullOrWhiteSpace(Tips))
            builder.Div("form-tips", Tips);
    }

    private void BuildCheck(RenderTreeBuilder builder)
    {
        builder.Label("form-check", attr =>
        {
            builder.Input(attr =>
            {
                attr.Type("checkbox").Name($"Check{Id}");
            });
        });
    }

    private void BuildEditButton(RenderTreeBuilder builder)
    {
        builder.Span("link", attr =>
        {
            attr.OnClick(Callback(e => isEdit = true));
            builder.Text("更改");
        });
    }

    private void BuildEditAction(RenderTreeBuilder builder)
    {
        builder.Span("link", attr =>
        {
            attr.OnClick(Callback(e =>
            {
                OnSave?.Invoke(Value);
                isEdit = false;
            }));
            builder.Text("保存");
        });
        builder.Span("link", attr =>
        {
            attr.OnClick(Callback(e => isEdit = false));
            builder.Text("取消");
        });
    }
}