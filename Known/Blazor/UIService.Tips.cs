using AntDesign;

namespace Known.Blazor;

public partial class UIService
{
    /// <summary>
    /// 弹出吐司组件提示消息框。
    /// </summary>
    /// <param name="message">提示消息文本。</param>
    /// <param name="style">提示样式，默认Success。</param>
    /// <returns></returns>
    public async Task Toast(string message, StyleType style = StyleType.Success)
    {
        var content = FormatMessage(message);
        switch (style)
        {
            case StyleType.Success:
                await _message.Success(content);
                break;
            case StyleType.Info:
                await _message.Info(content);
                break;
            case StyleType.Warning:
                await _message.Warning(content);
                break;
            case StyleType.Error:
                await _message.Error(content);
                break;
            default:
                await _message.Info(content);
                break;
        }
    }

    /// <summary>
    /// 弹出通知组件提示消息框。
    /// </summary>
    /// <param name="title">通知标题。</param>
    /// <param name="message">提示消息文本。</param>
    /// <param name="style">提示样式，默认Success。</param>
    /// <returns></returns>
    public async Task NoticeAsync(string title, string message, StyleType style = StyleType.Success)
    {
        var config = new NotificationConfig
        {
            Message = title,
            ClassName = style.ToString(),
            Description = FormatMessage(message),
            Placement = NotificationPlacement.BottomRight
        };
        switch (style)
        {
            case StyleType.Success:
                await _notice.Success(config);
                break;
            case StyleType.Info:
                await _notice.Info(config);
                break;
            case StyleType.Warning:
                await _notice.Warning(config);
                break;
            case StyleType.Error:
                config.Duration = 1000;
                await _notice.Error(config);
                break;
            default:
                await _notice.Info(config);
                break;
        }
    }

    /// <summary>
    /// 弹出消息提示框组件。
    /// </summary>
    /// <param name="message">提示消息文本。</param>
    /// <param name="action">点【确定】按钮的回调方法。</param>
    public bool Alert(string message, Func<Task> action = null)
    {
        var options = new ConfirmOptions
        {
            Title = Language?.GetTitle("Prompt"),
            Content = FormatMessage(message)
        };
        if (action != null)
            options.OnOk = e => action?.Invoke();
        _modal.Info(options);
        return true;
    }

    /// <summary>
    /// 弹出确认消息提示框组件。
    /// </summary>
    /// <param name="message">确认消息文本。</param>
    /// <param name="action">点【确定】按钮的回调方法。</param>
    public bool Confirm(string message, Func<Task> action)
    {
        var options = new ConfirmOptions
        {
            Title = Language?.GetTitle("Question"),
            Icon = b => b.Icon("question-circle"),
            Content = FormatMessage(message)
        };
        if (action != null)
            options.OnOk = e => action?.Invoke();
        _modal.Confirm(options);
        return true;
    }

    private static RenderFragment FormatMessage(string message)
    {
        if (!string.IsNullOrWhiteSpace(message))
        {
            message = message.Trim([.. Environment.NewLine]);
            if (message.Contains(Environment.NewLine))
            {
                message = message.Replace(Environment.NewLine, "<br/>");
                message = $"<div class=\"message\">{message}</div>";
            }
        }
        return b => b.Markup(message);
    }
}