namespace Known.Extensions;

public static class JSExtension
{
    public static async Task PasteTextAsync(this IJSRuntime js, Action<string> action)
    {
        var text = await js.InvokeAsync<string>("navigator.clipboard.readText", null);
        action?.Invoke(text);
    }
}