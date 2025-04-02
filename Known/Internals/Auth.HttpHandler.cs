namespace Known.Internals;

class AuthHttpHandler(NavigationManager navigation) : HttpClientHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var response = await base.SendAsync(request, cancellationToken);
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            navigation.GoLoginPage();

        return response;
    }
}