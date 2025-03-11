namespace Known;

class VisitLog
{
    private static readonly Dictionary<string, string> visitPages = [];

    internal static Task AddLogAsync(HttpContext ctx, Context context)
    {
        if (context.CurrentUser == null)
            return Task.CompletedTask;

        ctx.Request.Headers.TryGetValue("Referer", out var referer);
        if (string.IsNullOrWhiteSpace(referer))
            return Task.CompletedTask;

        var url = referer.ToString().Replace("://", "");
        var pageUrl = url.Substring(url.IndexOf("/"));
        visitPages.TryGetValue(context.CurrentUser.Token, out string prevUrl);
        if (pageUrl == prevUrl)
            return Task.CompletedTask;

        visitPages[context.CurrentUser.Token] = pageUrl;
        var service = ctx.RequestServices.GetRequiredService<IAdminService>();
        service.Context = context;
        var module = AppData.Data.Modules?.FirstOrDefault(m => m.Url == pageUrl);
        return service.AddLogAsync(new LogInfo
        {
            Type = nameof(LogType.Page),
            Target = module?.Name ?? pageUrl,
            Content = pageUrl
        });
    }
}