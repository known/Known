using Microsoft.AspNetCore.SignalR.Client;

namespace Known;

class WebConnection(NavigationManager navigation) : IConnection
{
    private HubConnection connection;

    public string Name => "Web";

    public Task StartAsync<T>(string hubUrl, string method, Action<T> handler, CancellationToken token = default)
    {
        if (connection == null)
        {
            var url = navigation.ToAbsoluteUri(hubUrl);
            var handler1 = new HttpClientHandler();//忽略证书验证
            handler1.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
            connection = new HubConnectionBuilder().WithUrl(url, option =>
            {
                option.HttpMessageHandlerFactory = _ => handler1;
            }).Build();
        }
        connection.On(method, (string message) =>
        {
            var info = Utils.FromJson<T>(message);
            handler.Invoke(info);
        });
        if (connection.State == HubConnectionState.Disconnected)
            return connection.StartAsync(token);
        return Task.CompletedTask;
    }

    public Task StopAsync(string method, CancellationToken token = default)
    {
        connection?.Remove(method);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken token = default)
    {
        return connection?.StopAsync(token);
    }

    public ValueTask DisposeAsync()
    {
        if (connection == null)
            return ValueTask.CompletedTask;

        return connection.DisposeAsync();
    }
}