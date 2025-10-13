using AntDesign;

namespace Known.Blazor;

public partial class UIService
{
    /// <summary>
    /// 弹出吐司组件提示消息框。
    /// </summary>
    /// <param name="text">提示消息文本。</param>
    /// <param name="style">提示样式，默认Success。</param>
    /// <returns></returns>
    internal Task ToastAsync(string text, StyleType style = StyleType.Success)
    {
        if (string.IsNullOrWhiteSpace(text))
            return Task.CompletedTask;

        var content = FormatMessage(text);
        return style switch
        {
            StyleType.Success => message.SuccessAsync(content),
            StyleType.Info => message.InfoAsync(content),
            StyleType.Warning => message.WarningAsync(content),
            StyleType.Error => message.ErrorAsync(content),
            _ => message.InfoAsync(content),
        };
    }

    /// <summary>
    /// 弹出通知组件提示消息框。
    /// </summary>
    /// <param name="title">通知标题。</param>
    /// <param name="text">提示消息文本。</param>
    /// <param name="style">提示样式，默认Success。</param>
    /// <returns></returns>
    public Task NoticeAsync(string title, string text, StyleType style = StyleType.Success)
    {
        if (string.IsNullOrWhiteSpace(text))
            return Task.CompletedTask;

        var config = new NotificationConfig
        {
            Message = title,
            ClassName = style.ToString(),
            Description = FormatMessage(text),
            Placement = NotificationPlacement.BottomRight
        };
        if (style == StyleType.Error)
            config.Duration = 60;
        return style switch
        {
            StyleType.Success => notice.Success(config),
            StyleType.Info => notice.Info(config),
            StyleType.Warning => notice.Warning(config),
            StyleType.Error => notice.Error(config),
            _ => notice.Info(config),
        };
    }

    /// <summary>
    /// 弹出消息提示框组件。
    /// </summary>
    /// <param name="text">提示消息文本。</param>
    /// <param name="action">点【确定】按钮的回调方法。</param>
    public bool Alert(string text, Func<Task> action = null)
    {
        var options = new ConfirmOptions
        {
            Title = Language[Language.Prompt],
            Content = FormatMessage(text)
        };
        if (action != null)
            options.OnOk = e => action?.Invoke();
        modal.Info(options);
        return true;
    }

    /// <summary>
    /// 弹出确认消息提示框组件。
    /// </summary>
    /// <param name="text">确认消息文本。</param>
    /// <param name="action">点【确定】按钮的回调方法。</param>
    public bool Confirm(string text, Func<Task> action)
    {
        var options = new ConfirmOptions
        {
            Title = Language[Language.Question],
            Icon = b => b.Icon("question-circle"),
            Content = FormatMessage(text)
        };
        if (action != null)
            options.OnOk = e => action?.Invoke();
        modal.Confirm(options);
        return true;
    }

    private RenderFragment FormatMessage(string text)
    {
        if (!string.IsNullOrWhiteSpace(text))
        {
            text = Language[text];
            text = text.Trim([.. Environment.NewLine]);
            if (text.Contains(Environment.NewLine))
            {
                text = text.Replace(Environment.NewLine, "<br/>");
                text = $"<div class=\"message\">{text}</div>";
            }
        }
        return b => b.Markup(text);
    }
}