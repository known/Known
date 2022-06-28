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

public class AttributeBuilder
{
    private readonly RenderTreeBuilder builder;

    public AttributeBuilder(RenderTreeBuilder builder)
    {
        this.builder = builder;
    }

    public AttributeBuilder Id(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return this;

        return Add("id", value);
    }

    public AttributeBuilder Name(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return this;

        return Add("name", value);
    }

    public AttributeBuilder For(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return this;

        return Add("for", value);
    }

    public AttributeBuilder Class(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return this;

        return Add("class", value);
    }

    public AttributeBuilder Style(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return this;

        return Add("style", value);
    }

    public AttributeBuilder Title(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return this;

        return Add("title", value);
    }

    public AttributeBuilder Placeholder(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return this;

        return Add("placeholder", value);
    }

    public AttributeBuilder Type(string value)
    {
        return Add("type", value);
    }

    public AttributeBuilder Value(string value)
    {
        return Add("value", value);
    }

    public AttributeBuilder Src(string value)
    {
        return Add("src", value);
    }

    public AttributeBuilder RowSpan(int rowSpan)
    {
        return Add("rowspan", rowSpan);
    }

    public AttributeBuilder ColSpan(int colSpan)
    {
        return Add("colspan", colSpan);
    }

    public AttributeBuilder Required(bool value)
    {
        builder.AddAttribute(0, "required", value);
        return this;
    }

    public AttributeBuilder Disabled(bool value)
    {
        builder.AddAttribute(0, "disabled", value);
        return this;
    }

    public AttributeBuilder Checked(bool value)
    {
        builder.AddAttribute(0, "checked", value);
        return this;
    }

    public AttributeBuilder Selected(bool value)
    {
        builder.AddAttribute(0, "selected", value);
        return this;
    }

    public AttributeBuilder OnClick(EventCallback? onClick)
    {
        if (onClick != null)
            builder.AddAttribute(0, "onclick", onClick);

        return this;
    }

    public AttributeBuilder OnChange(EventCallback<ChangeEventArgs>? onClick)
    {
        if (onClick != null)
            builder.AddAttribute(0, "onchange", onClick);

        return this;
    }

    public AttributeBuilder Add(string name, string value)
    {
        builder.AddAttribute(0, name, value);
        return this;
    }

    public AttributeBuilder Add(string name, object value)
    {
        builder.AddAttribute(0, name, value);
        return this;
    }
}

public class StyleBuilder
{
    private readonly Dictionary<string, string> styles = new();

    public StyleBuilder Add(string key, string value)
    {
        if (!string.IsNullOrEmpty(value))
            styles[key] = value;

        return this;
    }

    public StyleBuilder Width(int? width)
    {
        if (width.HasValue)
            styles["width"] = $"{width.Value}px";

        return this;
    }

    public StyleBuilder Height(int? height)
    {
        if (height.HasValue)
            styles["height"] = $"{height.Value}px";

        return this;
    }

    public string Build()
    {
        return string.Join(";", styles.Select(s => $"{s.Key}:{s.Value}"));
    }
}

public class FieldBuilder
{
    private readonly AttributeBuilder attr;

    public FieldBuilder(AttributeBuilder attr)
    {
        this.attr = attr;
    }

    public FieldBuilder Add(string name, string value)
    {
        attr.Add(name, value);
        return this;
    }

    public FieldBuilder Field(string label, string id)
    {
        attr.Add(nameof(Razor.Field.Label), label)
            .Add(nameof(Razor.Field.Id), id);
        return this;
    }

    public FieldBuilder Id(string id)
    {
        attr.Add(nameof(Razor.Field.Id), id);
        return this;
    }

    public FieldBuilder Label(string label)
    {
        attr.Add(nameof(Razor.Field.Label), label);
        return this;
    }

    public FieldBuilder Value(string value)
    {
        attr.Add(nameof(Razor.Field.Value), value);
        return this;
    }

    public FieldBuilder Value(DateTime value)
    {
        attr.Add(nameof(Date.Day), value);
        return this;
    }

    public FieldBuilder Style(string style)
    {
        attr.Add(nameof(Razor.Field.Style), style);
        return this;
    }

    public FieldBuilder Tips(string tips)
    {
        attr.Add(nameof(Razor.Field.Tips), tips);
        return this;
    }

    public FieldBuilder Required(bool required)
    {
        attr.Add(nameof(Razor.Field.Required), required);
        return this;
    }

    public FieldBuilder ReadOnly(bool readOnly)
    {
        attr.Add(nameof(Razor.Field.ReadOnly), readOnly);
        return this;
    }

    public FieldBuilder Enabled(bool enabled)
    {
        attr.Add(nameof(Razor.Field.Enabled), enabled);
        return this;
    }

    public FieldBuilder Visible(bool visible)
    {
        attr.Add(nameof(Razor.Field.Visible), visible);
        return this;
    }

    public FieldBuilder IsQuery(bool isQuery)
    {
        attr.Add(nameof(Razor.Field.IsQuery), isQuery);
        return this;
    }

    public FieldBuilder IsEdit(bool isEdit)
    {
        attr.Add(nameof(Razor.Field.IsEdit), isEdit);
        return this;
    }

    public FieldBuilder IsRangeQuery(bool isQuery)
    {
        attr.Add(nameof(Date.IsRangeQuery), isQuery);
        return this;
    }

    public FieldBuilder RowSpan(int rowSpan)
    {
        attr.Add(nameof(Razor.Field.RowSpan), rowSpan);
        return this;
    }

    public FieldBuilder ColSpan(int colSpan)
    {
        attr.Add(nameof(Razor.Field.ColSpan), colSpan);
        return this;
    }

    public FieldBuilder Width(int? width)
    {
        attr.Add(nameof(Razor.Field.Width), width);
        return this;
    }

    public FieldBuilder Height(int? height)
    {
        attr.Add(nameof(Razor.Field.Height), height);
        return this;
    }

    public FieldBuilder ChildContent(RenderFragment<object> childContent)
    {
        attr.Add(nameof(Razor.Field.ChildContent), childContent);
        return this;
    }

    public FieldBuilder ValueChanged(EventCallback<string> valueChanged)
    {
        attr.Add(nameof(Razor.Field.ValueChanged), valueChanged);
        return this;
    }

    public FieldBuilder OnValueChanged(Action<FieldContext, object> onValueChanged)
    {
        attr.Add(nameof(Razor.Field.OnValueChanged), onValueChanged);
        return this;
    }

    public FieldBuilder OnSave(Action<string> onSave)
    {
        attr.Add(nameof(Razor.Field.OnSave), onSave);
        return this;
    }

    public FieldBuilder Codes(string codes)
    {
        attr.Add(nameof(ListField.Codes), codes);
        return this;
    }

    public FieldBuilder Items(CodeInfo[] items)
    {
        attr.Add(nameof(ListField.Items), items);
        return this;
    }

    public FieldBuilder CodeAction(Func<CodeInfo[]> codeAction)
    {
        attr.Add(nameof(ListField.CodeAction), codeAction);
        return this;
    }

    public FieldBuilder EmptyText(string emptyText)
    {
        attr.Add(nameof(Select.EmptyText), emptyText);
        return this;
    }

    public FieldBuilder Text(string text)
    {
        attr.Add(nameof(CheckBox.Text), text);
        return this;
    }

    public FieldBuilder Unit(string unit)
    {
        attr.Add(nameof(Number.Unit), unit);
        return this;
    }

    public FieldBuilder Icon(string icon)
    {
        attr.Add(nameof(Razor.Text.Icon), icon);
        return this;
    }

    public FieldBuilder Placeholder(string placeholder)
    {
        attr.Add(nameof(Razor.Text.Placeholder), placeholder);
        return this;
    }

    public FieldBuilder Picker(IPicker picker)
    {
        attr.Add(nameof(Razor.Picker.Pick), picker);
        return this;
    }
}
