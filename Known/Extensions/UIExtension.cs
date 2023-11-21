using Known.Razor;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Extensions;

public static class UIExtension
{
    public static void Info(this IUIService service, string message) => service.Toast(message, StyleType.Info);
    public static void Warning(this IUIService service, string message) => service.Toast(message, StyleType.Warning);
    public static void Error(this IUIService service, string message) => service.Toast(message, StyleType.Error);

    public static void Result(this IUIService service, Result result, Action onSuccess = null)
    {
        if (!result.IsValid)
        {
            service.Error(result.Message);
            return;
        }

        onSuccess?.Invoke();
        service.Toast(result.Message);
    }

    public static void BizStatus(this IUIService service, RenderTreeBuilder builder, string status)
    {
        if (string.IsNullOrWhiteSpace(status))
            return;

        var color = "";
        if (status.Contains("待") || status.Contains("中") || status.Contains("提交"))
            color = "#2db7f5";
        else if (status.Contains("完成") || status.Contains("结束"))
            color = "#108ee9";
        else if (status.Contains("退回") || status.Contains("不通过") || status.Contains("失败"))
            color = "#f50";
        else if (status.Contains("已") || status.Contains("通过") || status.Contains("成功") || status == "正常")
            color = "#87d068";
        service.BuildTag(builder, status, color);
    }
}