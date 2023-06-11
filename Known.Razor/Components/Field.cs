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

    internal virtual object GetValue() => Value;

    public T ValueAs<T>()
    {
        var value = GetValue();
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

        if (IsInput)
        {
            BuildDivField(builder);
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
    protected virtual void SetInputValue(object value) => Value = FormatValue(value);
    protected virtual void SetContext(FieldContext context) { }

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
            SetContext(FieldContext);
            OnValueChanged?.Invoke(FieldContext);
        }
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
            BuildFormInput(builder);
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
    private void BuildFormInput(RenderTreeBuilder builder)
    {
        var isCheck = false;
        if (FieldContext != null)
        {
            var checkFields = FieldContext.CheckFields;
            isCheck = !string.IsNullOrWhiteSpace(checkFields) && checkFields.Contains(Id);
        }

        var sb = new StyleBuilder();
        var style = sb.Width(Width).Height(Height).Build();
        var className = CssBuilder.Default("form-input")
                                  .AddClass(error)
                                  .AddClass("readonly", IsReadOnly)
                                  .AddClass("check", isCheck)
                                  .Build();
        builder.Div(className, attr =>
        {
            attr.Style(style);
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
        });

        if (isEdit)
            BuildEditAction(builder);

        if (!string.IsNullOrWhiteSpace(Tips))
            builder.Span("form-tips", Tips);
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