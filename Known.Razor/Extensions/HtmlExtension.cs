namespace Known.Razor.Extensions;

public static class HtmlExtension
{
    public static RenderTreeBuilder H1(this RenderTreeBuilder builder, string text) => builder.Element("h1", attr => builder.Text(text));
    public static RenderTreeBuilder H2(this RenderTreeBuilder builder, string text) => builder.Element("h2", attr => builder.Text(text));
    public static RenderTreeBuilder H3(this RenderTreeBuilder builder, string text) => builder.Element("h3", attr => builder.Text(text));
    public static RenderTreeBuilder H4(this RenderTreeBuilder builder, string text) => builder.Element("h4", attr => builder.Text(text));
    public static RenderTreeBuilder H5(this RenderTreeBuilder builder, string text) => builder.Element("h5", attr => builder.Text(text));
    public static RenderTreeBuilder H6(this RenderTreeBuilder builder, string text) => builder.Element("h6", attr => builder.Text(text));
    public static RenderTreeBuilder Small(this RenderTreeBuilder builder, string text) => builder.Element("small", attr => builder.Text(text));
    public static RenderTreeBuilder Sup(this RenderTreeBuilder builder, string text) => builder.Element("sup", attr => builder.Text(text));
    public static RenderTreeBuilder Header(this RenderTreeBuilder builder, Action<AttributeBuilder> child = null) => builder.Element("header", attr => child?.Invoke(attr));
    public static RenderTreeBuilder Aside(this RenderTreeBuilder builder, Action<AttributeBuilder> child = null) => builder.Element("aside", attr => child?.Invoke(attr));
    public static RenderTreeBuilder Main(this RenderTreeBuilder builder, Action<AttributeBuilder> child = null) => builder.Element("main", attr => child?.Invoke(attr));
    public static RenderTreeBuilder Nav(this RenderTreeBuilder builder, Action<AttributeBuilder> child = null) => builder.Element("nav", attr => child?.Invoke(attr));
    public static RenderTreeBuilder Article(this RenderTreeBuilder builder, Action<AttributeBuilder> child = null) => builder.Element("article", attr => child?.Invoke(attr));
    public static RenderTreeBuilder Details(this RenderTreeBuilder builder, Action<AttributeBuilder> child = null) => builder.Element("details", attr => child?.Invoke(attr));
    public static RenderTreeBuilder Summary(this RenderTreeBuilder builder, Action<AttributeBuilder> child = null) => builder.Element("summary", attr => child?.Invoke(attr));

    public static RenderTreeBuilder Blockquote(this RenderTreeBuilder builder, string className, Action<AttributeBuilder> child = null)
    {
        return builder.Element("blockquote", attr =>
        {
            attr.Class(className);
            child?.Invoke(attr);
        });
    }

    public static RenderTreeBuilder Form(this RenderTreeBuilder builder, string className, Action<AttributeBuilder> child = null)
    {
        return builder.Element("form", attr =>
        {
            attr.Class(className).Add("onsubmit", "return false;");
            child?.Invoke(attr);
        });
    }

    public static RenderTreeBuilder Div(this RenderTreeBuilder builder, Action<AttributeBuilder> child = null) => builder.Element("div", attr => child?.Invoke(attr));

    public static RenderTreeBuilder Div(this RenderTreeBuilder builder, string className, Action<AttributeBuilder> child = null)
    {
        return builder.Div(attr =>
        {
            attr.Class(className);
            child?.Invoke(attr);
        });
    }

    public static RenderTreeBuilder Div(this RenderTreeBuilder builder, string className, string text)
    {
        return builder.Div(attr =>
        {
            attr.Class(className);
            builder.Text(text);
        });
    }

    public static RenderTreeBuilder Anchor(this RenderTreeBuilder builder, string text, string url, string download = null)
    {
        return builder.Element("a", attr =>
        {
            attr.Add("href", url)
                .Add("target", "_blank");
            if (!string.IsNullOrWhiteSpace(download))
                attr.Add("download", download);
            builder.Text(text);
        });
    }

    public static RenderTreeBuilder Paragraph(this RenderTreeBuilder builder, Action<AttributeBuilder> child = null) => builder.Element("p", attr => child?.Invoke(attr));

    public static RenderTreeBuilder Paragraph(this RenderTreeBuilder builder, string className, Action<AttributeBuilder> child = null)
    {
        return builder.Paragraph(attr =>
        {
            attr.Class(className);
            child?.Invoke(attr);
        });
    }

    public static RenderTreeBuilder Table(this RenderTreeBuilder builder, Action<AttributeBuilder> child = null) => builder.Element("table", attr => child?.Invoke(attr));

    public static RenderTreeBuilder Table(this RenderTreeBuilder builder, string className, Action<AttributeBuilder> child = null)
    {
        return builder.Table(attr =>
        {
            attr.Class(className);
            child?.Invoke(attr);
        });
    }

    public static RenderTreeBuilder ColGroup(this RenderTreeBuilder builder, params int?[] widths)
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
        return builder.Markup(sb.ToString());
    }

    public static RenderTreeBuilder ColGroup(this RenderTreeBuilder builder, params string[] widths)
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
        return builder.Markup(sb.ToString());
    }

    public static RenderTreeBuilder THead(this RenderTreeBuilder builder, Action<AttributeBuilder> child = null) => builder.Element("thead", attr => child?.Invoke(attr));
    public static RenderTreeBuilder TBody(this RenderTreeBuilder builder, Action<AttributeBuilder> child = null) => builder.Element("tbody", attr => child?.Invoke(attr));
    public static RenderTreeBuilder TFoot(this RenderTreeBuilder builder, Action<AttributeBuilder> child = null) => builder.Element("tfoot", attr => child?.Invoke(attr));
    public static RenderTreeBuilder Tr(this RenderTreeBuilder builder, Action<AttributeBuilder> child = null) => builder.Element("tr", attr => child?.Invoke(attr));

    public static RenderTreeBuilder Tr(this RenderTreeBuilder builder, string className, Action<AttributeBuilder> child = null)
    {
        return builder.Tr(attr =>
        {
            attr.Class(className);
            child?.Invoke(attr);
        });
    }

    public static RenderTreeBuilder Th(this RenderTreeBuilder builder, Action<AttributeBuilder> child = null) => builder.Element("th", attr => child?.Invoke(attr));

    public static RenderTreeBuilder Th(this RenderTreeBuilder builder, string className, Action<AttributeBuilder> child = null)
    {
        return builder.Th(attr =>
        {
            attr.Class(className);
            child?.Invoke(attr);
        });
    }

    public static RenderTreeBuilder Th(this RenderTreeBuilder builder, string className, string text, int? rowSpan = null, int? colSpan = null)
    {
        return builder.Th(attr =>
        {
            attr.Class(className);
            if (rowSpan != null && rowSpan.Value > 0)
                attr.RowSpan(rowSpan.Value);
            if (colSpan != null && colSpan.Value > 0)
                attr.ColSpan(colSpan.Value);
            builder.Text(text);
        });
    }

    public static RenderTreeBuilder Td(this RenderTreeBuilder builder, Action<AttributeBuilder> child = null) => builder.Element("td", attr => child?.Invoke(attr));

    public static RenderTreeBuilder Td(this RenderTreeBuilder builder, string className, Action<AttributeBuilder> child = null, int? colSpan = null)
    {
        return builder.Td(attr =>
        {
            attr.Class(className);
			if (colSpan != null && colSpan.HasValue)
				attr.ColSpan(colSpan.Value);
			child?.Invoke(attr);
        });
    }

    public static RenderTreeBuilder Td(this RenderTreeBuilder builder, string className, string text, int? colSpan = null)
    {
        return builder.Td(attr =>
        {
            if (colSpan != null && colSpan.HasValue)
                attr.ColSpan(colSpan.Value);

            attr.Class(className);
            builder.Span("text", text);
        });
    }

    public static RenderTreeBuilder ThTd(this RenderTreeBuilder builder, string label, string value, int? colSpan = null)
    {
        builder.Th(attr => builder.Label("", label));
        builder.Td(attr =>
        {
            if (colSpan != null && colSpan.HasValue)
                attr.ColSpan(colSpan.Value);

            builder.Div("form-input", attr => builder.Span("text", value));
        });
        return builder;
    }

    public static RenderTreeBuilder Ul(this RenderTreeBuilder builder, Action<AttributeBuilder> child = null) => builder.Element("ul", attr => child?.Invoke(attr));

    public static RenderTreeBuilder Ul(this RenderTreeBuilder builder, string className, Action<AttributeBuilder> child = null)
    {
        return builder.Ul(attr =>
        {
            attr.Class(className);
            child?.Invoke(attr);
        });
    }

    public static RenderTreeBuilder Li(this RenderTreeBuilder builder, Action<AttributeBuilder> child = null) => builder.Element("li", attr => child?.Invoke(attr));

    public static RenderTreeBuilder Li(this RenderTreeBuilder builder, string className, Action<AttributeBuilder> child = null)
    {
        return builder.Li(attr =>
        {
            attr.Class(className);
            child?.Invoke(attr);
        });
    }

    public static RenderTreeBuilder Dl(this RenderTreeBuilder builder, Action<AttributeBuilder> child = null) => builder.Element("dl", attr => child?.Invoke(attr));

    public static RenderTreeBuilder Dl(this RenderTreeBuilder builder, string className, Action<AttributeBuilder> child = null)
    {
        return builder.Dl(attr =>
        {
            attr.Class(className);
            child?.Invoke(attr);
        });
    }

    public static RenderTreeBuilder Dd(this RenderTreeBuilder builder, Action<AttributeBuilder> child = null) => builder.Element("dd", attr => child?.Invoke(attr));

    public static RenderTreeBuilder Dd(this RenderTreeBuilder builder, string className, Action<AttributeBuilder> child = null)
    {
        return builder.Dd(attr =>
        {
            attr.Class(className);
            child?.Invoke(attr);
        });
    }

    public static RenderTreeBuilder Pre(this RenderTreeBuilder builder, string text) => builder.Element("pre", attr => builder.Text(text));
    public static RenderTreeBuilder Pre(this RenderTreeBuilder builder, string className, string text)
    {
        return builder.Element("pre", attr =>
        {
            attr.Class(className);
            builder.Text(text);
        });
    }
    
    public static RenderTreeBuilder Span(this RenderTreeBuilder builder, Action<AttributeBuilder> child = null) => builder.Element("span", attr => child?.Invoke(attr));

    public static RenderTreeBuilder Span(this RenderTreeBuilder builder, string className, Action<AttributeBuilder> child)
    {
        return builder.Span(attr =>
        {
            attr.Class(className);
            child.Invoke(attr);
        });
    }

    public static RenderTreeBuilder Span(this RenderTreeBuilder builder, string text) => builder.Span(attr => builder.Text(text));

    public static RenderTreeBuilder Span(this RenderTreeBuilder builder, string className, string text, EventCallback? onClick = null)
    {
        return builder.Span(attr =>
        {
            attr.Class(className).OnClick(onClick);
            builder.Text(text);
        });
    }

    public static RenderTreeBuilder Icon(this RenderTreeBuilder builder, Action<AttributeBuilder> child = null) => builder.Element("i", attr => child?.Invoke(attr));

    public static RenderTreeBuilder Icon(this RenderTreeBuilder builder, string icon, Action<AttributeBuilder> child = null)
    {
        return builder.Icon(attr =>
        {
            attr.Class(icon);
            child?.Invoke(attr);
        });
    }

    public static RenderTreeBuilder Label(this RenderTreeBuilder builder, Action<AttributeBuilder> child = null) => builder.Element("label", attr => child?.Invoke(attr));

    public static RenderTreeBuilder Label(this RenderTreeBuilder builder, string className, Action<AttributeBuilder> child = null)
    {
        return builder.Label(attr =>
        {
            attr.Class(className);
            child?.Invoke(attr);
        });
    }

    public static RenderTreeBuilder Label(this RenderTreeBuilder builder, string className, string text) => builder.Label(className, attr => builder.Text(text));
    public static RenderTreeBuilder Input(this RenderTreeBuilder builder, Action<AttributeBuilder> child = null) => builder.Element("input", attr => child?.Invoke(attr));

    public static RenderTreeBuilder Check(this RenderTreeBuilder builder, Action<AttributeBuilder> child = null)
    {
        return builder.Input(attr =>
        {
            attr.Type("checkbox");
            child?.Invoke(attr);
        });
    }

    public static RenderTreeBuilder Radio(this RenderTreeBuilder builder, Action<AttributeBuilder> child = null)
    {
        return builder.Input(attr =>
        {
            attr.Type("radio");
            child?.Invoke(attr);
        });
    }

    public static RenderTreeBuilder TextArea(this RenderTreeBuilder builder, Action<AttributeBuilder> child = null) => builder.Element("textarea", attr => child?.Invoke(attr));
    public static RenderTreeBuilder Select(this RenderTreeBuilder builder, Action<AttributeBuilder> child = null) => builder.Element("select", attr => child?.Invoke(attr));
    public static RenderTreeBuilder Option(this RenderTreeBuilder builder, Action<AttributeBuilder> child = null) => builder.Element("option", attr => child?.Invoke(attr));
    public static RenderTreeBuilder Img(this RenderTreeBuilder builder, Action<AttributeBuilder> child = null) => builder.Element("img", attr => child?.Invoke(attr));

    public static void Link(this RenderTreeBuilder builder, string text, EventCallback onClick, string style = null)
    {
        builder.Span($"link {style}", attr =>
        {
            attr.OnClick(onClick);
            builder.Text(text);
        });
    }
}