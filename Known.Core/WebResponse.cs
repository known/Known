namespace Known.Core;

internal class WebResponse(HttpContext context) : IResponse
{
    private readonly HttpContext Context = context;
    private HttpResponse Response => Context.Response;

    public void Redirect(string url)
    {
        Response.Redirect(url);
    }
}