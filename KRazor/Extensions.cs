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

using System.Text;
using Known.Core;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.DependencyInjection;

namespace Known.Razor;

public static class DIExtension
{
    public static IServiceCollection AddKBlazor(this IServiceCollection services)
    {
        services.AddScoped<HttpClient>();
        services.AddScoped<PlatformService>();
        services.AddScoped<SystemService>();
        services.AddScoped<UIService>();
        return services;
    }
}

public static class ElementExtension
{
    public static RenderTreeBuilder Element(this RenderTreeBuilder builder, string name, Action<AttributeBuilder> child = null)
    {
        builder.OpenElement(0, name);
        var attr = new AttributeBuilder(builder);
        child?.Invoke(attr);
        builder.CloseElement();
        return builder;
    }

    public static RenderTreeBuilder Component<TComponent>(this RenderTreeBuilder builder, Action<AttributeBuilder> child = null) where TComponent : notnull, IComponent
    {
        builder.OpenComponent<TComponent>(0);
        var attr = new AttributeBuilder(builder);
        child?.Invoke(attr);
        builder.CloseComponent();
        return builder;
    }

    public static RenderTreeBuilder Reference<TComponent>(this RenderTreeBuilder builder, TComponent value) where TComponent : notnull, IComponent
    {
        builder.AddComponentReferenceCapture(0, v =>
        {
            value = (TComponent)v;
        });
        return builder;
    }

    public static RenderTreeBuilder Reference<TComponent>(this RenderTreeBuilder builder, Action<TComponent> action) where TComponent : notnull, IComponent
    {
        builder.AddComponentReferenceCapture(0, value =>
        {
            action.Invoke((TComponent)value);
        });
        return builder;
    }

    public static RenderTreeBuilder Fragment(this RenderTreeBuilder builder, RenderFragment fragment)
    {
        if (fragment != null)
            builder.AddContent(0, fragment);

        return builder;
    }

    public static RenderTreeBuilder Fragment<TValue>(this RenderTreeBuilder builder, RenderFragment<TValue> fragment, TValue value)
    {
        if (fragment != null)
            builder.AddContent(0, fragment, value);

        return builder;
    }

    public static RenderTreeBuilder Markup(this RenderTreeBuilder builder, string markup)
    {
        if (!string.IsNullOrWhiteSpace(markup))
            builder.AddMarkupContent(0, markup);

        return builder;
    }

    public static RenderTreeBuilder Text(this RenderTreeBuilder builder, string text)
    {
        if (!string.IsNullOrWhiteSpace(text))
            builder.AddContent(0, text);

        return builder;
    }

    public static RenderTreeBuilder Div(this RenderTreeBuilder builder, Action<AttributeBuilder> child = null)
    {
        return builder.Element("div", attr => child?.Invoke(attr));
    }

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

    public static RenderTreeBuilder P(this RenderTreeBuilder builder, Action<AttributeBuilder> child = null)
    {
        return builder.Element("p", attr => child?.Invoke(attr));
    }

    public static RenderTreeBuilder P(this RenderTreeBuilder builder, string className, Action<AttributeBuilder> child = null)
    {
        return builder.P(attr =>
        {
            attr.Class(className);
            child?.Invoke(attr);
        });
    }

    public static RenderTreeBuilder Table(this RenderTreeBuilder builder, Action<AttributeBuilder> child = null)
    {
        return builder.Element("table", attr => child?.Invoke(attr));
    }

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
                sb.Append($"<col style=\"width:{item.Value}px\" />");
            else
                sb.Append("<col />");
        }
        sb.Append("</colgroup>");
        return builder.Markup(sb.ToString());
    }

    public static RenderTreeBuilder Tr(this RenderTreeBuilder builder, Action<AttributeBuilder> child = null)
    {
        return builder.Element("tr", attr => child?.Invoke(attr));
    }

    public static RenderTreeBuilder Tr(this RenderTreeBuilder builder, string className, Action<AttributeBuilder> child = null)
    {
        return builder.Tr(attr =>
        {
            attr.Class(className);
            child?.Invoke(attr);
        });
    }

    public static RenderTreeBuilder Th(this RenderTreeBuilder builder, Action<AttributeBuilder> child = null)
    {
        return builder.Element("th", attr => child?.Invoke(attr));
    }

    public static RenderTreeBuilder Th(this RenderTreeBuilder builder, string className, Action<AttributeBuilder> child = null)
    {
        return builder.Th(attr =>
        {
            attr.Class(className);
            child?.Invoke(attr);
        });
    }

    public static RenderTreeBuilder Td(this RenderTreeBuilder builder, Action<AttributeBuilder> child = null)
    {
        return builder.Element("td", attr => child?.Invoke(attr));
    }

    public static RenderTreeBuilder Td(this RenderTreeBuilder builder, string className, Action<AttributeBuilder> child = null)
    {
        return builder.Td(attr =>
        {
            attr.Class(className);
            child?.Invoke(attr);
        });
    }

    public static RenderTreeBuilder Ul(this RenderTreeBuilder builder, Action<AttributeBuilder> child = null)
    {
        return builder.Element("ul", attr => child?.Invoke(attr));
    }

    public static RenderTreeBuilder Ul(this RenderTreeBuilder builder, string className, Action<AttributeBuilder> child = null)
    {
        return builder.Ul(attr =>
        {
            attr.Class(className);
            child?.Invoke(attr);
        });
    }

    public static RenderTreeBuilder Li(this RenderTreeBuilder builder, Action<AttributeBuilder> child = null)
    {
        return builder.Element("li", attr => child?.Invoke(attr));
    }

    public static RenderTreeBuilder Li(this RenderTreeBuilder builder, string className, Action<AttributeBuilder> child = null)
    {
        return builder.Li(attr =>
        {
            attr.Class(className);
            child?.Invoke(attr);
        });
    }

    public static RenderTreeBuilder Span(this RenderTreeBuilder builder, Action<AttributeBuilder> child = null)
    {
        return builder.Element("span", attr => child?.Invoke(attr));
    }

    public static RenderTreeBuilder Span(this RenderTreeBuilder builder, string className, Action<AttributeBuilder> child)
    {
        return builder.Span(attr =>
        {
            attr.Class(className);
            child.Invoke(attr);
        });
    }

    public static RenderTreeBuilder Span(this RenderTreeBuilder builder, string text)
    {
        return builder.Span(attr => builder.Text(text));
    }

    public static RenderTreeBuilder Span(this RenderTreeBuilder builder, string className, string text, EventCallback? onClick = null)
    {
        return builder.Span(attr =>
        {
            attr.Class(className).OnClick(onClick);
            builder.Text(text);
        });
    }

    public static RenderTreeBuilder Icon(this RenderTreeBuilder builder, Action<AttributeBuilder> child = null)
    {
        return builder.Element("i", attr => child?.Invoke(attr));
    }

    public static RenderTreeBuilder Icon(this RenderTreeBuilder builder, string icon, Action<AttributeBuilder> child = null)
    {
        return builder.Icon(attr =>
        {
            attr.Class($"icon {icon}");
            child?.Invoke(attr);
        });
    }

    public static RenderTreeBuilder Label(this RenderTreeBuilder builder, Action<AttributeBuilder> child = null)
    {
        return builder.Element("label", attr => child?.Invoke(attr));
    }

    public static RenderTreeBuilder Label(this RenderTreeBuilder builder, string className, Action<AttributeBuilder> child = null)
    {
        return builder.Label(attr =>
        {
            attr.Class(className);
            child?.Invoke(attr);
        });
    }

    public static RenderTreeBuilder Label(this RenderTreeBuilder builder, string className, string text)
    {
        return builder.Label(className, attr => builder.Text(text));
    }

    public static RenderTreeBuilder Button(this RenderTreeBuilder builder, Action<AttributeBuilder> child = null)
    {
        return builder.Element("button", attr => child?.Invoke(attr));
    }

    public static RenderTreeBuilder Button(this RenderTreeBuilder builder, string className, Action<AttributeBuilder> child = null)
    {
        return builder.Button(attr =>
        {
            attr.Class(className);
            child?.Invoke(attr);
        });
    }

    public static RenderTreeBuilder Input(this RenderTreeBuilder builder, Action<AttributeBuilder> child = null)
    {
        return builder.Element("input", attr => child?.Invoke(attr));
    }

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

    public static RenderTreeBuilder TextArea(this RenderTreeBuilder builder, Action<AttributeBuilder> child = null)
    {
        return builder.Element("textarea", attr => child?.Invoke(attr));
    }

    public static RenderTreeBuilder Select(this RenderTreeBuilder builder, Action<AttributeBuilder> child = null)
    {
        return builder.Element("select", attr => child?.Invoke(attr));
    }

    public static RenderTreeBuilder Option(this RenderTreeBuilder builder, Action<AttributeBuilder> child = null)
    {
        return builder.Element("option", attr => child?.Invoke(attr));
    }

    public static RenderTreeBuilder Img(this RenderTreeBuilder builder, Action<AttributeBuilder> child = null)
    {
        return builder.Element("img", attr => child?.Invoke(attr));
    }

    public static void Link(this RenderTreeBuilder builder, string text, EventCallback onClick)
    {
        builder.Span("link", attr =>
        {
            attr.OnClick(onClick);
            builder.Text(text);
        });
    }

    public static void Button(this RenderTreeBuilder builder, string text, EventCallback onClick, string style = null)
    {
        builder.Button(text, "", onClick, style);
    }

    public static void Button(this RenderTreeBuilder builder, string text, string icon, EventCallback onClick, string style = null)
    {
        builder.Component<Button>(attr =>
        {
            attr.Add(nameof(Razor.Button.Style), style)
                .Add(nameof(Razor.Button.Icon), icon)
                .Add(nameof(Razor.Button.Text), text)
                .Add(nameof(Razor.Button.OnClick), onClick);
        });
    }

    public static void ButtonOK(this RenderTreeBuilder builder, EventCallback onClick)
    {
        builder.Button(Language.OK, "fa fa-check", onClick, "primary");
    }

    public static void ButtonCancel(this RenderTreeBuilder builder, EventCallback onClick)
    {
        builder.Button(Language.Cancel, "fa fa-close", onClick, "danger");
    }

    public static void ButtonAdd(this RenderTreeBuilder builder, EventCallback onClick, string style = "right")
    {
        builder.Button(Language.Add, "fa fa-plus", onClick, style);
    }

    public static void ButtonNew(this RenderTreeBuilder builder, EventCallback onClick)
    {
        builder.Button(Language.New, "fa fa-plus", onClick);
    }

    public static void ButtonQuery(this RenderTreeBuilder builder, EventCallback onClick)
    {
        builder.Button(Language.Query, "fa fa-search", onClick);
    }

    public static void ButtonSave(this RenderTreeBuilder builder, EventCallback onClick)
    {
        builder.Button(Language.Save, "fa fa-save", onClick);
    }

    public static void ButtonClose(this RenderTreeBuilder builder, EventCallback onClick)
    {
        builder.Button(Language.Close, "fa fa-close", onClick, "danger");
    }

    public static void FormTitle(this RenderTreeBuilder builder, string title)
    {
        builder.Div("form-title", attr => builder.Text(title));
    }

    public static void FormItem(this RenderTreeBuilder builder, Action<RenderTreeBuilder> action)
    {
        builder.Div("form-item", attr =>
        {
            builder.Label("form-label");
            action(builder);
        });
    }

    public static void FormCaption(this RenderTreeBuilder builder, string text, int colSpan)
    {
        builder.Td("form-caption", attr =>
        {
            attr.ColSpan(colSpan);
            builder.Text(text);
        });
    }

    public static void Hidden(this RenderTreeBuilder builder, string id)
    {
        builder.Component<Hidden>(attr => attr.Add(nameof(Razor.Hidden.Id), id));
    }

    public static void Hidden(this RenderTreeBuilder builder, string id, string value)
    {
        builder.Component<Hidden>(attr =>
        {
            attr.Add(nameof(Razor.Hidden.Id), id)
                .Add(nameof(Razor.Hidden.Value), value);
        });
    }

    public static void Action(this RenderTreeBuilder builder, int width, RenderFragment<object> child)
    {
        builder.Field<Field>(fb => fb.Field("操作", "Action").Width(width).ChildContent(child));
    }

    public static void Field<T>(this RenderTreeBuilder builder, Action<FieldBuilder> action, Action<AttributeBuilder> action1 = null) where T : Field
    {
        builder.Component<T>(attr =>
        {
            action1?.Invoke(attr);
            var fb = new FieldBuilder(attr);
            action?.Invoke(fb);
        });
    }

    public static void Field<T>(this RenderTreeBuilder builder, string label, string id, bool isQuery = false, bool isDateQuery = false) where T : Field
    {
        builder.Field<T>(fb =>
        {
            fb.Field(label, id).IsQuery(isQuery);
            if (isDateQuery)
            {
                fb.IsRangeQuery(false);
            }
        });
    }

    public static void Field<T>(this RenderTreeBuilder builder, string label, string id, Action<RenderTreeBuilder, object> action) where T : Field
    {
        builder.Field<T>(fb => fb.Field(label, id).ChildContent((object row) => delegate (RenderTreeBuilder builder1) { action(builder1, row); }));
    }

    public static void Field<T>(this RenderTreeBuilder builder, string label, string id, string unit) where T : Field
    {
        builder.Field<T>(fb => fb.Field(label, id).Unit(unit));
    }
}
