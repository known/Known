using Known.Extensions;

namespace Known.Razor;

partial class UIService
{
    public void ShowLoading(string message) => InvokeVoidAsync("KRazor.showLoading", message);
    public void CloseLoading() => InvokeVoidAsync("KRazor.closeLoading");

    public void Notify(string message, StyleType style = StyleType.Success, int timeout = 5000)
    {
        InvokeVoidAsync("KRazor.showNotify", message, style.ToString().ToLower(), timeout);
    }
    
    public void Toast(string message, StyleType style = StyleType.Success)
    {
        InvokeVoidAsync("KRazor.showToast", message, style.ToString().ToLower());
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
        }, true);
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
        }, true);
    }

    public void Confirm(string message, Action action, bool isTop = true)
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
        Toast(result.Message);
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