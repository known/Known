namespace Sample.Razor;

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

    internal static void BillStatus(this RenderTreeBuilder builder, string? status)
    {
        if (string.IsNullOrWhiteSpace(status))
            return;

        var style = StyleType.Default;
        if (status.Contains("待") || status.Contains("中"))
            style = StyleType.Info;
        else if (status.Contains("完成"))
            style = StyleType.Primary;
        else if (status.Contains("重新"))
            style = StyleType.Warning;
        else if (status.Contains("退回") || status.Contains("不通过") || status.Contains("失败"))
            style = StyleType.Danger;
        else if (status.Contains("已") || status.Contains("通过") || status.Contains("成功") || status == "正常")
            style = StyleType.Success;
        builder.Tag(style, status);
    }
}