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
    public async Task Toast(string text, StyleType style = StyleType.Success)
    {
        if (string.IsNullOrWhiteSpace(text))
            return;

        var content = FormatMessage(text);
        switch (style)
        {
            case StyleType.Success:
                await message.SuccessAsync(content);
                break;
            case StyleType.Info:
                await message.InfoAsync(content);
                break;
            case StyleType.Warning:
                await message.WarningAsync(content);
                break;
            case StyleType.Error:
                await message.ErrorAsync(content);
                break;
            default:
                await message.InfoAsync(content);
                break;
        }
    }

    /// <summary>
    /// 弹出通知组件提示消息框。
    /// </summary>
    /// <param name="title">通知标题。</param>
    /// <param name="text">提示消息文本。</param>
    /// <param name="style">提示样式，默认Success。</param>
    /// <returns></returns>
    public async Task NoticeAsync(string title, string text, StyleType style = StyleType.Success)
    {
        if (string.IsNullOrWhiteSpace(text))
            return;

        var config = new NotificationConfig
        {
            Message = title,
            ClassName = style.ToString(),
            Description = FormatMessage(text),
            Placement = NotificationPlacement.BottomRight
        };
        switch (style)
        {
            case StyleType.Success:
                await notice.Success(config);
                break;
            case StyleType.Info:
                await notice.Info(config);
                break;
            case StyleType.Warning:
                await notice.Warning(config);
                break;
            case StyleType.Error:
                config.Duration = 1000;
                await notice.Error(config);
                break;
            default:
                await notice.Info(config);
                break;
        }
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