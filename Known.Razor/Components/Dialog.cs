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

using System.Drawing;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Razor;

public class DialogOption
{
    public bool IsTips { get; set; }
    public Size Size { get; set; } = new Size(500, 300);
    public string Title { get; set; }
    public string HeadStyle { get; set; }
    public string BodyStyle { get; set; }
    public string FootStyle { get; set; }
    public RenderFragment Content { get; set; }
    public RenderFragment Body { get; set; }
    public RenderFragment Foot { get; set; }
    public Action<bool> OnShow { get; set; }

    internal string Previous { get; set; }
}

public class DialogContainer : BaseComponent
{
    private string current;
    private DialogOption tips;
    private readonly Dictionary<string, DialogOption> dialogs = new();

    internal void ShowTips(string message)
    {
        tips = new DialogOption
        {
            IsTips = true,
            Content = builder => builder.Text(message)
        };
        StateHasChanged();
    }

    internal void CloseTips()
    {
        tips = null;
        StateHasChanged();
    }

    internal void Show(DialogOption option)
    {
        option.Previous = current;
        if (!string.IsNullOrWhiteSpace(option.Title) && !dialogs.ContainsKey(option.Title))
        {
            current = option.Title;
            dialogs[current] = option;
            StateHasChanged();
        }
    }

    public void Close()
    {
        if (string.IsNullOrWhiteSpace(current))
            return;

        if (dialogs.ContainsKey(current))
        {
            var previous = dialogs[current].Previous;
            dialogs.Remove(current);
            current = previous;
            StateHasChanged();
        }
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        if (UI != null)
        {
            UI.Register(this);
        }
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (tips != null)
        {
            builder.Component<Dialog>(attr =>
            {
                attr.Add(nameof(Dialog.Option), tips);
            });
        }

        foreach (var item in dialogs)
        {
            var option = item.Value;
            builder.Component<Dialog>(attr =>
            {
                attr.Add(nameof(Dialog.Option), option)
                    .Add(nameof(Dialog.OnClose), () => Close());
            });
        }
    }
}

public class Dialog : BaseComponent
{
    [Parameter] public DialogOption Option { get; set; }
    [Parameter] public Action OnClose { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (Option.IsTips)
            BuildTips(builder);
        else
            BuildDialog(builder);
    }

    protected override void OnAfterRender(bool firstRender)
    {
        base.OnAfterRender(firstRender);
        Option.OnShow?.Invoke(firstRender);
    }

    private void BuildTips(RenderTreeBuilder builder)
    {
        builder.Div("dlg-tips animated fadeInDown", attr =>
        {
            builder.Fragment(Option.Content);
        });
    }

    private void BuildDialog(RenderTreeBuilder builder)
    {
        builder.Div("mask");
        builder.Div("dialog", attr =>
        {
            attr.Style(GetStyle());
            BuildHead(builder);
            BuildContent(builder);
        });
    }

    private void BuildHead(RenderTreeBuilder builder)
    {
        if (!string.IsNullOrWhiteSpace(Option.Title))
        {
            builder.Div($"dlg-head {Option.HeadStyle}", attr =>
            {
                builder.Span(Option.Title);
                BuildClose(builder);
            });
        }
        else
        {
            BuildClose(builder);
        }
    }

    private void BuildClose(RenderTreeBuilder builder)
    {
        builder.Span("close fa fa-close", attr =>
        {
            attr.OnClick(Callback(e => OnClose()));
        });
    }

    private void BuildContent(RenderTreeBuilder builder)
    {
        if (Option.Content != null)
        {
            builder.Div("dlg-content", attr =>
            {
                builder.Fragment(Option.Content);
            });
        }
        else
        {
            BuildBody(builder);
            BuildFoot(builder);
        }
    }

    private void BuildBody(RenderTreeBuilder builder)
    {
        if (Option.Body == null)
            return;

        builder.Div($"dlg-body {Option.BodyStyle}", attr =>
        {
            builder.Fragment(Option.Body);
        });
    }

    private void BuildFoot(RenderTreeBuilder builder)
    {
        if (Option.Foot == null)
            return;

        builder.Div($"dlg-foot {Option.FootStyle}", attr =>
        {
            builder.Fragment(Option.Foot);
        });
    }

    private string GetStyle()
    {
        var size = Option.Size;
        return new StyleBuilder()
                .Add("width", $"{size.Width}px")
                .Add("height", $"{size.Height}px")
                .Add("margin-top", $"-{size.Height / 2}px")
                .Add("margin-left", $"-{size.Width / 2}px")
                .Build();
    }
}
