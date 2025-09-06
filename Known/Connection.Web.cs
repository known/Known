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
            connection = new HubConnectionBuilder().WithUrl(url).Build();
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