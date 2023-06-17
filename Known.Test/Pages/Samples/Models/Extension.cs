namespace Known.Test.Pages.Samples.Models;

static class Extension
{
    internal static T Random<T>(this IEnumerable<T> lists)
    {
        if (lists == null || !lists.Any())
            return default;

        var rand = new Random();
        var index = rand.Next(0, lists.Count());
        return lists.ElementAt(index);
    }

    internal static void BillStatus(this RenderTreeBuilder builder, string status)
    {
        var color = "bg-gray";
        if (status.Contains("待") || status.Contains("中"))
            color = "bg-info";
        else if (status.Contains("完成"))
            color = "bg-primary";
        else if (status.Contains("退回") || status.Contains("不通过") || status.Contains("失败"))
            color = "bg-danger";
        else if (status.Contains("已") || status.Contains("通过") || status.Contains("成功") || status == "正常")
            color = "bg-success";
        builder.Span($"badge {color}", status);
    }
}