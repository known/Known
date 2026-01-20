using AntDesign;

namespace Known.Extensions;

/// <summary>
/// 组件建造者扩展类。
/// </summary>
public static class BuilderExtension
{
    /// <summary>
    /// 创建HTML元素建造者。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="elementName">HTML元素名称。</param>
    /// <returns>HTML元素建造者。</returns>
    public static ElementBuilder Element(this RenderTreeBuilder builder, string elementName)
    {
        return new ElementBuilder(builder, elementName);
    }

    /// <summary>
    /// 创建组件建造者。
    /// </summary>
    /// <typeparam name="T">组件类型。</typeparam>
    /// <param name="builder">呈现树建造者。</param>
    /// <returns>组件建造者。</returns>
    public static ComponentBuilder<T> Component<T>(this RenderTreeBuilder builder) where T : notnull, Microsoft.AspNetCore.Components.IComponent
    {
        return new ComponentBuilder<T>(builder);
    }

    /// <summary>
    /// 创建水印组件。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="texts">水印文字。</param>
    /// <param name="content">组件内容。</param>
    public static void Watermark(this RenderTreeBuilder builder, string[] texts, RenderFragment content)
    {
        builder.Component<Watermark>()
               .Set(c => c.Contents, texts)
               .Set(c => c.ChildContent, content)
               .Build();
    }

    internal static void BuildBody(this RenderTreeBuilder builder, Context context, RenderFragment body)
    {
        if (Config.System?.IsWatermark == true && context != null)
        {
            var user = context.CurrentUser;
            var type = Utils.ConvertTo<WatermarkType>(Config.System.Watermark);
            var text = type switch
            {
                WatermarkType.Account => user.UserName,
                WatermarkType.Name => user.Name,
                _ => $"{user?.Name}({user?.UserName})",
            };
            var texts = new List<string> { text };
            if (!string.IsNullOrWhiteSpace(Config.System.WmDateFormat))
                texts.Add(DateTime.Now.ToString(Config.System.WmDateFormat));
            //var watermark = user?.Watermark ?? text;
            builder.Div("kui-watermark", () => builder.Watermark([.. texts], body));
        }
        else
        {
            builder.Fragment(body);
        }
    }

    internal static void BuildTable(this RenderTreeBuilder builder, string width, bool isForm, Action child)
    {
        var className = CssBuilder.Default("kui-table").AddClass("form-list", isForm).BuildClass();
        var style = CssBuilder.Default().Add("width", width).BuildStyle();
        builder.Div().Class(className).Style(style).Child(child);
    }
}