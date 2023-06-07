namespace Known.Razor.Extensions;

public static class HtmlExtension
{
    public static void Div(this RenderTreeBuilder builder, Action<AttributeBuilder> child = null) => builder.Element("div", attr => child?.Invoke(attr));

    public static void Div(this RenderTreeBuilder builder, string className, Action<AttributeBuilder> child = null)
    {
        builder.Div(attr =>
        {
            attr.Class(className);
            child?.Invoke(attr);
        });
    }

    public static void Div(this RenderTreeBuilder builder, string className, string text)
    {
        builder.Div(attr =>
        {
            attr.Class(className);
            builder.Text(text);
        });
    }

    public static void Anchor(this RenderTreeBuilder builder, string text, string url, string download = null)
    {
        builder.Element("a", attr =>
        {
            attr.Add("href", url)
                .Add("target", "_blank");
            if (!string.IsNullOrWhiteSpace(download))
                attr.Add("download", download);
            builder.Text(text);
        });
    }

    public static void Paragraph(this RenderTreeBuilder builder, Action<AttributeBuilder> child = null) => builder.Element("p", attr => child?.Invoke(attr));

    public static void Paragraph(this RenderTreeBuilder builder, string className, Action<AttributeBuilder> child = null)
    {
        builder.Paragraph(attr =>
        {
            attr.Class(className);
            child?.Invoke(attr);
        });
    }

    public static void Table(this RenderTreeBuilder builder, Action<AttributeBuilder> child = null)
    {
        builder.Element("table", child);
    }

    public static void Table(this RenderTreeBuilder builder, Action<RenderTreeBuilder> child = null)
    {
        var table = new TableContext();
        builder.Component<CascadingValue<TableContext>>(ab =>
        {
            ab.Set(c => c.IsFixed, false)
              .Set(c => c.Value, table)
              .Set(c => c.ChildContent, delegate (RenderTreeBuilder b)
              {
                  b.Element("table", attr => child?.Invoke(b));
              });
        });
    }

    public static void ColGroup(this RenderTreeBuilder builder, params int?[] widths)
    {
        var sb = new StringBuilder();
        sb.Append("<colgroup>");
        foreach (var item in widths)
        {
            if (item.HasValue)
            {
                if (item.Value < 100)
                    sb.Append($"<col style=\"width:{item.Value}%\" />");
                else
                    sb.Append($"<col style=\"width:{item.Value}px\" />");
            }
            else
            {
                sb.Append("<col />");
            }
        }
        sb.Append("</colgroup>");
        builder.Markup(sb.ToString());
    }

    public static void ColGroup(this RenderTreeBuilder builder, params string[] widths)
    {
        var sb = new StringBuilder();
        sb.Append("<colgroup>");
        foreach (var item in widths)
        {
            if (!string.IsNullOrWhiteSpace(item))
                sb.Append($"<col style=\"width:{item}\" />");
            else
                sb.Append("<col />");
        }
        sb.Append("</colgroup>");
        builder.Markup(sb.ToString());
    }

    public static void THead(this RenderTreeBuilder builder, Action<AttributeBuilder> child = null) => builder.Element("thead", attr => child?.Invoke(attr));
    public static void TBody(this RenderTreeBuilder builder, Action<AttributeBuilder> child = null) => builder.Element("tbody", attr => child?.Invoke(attr));
    public static void TFoot(this RenderTreeBuilder builder, Action<AttributeBuilder> child = null) => builder.Element("tfoot", attr => child?.Invoke(attr));
    public static void Tr(this RenderTreeBuilder builder, Action<AttributeBuilder> child = null) => builder.Element("tr", attr => child?.Invoke(attr));

    public static void Tr(this RenderTreeBuilder builder, string className, Action<AttributeBuilder> child = null)
    {
        builder.Tr(attr =>
        {
            attr.Class(className);
            child?.Invoke(attr);
        });
    }

    public static void Th(this RenderTreeBuilder builder, Action<AttributeBuilder> child = null) => builder.Element("th", attr => child?.Invoke(attr));

    public static void Th(this RenderTreeBuilder builder, string className, Action<AttributeBuilder> child = null)
    {
        builder.Th(attr =>
        {
            attr.Class(className);
            child?.Invoke(attr);
        });
    }

    public static void Th(this RenderTreeBuilder builder, string className, string text, int? rowSpan = null, int? colSpan = null)
    {
        builder.Th(attr =>
        {
            attr.Class(className);
            if (rowSpan != null && rowSpan.Value > 0)
                attr.RowSpan(rowSpan.Value);
            if (colSpan != null && colSpan.Value > 0)
                attr.ColSpan(colSpan.Value);
            builder.Text(text);
        });
    }

    public static void Td(this RenderTreeBuilder builder, Action<AttributeBuilder> child = null) => builder.Element("td", attr => child?.Invoke(attr));

    public static void Td(this RenderTreeBuilder builder, string className, Action<AttributeBuilder> child = null, int? colSpan = null)
    {
        builder.Td(attr =>
        {
            attr.Class(className);
			if (colSpan != null && colSpan.HasValue)
				attr.ColSpan(colSpan.Value);
			child?.Invoke(attr);
        });
    }

    public static void Td(this RenderTreeBuilder builder, string className, string text, int? colSpan = null)
    {
        builder.Td(attr =>
        {
            if (colSpan != null && colSpan.HasValue)
                attr.ColSpan(colSpan.Value);

            attr.Class(className);
            builder.Span("text", text);
        });
    }

    public static void ThTd(this RenderTreeBuilder builder, string label, string value, int? colSpan = null)
    {
        builder.Th(attr => builder.Label("", label));
        builder.Td(attr =>
        {
            if (colSpan != null && colSpan.HasValue)
                attr.ColSpan(colSpan.Value);

            builder.Div("form-input", attr => builder.Span("text", value));
        });
    }

    public static void Ul(this RenderTreeBuilder builder, Action<AttributeBuilder> child = null) => builder.Element("ul", attr => child?.Invoke(attr));

    public static void Ul(this RenderTreeBuilder builder, string className, Action<AttributeBuilder> child = null)
    {
        builder.Ul(attr =>
        {
            attr.Class(className);
            child?.Invoke(attr);
        });
    }

    public static void Li(this RenderTreeBuilder builder, Action<AttributeBuilder> child = null) => builder.Element("li", attr => child?.Invoke(attr));

    public static void Li(this RenderTreeBuilder builder, string className, Action<AttributeBuilder> child = null)
    {
        builder.Li(attr =>
        {
            attr.Class(className);
            child?.Invoke(attr);
        });
    }

    public static void Dl(this RenderTreeBuilder builder, Action<AttributeBuilder> child = null) => builder.Element("dl", attr => child?.Invoke(attr));

    public static void Dl(this RenderTreeBuilder builder, string className, Action<AttributeBuilder> child = null)
    {
        builder.Dl(attr =>
        {
            attr.Class(className);
            child?.Invoke(attr);
        });
    }

    public static void Dd(this RenderTreeBuilder builder, Action<AttributeBuilder> child = null) => builder.Element("dd", attr => child?.Invoke(attr));

    public static void Dd(this RenderTreeBuilder builder, string className, Action<AttributeBuilder> child = null)
    {
        builder.Dd(attr =>
        {
            attr.Class(className);
            child?.Invoke(attr);
        });
    }

    public static void Pre(this RenderTreeBuilder builder, string text) => builder.Element("pre", attr => builder.Text(text));
    public static void Pre(this RenderTreeBuilder builder, string className, string text)
    {
        builder.Element("pre", attr =>
        {
            attr.Class(className);
            builder.Text(text);
        });
    }
    
    public static void Span(this RenderTreeBuilder builder, Action<AttributeBuilder> child = null) => builder.Element("span", attr => child?.Invoke(attr));

    public static void Span(this RenderTreeBuilder builder, string className, Action<AttributeBuilder> child)
    {
        builder.Span(attr =>
        {
            attr.Class(className);
            child.Invoke(attr);
        });
    }

    public static void Span(this RenderTreeBuilder builder, string text) => builder.Span(attr => builder.Text(text));

    public static void Span(this RenderTreeBuilder builder, string className, string text, EventCallback? onClick = null)
    {
        builder.Span(attr =>
        {
            attr.Class(className).OnClick(onClick);
            builder.Text(text);
        });
    }

    public static void Icon(this RenderTreeBuilder builder, Action<AttributeBuilder> child = null) => builder.Element("i", attr => child?.Invoke(attr));

    public static void Icon(this RenderTreeBuilder builder, string icon, Action<AttributeBuilder> child = null)
    {
        builder.Icon(attr =>
        {
            attr.Class($"icon {icon}");
            child?.Invoke(attr);
        });
    }

    public static void Label(this RenderTreeBuilder builder, Action<AttributeBuilder> child = null) => builder.Element("label", attr => child?.Invoke(attr));

    public static void Label(this RenderTreeBuilder builder, string className, Action<AttributeBuilder> child = null)
    {
        builder.Label(attr =>
        {
            attr.Class(className);
            child?.Invoke(attr);
        });
    }

    public static void Label(this RenderTreeBuilder builder, string className, string text) => builder.Label(className, attr => builder.Text(text));
    public static void Input(this RenderTreeBuilder builder, Action<AttributeBuilder> child = null) => builder.Element("input", attr => child?.Invoke(attr));

    public static void Check(this RenderTreeBuilder builder, Action<AttributeBuilder> child = null)
    {
        builder.Input(attr =>
        {
            attr.Type("checkbox");
            child?.Invoke(attr);
        });
    }

    public static void Radio(this RenderTreeBuilder builder, Action<AttributeBuilder> child = null)
    {
        builder.Input(attr =>
        {
            attr.Type("radio");
            child?.Invoke(attr);
        });
    }

    public static void TextArea(this RenderTreeBuilder builder, Action<AttributeBuilder> child = null) => builder.Element("textarea", attr => child?.Invoke(attr));
    public static void Select(this RenderTreeBuilder builder, Action<AttributeBuilder> child = null) => builder.Element("select", attr => child?.Invoke(attr));
    public static void Option(this RenderTreeBuilder builder, Action<AttributeBuilder> child = null) => builder.Element("option", attr => child?.Invoke(attr));
    public static void Img(this RenderTreeBuilder builder, Action<AttributeBuilder> child = null) => builder.Element("img", attr => child?.Invoke(attr));

    public static void Link(this RenderTreeBuilder builder, string text, EventCallback onClick, string style = null)
    {
        builder.Span($"link {style}", attr =>
        {
            attr.OnClick(onClick);
            builder.Text(text);
        });
    }
}