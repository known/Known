﻿using AntDesign;

namespace Known.Extensions;

/// <summary>
/// 按钮组件扩展类。
/// </summary>
public static class ButtonExtension
{
    /// <summary>
    /// 构建按钮组件。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="info">操作按钮信息对象。</param>
    public static void Button(this RenderTreeBuilder builder, ActionInfo info)
    {
        builder.Component<KButton>()
               .Set(c => c.Enabled, info.Enabled)
               .Set(c => c.Icon, info.Icon)
               .Set(c => c.Name, info.Name)
               .Set(c => c.Type, GetButtonType(info.Style))
               .Set(c => c.Danger, GetButtonDanger(info.Style))
               .Set(c => c.OnClick, info.OnClick)
               .Build();
    }

    /// <summary>
    /// 呈现一个按钮。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="info">操作信息。</param>
    /// <param name="onClick">按钮单击事件。</param>
    public static void Button(this RenderTreeBuilder builder, ActionInfo info, EventCallback<MouseEventArgs> onClick)
    {
        builder.Component<KButton>()
               .Set(c => c.Id, info.Id)
               .Set(c => c.Name, info.Name)
               .Set(c => c.Icon, info.Icon)
               .Set(c => c.Type, GetButtonType(info.Style))
               .Set(c => c.Danger, GetButtonDanger(info.Style))
               .Set(c => c.Enabled, info.Enabled)
               .Set(c => c.Visible, info.Visible)
               .Set(c => c.OnClick, onClick)
               .Build();
    }

    /// <summary>
    /// 呈现一个按钮。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="name">按钮名称。</param>
    /// <param name="onClick">按钮单击事件。</param>
    /// <param name="type">按钮样式，默认primary。</param>
    public static void Button(this RenderTreeBuilder builder, string name, EventCallback<MouseEventArgs> onClick, string type = "primary")
    {
        builder.Component<KButton>()
               .Set(c => c.Name, name)
               .Set(c => c.Type, GetButtonType(type))
               .Set(c => c.Danger, GetButtonDanger(type))
               .Set(c => c.OnClick, onClick)
               .Build();
    }

    internal static ButtonType GetButtonType(string style)
    {
        return style switch
        {
            "Default" => ButtonType.Default,
            "default" => ButtonType.Default,
            "Primary" => ButtonType.Primary,
            "primary" => ButtonType.Primary,
            "Dashed" => ButtonType.Dashed,
            "dashed" => ButtonType.Dashed,
            "Link" => ButtonType.Link,
            "link" => ButtonType.Link,
            "Text" => ButtonType.Text,
            "text" => ButtonType.Text,
            _ => ButtonType.Primary
        };
    }

    internal static bool GetButtonDanger(string style)
    {
        return style == "danger";
    }
}