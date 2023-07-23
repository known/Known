namespace Known.Razor.Components;

public class DialogOption
{
    public bool IsMax { get; set; }
    public bool IsMaxButton { get; set; }
    public Size? Size { get; set; } = new Size(500, 300);
    public string Title { get; set; }
    public string HeadStyle { get; set; }
    public string ContentStyle { get; set; }
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
    private readonly Dictionary<string, DialogOption> options = new();
    private readonly Dictionary<string, Dialog> dialogs = new();

    public DialogContainer()
    {
        Id = "top";
    }

    internal void Show(DialogOption option)
    {
        if (string.IsNullOrWhiteSpace(option.Title))
            return;

        option.Previous = current;
        if (!options.ContainsKey(option.Title))
        {
            current = option.Title;
            options[current] = option;
            StateChanged();
        }
    }

    internal async void Close()
    {
        if (string.IsNullOrWhiteSpace(current))
            return;

        if (options.ContainsKey(current))
        {
            var previous = options[current].Previous;
            await dialogs[current].DisposeAsync();
            dialogs.Remove(current);
            options.Remove(current);
            current = previous;
            StateChanged();
        }
    }

    protected override void OnInitialized()
    {
        if (Id == "top")
        {
            UI.TopDialog = this;
        }
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        var index = 1;
        foreach (var item in options)
        {
            var option = item.Value;
            builder.Component<Dialog>()
                   .Set(c => c.StartIndex, Id == "top" ? 10000 : 1000)
                   .Set(c => c.Index, index++)
                   .Set(c => c.Option, option)
                   .Set(c => c.OnClose, Close)
                   .Build(async value =>
                   {
                       if (dialogs.ContainsKey(option.Title))
                       {
                           await dialogs[option.Title].DisposeAsync();
                       }
                       dialogs[option.Title] = value;
                   });
        }
    }

    protected override async ValueTask DisposeAsync(bool disposing)
    {
        if (disposing)
        {
            foreach (var item in dialogs)
            {
                await item.Value.DisposeAsync();
            }
            dialogs.Clear();
            options.Clear();
        }
        await base.DisposeAsync(disposing);
    }
}

class Dialog : BaseComponent
{
    private readonly string dialogId;

    public Dialog()
    {
        dialogId = "dailog-" + Utils.GetGuid();
    }

    [Parameter] public int StartIndex { get; set; } = 1000;
    [Parameter] public int Index { get; set; }
    [Parameter] public DialogOption Option { get; set; }
    [Parameter] public Action OnClose { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        var isMax = Option.IsMax;
        builder.Div("mask", attr => attr.Id($"mask-{dialogId}").Style($"z-index:{StartIndex + Index}"));
        var css = CssBuilder.Default("dialog animated fadeIn")
                            .AddClass("max", isMax)
                            .AddClass("hasFoot", Option.Content == null)
                            .Build();
        builder.Div(css, attr =>
        {
            attr.Id(dialogId).Style(GetStyle(isMax));
            BuildHead(builder, isMax);
            BuildContent(builder);
        });
    }

    protected override void OnAfterRender(bool firstRender)
    {
        if (!Option.IsMax)
        {
            UI.SetDialogMove(dialogId);
        }
        Option.OnShow?.Invoke(firstRender);
    }

    private void BuildHead(RenderTreeBuilder builder, bool isMax)
    {
        if (!string.IsNullOrWhiteSpace(Option.Title))
        {
            var css = CssBuilder.Default("dlg-head").AddClass("draggable", !isMax).AddClass(Option.HeadStyle).Build();
            builder.Div(css, attr =>
            {
                builder.Span("title", Option.Title);
                BuildHeadTools(builder, isMax);
            });
        }
        else
        {
            BuildClose(builder);
        }
    }

    private void BuildHeadTools(RenderTreeBuilder builder, bool isMax)
    {
        builder.Div("btns", attr =>
        {
            if (Option.IsMaxButton)
            {
                var className = isMax ? "fa-window-restore" : "fa-window-maximize";
                var title = isMax ? "恢复" : "最大化";
                builder.Span($"bmax fa {className}", attr =>
                {
                    attr.Title(title).OnClick(Callback(e => Option.IsMax = !Option.IsMax));
                });
            }

            BuildClose(builder);
        });
    }

    private void BuildClose(RenderTreeBuilder builder)
    {
        builder.Span("close fa fa-close", attr =>
        {
            attr.Title("关闭").OnClick(Callback(OnClose));
        });
    }

    private void BuildContent(RenderTreeBuilder builder)
    {
        if (Option.Content != null)
        {
            var css = CssBuilder.Default("dlg-content").AddClass(Option.ContentStyle).Build();
            builder.Div(css, attr => builder.Fragment(Option.Content));
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

        var css = CssBuilder.Default("dlg-body").AddClass(Option.BodyStyle).Build();
        builder.Div(css, attr => builder.Fragment(Option.Body));
    }

    private void BuildFoot(RenderTreeBuilder builder)
    {
        if (Option.Foot == null)
            return;

        var css = CssBuilder.Default("dlg-foot").AddClass(Option.FootStyle).Build();
        builder.Div(css, attr => builder.Fragment(Option.Foot));
    }

    private string GetStyle(bool isMax)
    {
        var size = Option.Size;
        var sb = new StyleBuilder();
        if (!isMax)
        {
            sb.Add("width", $"{size?.Width}px")
              .Add("height", $"{size?.Height}px")
              .Add("margin-top", $"-{size?.Height / 2}px")
              .Add("margin-left", $"-{size?.Width / 2}px");
        }
        sb.Add("z-index", $"{StartIndex + Index + 1}");

        var rndColor = StyleExtension.GetRandomColor();
        if (!string.IsNullOrWhiteSpace(rndColor))
            sb.Add("border-top-color", rndColor);

        return sb.Build();
    }
}