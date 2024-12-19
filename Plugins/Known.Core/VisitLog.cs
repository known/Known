namespace Known.Core;

class VisitLog
{
    private static readonly Dictionary<string, string> visitPages = [];

    internal static async Task AddLogAsync(HttpContext ctx, Context context)
    {
        if (context.CurrentUser == null)
            return;

        var origin = ctx.Request.Headers["Origin"].ToString();
        var referer = ctx.Request.Headers["Referer"].ToString();
        if (string.IsNullOrWhiteSpace(origin) || string.IsNullOrWhiteSpace(referer))
            return;

        var pageUrl = referer.Replace(origin, "");
        visitPages.TryGetValue(context.CurrentUser.Token, out string prevUrl);
        if (pageUrl == prevUrl)
            return;

        visitPages[context.CurrentUser.Token] = pageUrl;
        var service = ctx.RequestServices.GetRequiredService<IAdminService>();
        service.Context = context;
        await service.AddLogAsync(new LogInfo { Type = LogType.Page, Content = pageUrl });
    }
}