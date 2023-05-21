namespace Known.Razor;

partial class UIService
{
    public async void Tips(string message)
    {
        await InvokeAsync<string>("KRazor.showTips", message);
    }

    public void Alert(string message, bool isMax)
    {
        var tuple = GetMessage(message);
        Show(new DialogOption
        {
            IsMaxButton = isMax,
            Title = Language.AlertTips,
            Size = tuple.Item1,
            Body = builder => BuildMessage(builder, tuple.Item2),
            Foot = builder => BuildButton(builder, null)
        });
    }

    public void Alert(string message, Size? size = null, Action action = null)
    {
        var tuple = GetMessage(message);
        Show(new DialogOption
        {
            Title = Language.AlertTips,
            Size = size ?? tuple.Item1,
            Body = builder => BuildMessage(builder, tuple.Item2),
            Foot = builder => BuildButton(builder, action)
        });
    }

    public void Confirm(string message, Action action, bool isTop = false)
    {
        var tuple = GetMessage(message);
        Show(new DialogOption
        {
            Title = Language.AlertConfirm,
            Size = tuple.Item1,
            Body = builder => BuildMessage(builder, tuple.Item2),
            Foot = builder => BuildButton(builder, action, true),
            FootStyle = "confirm"
        }, isTop);
    }

    public void Result(Result result, bool back)
    {
        if (back)
            Result(result, Back);
        else
            Result(result);
    }

    public void Result(Result result, Action action = null)
    {
        if (!result.IsValid)
        {
            Alert(result.Message, result.Size);
            return;
        }

        action?.Invoke();
        Tips(result.Message);
    }

    private static void BuildMessage(RenderTreeBuilder builder, string message)
    {
        builder.Div("message", attr => builder.Markup(message));
    }

    private void BuildButton(RenderTreeBuilder builder, Action action, bool cancel = false)
    {
        builder.Button(FormButton.OK, EventCallback.Factory.Create(this, () =>
        {
            CloseDialog();
            action?.Invoke();
        }));
        if (cancel)
        {
            builder.Button(FormButton.Cancel, EventCallback.Factory.Create(this, CloseDialog));
        }
    }

    private static Tuple<Size, string> GetMessage(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
            return Tuple.Create(new Size(380, 200), message);

        var lines = message.Split(Environment.NewLine);
        if (lines.Length == 1)
            return Tuple.Create(new Size(380, 200), message);

        var text = message.Replace(Environment.NewLine, "<br/>");
        var length = lines.Length > 10 ? 10 : lines.Length;
        var height = length * 30 + 150;
        return Tuple.Create(new Size(380, height), text);
    }
}