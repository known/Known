/* -------------------------------------------------------------------------------
 * Copyright (c) Suzhou Puman Technology Co., Ltd. All rights reserved.
 * 
 * WebSite: https://www.pumantech.com
 * Contact: knownchen@163.com
 * 
 * Change Logs:
 * Date           Author       Notes
 * 2022-04-01     KnownChen
 * ------------------------------------------------------------------------------- */

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Razor;

public class Field : BaseComponent
{
    private string error;
    private object orgValue;

    [Parameter] public string Id { get; set; }
    [Parameter] public string Label { get; set; }
    [Parameter] public string Value { get; set; }
    [Parameter] public string Style { get; set; }
    [Parameter] public string Tips { get; set; }
    [Parameter] public bool Required { get; set; }
    [Parameter] public bool ReadOnly { get; set; }
    [Parameter] public bool Enabled { get; set; } = true;
    [Parameter] public bool Visible { get; set; } = true;
    [Parameter] public bool IsQuery { get; set; }
    [Parameter] public bool IsEdit { get; set; }
    [Parameter] public int RowSpan { get; set; }
    [Parameter] public int ColSpan { get; set; }
    [Parameter] public int? Width { get; set; }
    [Parameter] public int? Height { get; set; }
    [Parameter] public RenderFragment<object> ChildContent { get; set; }
    [Parameter] public EventCallback<string> ValueChanged { get; set; }
    [Parameter] public Action<FieldContext, object> OnValueChanged { get; set; }
    [Parameter] public Action<string> OnSave { get; set; }

    internal virtual string GridCellStyle
    {
        get { return Id == "Action" ? "txt-center" : ""; }
    }

    [CascadingParameter]
    protected FieldContext FieldContext { get; set; }

    private bool IsReadOnly
    {
        get { return ReadOnly || (FieldContext != null && FieldContext.ReadOnly); }
    }

    public bool Validate()
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
        StateHasChanged();
    }

    public T GetValue<T>()
    {
        return Utils.ConvertTo<T>(Value);
    }

    public virtual object GetValue()
    {
        return Value;
    }

    public virtual void SetValue(object value)
    {
        SetInputValue(value);
        StateHasChanged();
    }

    public void SetRequired(bool required)
    {
        Required = required;
        StateHasChanged();
    }

    public void SetReadOnly(bool readOnly)
    {
        ReadOnly = readOnly;
        StateHasChanged();
    }

    public void SetEnabled(bool enabled)
    {
        Enabled = enabled;
        StateHasChanged();
    }

    public void SetVisible(bool visible)
    {
        Visible = visible;
        StateHasChanged();
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        orgValue = Value;
        if (FieldContext != null && !string.IsNullOrWhiteSpace(Id))
        {
            FieldContext.Fields[Id] = this;

            var model = FieldContext.Model;
            if (model != null && model.ContainsKey(Id))
            {
                orgValue = model[Id];
                SetInputValue(orgValue);
            }
        }
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (!Visible || (FieldContext != null && FieldContext.IsGridView))
            return;

        if (string.IsNullOrWhiteSpace(Label))
        {
            BuildFormInput(builder);
            return;
        }

        if (FieldContext != null && FieldContext.IsTableForm)
            BuildTableField(builder);
        else
            BuildDivField(builder);
    }

    protected virtual string FormatValue(object value)
    {
        return value?.ToString();
    }

    internal virtual void BuildQuery(RenderTreeBuilder builder)
    {
        builder.Component<Text>(attr =>
        {
            attr.Add(nameof(Id), Id)
                .Add(nameof(Label), Label);
        });
    }

    internal void BuildGridCell(RenderTreeBuilder builder, object value)
    {
        if (IsEdit)
        {
            Value = FormatValue(value);
            BuidChildContent(builder);
        }
        else
        {
            BuildGridCellText(builder, value);
        }
    }

    protected virtual void BuildGridCellText(RenderTreeBuilder builder, object value)
    {
        var text = FormatValue(value);
        builder.Text(text);
    }

    protected virtual void BuildChildText(RenderTreeBuilder builder)
    {
        builder.Text(Value);
    }

    protected virtual void BuidChildContent(RenderTreeBuilder builder)
    {
    }

    protected virtual void SetInputValue(object value)
    {
        Value = value?.ToString();
    }

    protected void BuildRadio(RenderTreeBuilder builder, string type, string text, string value, bool isChecked, Action<bool, string> action = null)
    {
        builder.Label("form-radio", attr =>
        {
            builder.Input(attr =>
            {
                attr.Type(type).Name(Id).Disabled(!Enabled)
                    .Value(value).Checked(isChecked);
                if (type == "checkbox")
                {
                    attr.OnChange(EventCallback.Factory.CreateBinder<bool>(this, v =>
                    {
                        action?.Invoke(v, value);
                        FieldContext.Field = Id;
                        OnValueChanged?.Invoke(FieldContext, value);
                    }, isChecked));
                }
                else
                {
                    attr.OnChange(CreateBinder());
                }
            });
            builder.Span(text);
        });
    }

    protected EventCallback<ChangeEventArgs> CreateBinder(Action<DateTime> action = null)
    {
        return EventCallback.Factory.CreateBinder(this, value =>
        {
            Value = value;
            action?.Invoke(DateTime.Parse(value));
            if (AppContext != null)
            {
                FieldContext.Field = Id;
                OnValueChanged?.Invoke(FieldContext, GetValue());
            }
        }, Value);
    }

    private void BuildTableField(RenderTreeBuilder builder)
    {
        var required = Required && !IsReadOnly ? "required" : "";
        builder.Th(required, attr =>
        {
            if (RowSpan > 1)
                attr.Add("rowspan", RowSpan);
            builder.Label(attr =>
            {
                attr.For(Id);
                builder.Text(Label);
            });
        });
        builder.Td(attr =>
        {
            if (RowSpan > 1)
                attr.Add("rowspan", RowSpan);
            if (ColSpan > 1)
                attr.Add("colspan", ColSpan);
            BuildFormInput(builder);
        });
    }

    private void BuildDivField(RenderTreeBuilder builder)
    {
        builder.Div("form-item", attr =>
        {
            var required = Required && !IsReadOnly ? " required" : "";
            builder.Label($"form-label{required}", attr =>
            {
                attr.For(Id);
                builder.Text(Label);
            });
            BuildFormInput(builder);
        });
    }

    private bool isEdit = false;
    private void BuildFormInput(RenderTreeBuilder builder)
    {
        var sb = new StyleBuilder();
        var style = sb.Width(Width).Height(Height).Build();
        builder.Div($"form-input {Style} {error}", attr =>
        {
            attr.Style(style);
            if (IsReadOnly && !isEdit)
            {
                builder.Span("text", attr => BuildChildText(builder));
                if (IsEdit)
                {
                    BuildEditButton(builder);
                }
            }
            else
            {
                if (ChildContent != null)
                    builder.Fragment(ChildContent(null));
                else
                    BuidChildContent(builder);
            }
        });

        if (isEdit)
        {
            BuildEditAction(builder);
        }

        if (!string.IsNullOrWhiteSpace(Tips))
        {
            builder.Span("form-tips", Tips);
        }
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
