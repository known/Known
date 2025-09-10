namespace Known;

internal class WebHttpResponse(HttpContext context) : IResponse
{
    private readonly HttpContext Context = context;
    private HttpResponse Response => Context.Response;

    public void Redirect(string url)
    {
        Response.Redirect(url);
    }
}