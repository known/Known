﻿using AntDesign;

namespace Known.Blazor;

/// <summary>
/// UI组件服务类。
/// </summary>
/// <param name="modal">模态弹窗服务。</param>
/// <param name="drawer">抽屉弹窗服务。</param>
/// <param name="message">消息弹窗服务。</param>
/// <param name="notice">通知弹窗服务。</param>
[Service]
public partial class UIService(ModalService modal, DrawerService drawer, MessageService message, INotificationService notice)
{
    /// <summary>
    /// 取得或设置语言实例。
    /// </summary>
    public Language Language { get; set; }

    internal static string GetTagColor(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;

        if (UIConfig.TagColor != null)
        {
            var color = UIConfig.TagColor.Invoke(text);
            if (!string.IsNullOrWhiteSpace(color))
                return color;
        }

        //Button
        if (text == "primary") return "blue-inverse";
        else if (text == "danger") return "red-inverse";
        //Module
        else if (text == "Menu") return "purple";
        else if (text == "Page") return "blue";
        else if (text == "Custom") return "green";
        else if (text == "Link") return "green";
        //Log
        else if (text == "Login") return "success";
        else if (text == "Logout" || text == nameof(LogLevel.Error)) return "red";
        else if (text == nameof(LogLevel.Critical)) return "magenta";
        else if (text == nameof(LogLevel.Warning)) return "orange";
        else if (text == nameof(LogLevel.Information) || text == nameof(LogTarget.JSON)) return "blue";
        else if (text == nameof(LogLevel.Debug) || text == nameof(LogTarget.BackEnd)) return "geekblue";
        else if (text == nameof(LogLevel.Trace) || text == nameof(LogTarget.FrontEnd)) return "cyan";
        //Task
        else if (text == "Pending") return "default";
        else if (text == "Running") return "processing";
        else if (text == "Success") return "success";
        else if (text == "Failed") return "error";
        //User
        else if (text == "Male" || text == "男") return "#108ee9";
        else if (text == "Female" || text == "女") return "hotpink";
        //Flow
        else if (text == "Save") return "default";
        else if (text == "Revoked") return "lime";
        else if (text == "Verifing") return "processing";
        else if (text == "Pass") return "success";
        else if (text == "Fail") return "error";
        //Status
        else if (text.Contains("已完成")) return "success";
        else if (text.Contains("中")) return "processing";
        else if (text.Contains("待") || text.Contains("提交")) return "#2db7f5";
        else if (text.Contains("完成") || text.Contains("结束")) return "#108ee9";
        else if (text.Contains("退回") || text.Contains("不通过") || text.Contains("失败") || text.Contains("异常")) return "#f50";
        else if (text.Contains("已") || text.Contains("通过") || text.Contains("成功") || text == "正常") return "#87d068";

        return "default";
    }

    private static RenderFragment BuildTree(Action<RenderTreeBuilder> action)
    {
        return delegate (RenderTreeBuilder builder) { action(builder); };
    }
}