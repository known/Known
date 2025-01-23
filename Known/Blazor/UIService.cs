using AntDesign;

namespace Known.Blazor;

/// <summary>
/// UI组件服务类。
/// </summary>
/// <param name="modalService">模态弹窗服务。</param>
/// <param name="messageService">消息弹窗服务。</param>
/// <param name="noticeService">通知弹窗服务。</param>
public partial class UIService(ModalService modalService, MessageService messageService, INotificationService noticeService)
{
    private readonly ModalService _modal = modalService;
    private readonly MessageService _message = messageService;
    private readonly INotificationService _notice = noticeService;

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

        //Module
        if (text == "Menu") return "purple";
        else if (text == "Page") return "blue";
        else if (text == "Custom") return "green";
        else if (text == "Link") return "green";
        //Log
        else if (text == "Login") return "success";
        else if (text == "Logout") return "red";
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
}