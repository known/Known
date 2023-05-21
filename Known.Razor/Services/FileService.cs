using Known.Razor.Templates;
using Microsoft.Extensions.DependencyInjection;

namespace Known.Razor;

partial class UIService
{
    public void Print<T>(Action<ComponentRenderer<T>> action) where T : IComponent
    {
        var services = new ServiceCollection();
        services.AddScoped<HttpClient>();
        var provider = services.BuildServiceProvider();
        var component = new ComponentRenderer<T>().AddServiceProvider(provider);
        action?.Invoke(component);
        var content = component.Render();
        Print(content);
    }

    public void Print(string content) => InvokeVoidAsync("KRazor.printContent", content);
    public void DownloadFile(string fileName, string url) => InvokeVoidAsync("KRazor.downloadFileByUrl", fileName, url);

    public async void DownloadFile(string fileName, Stream stream)
    {
        var module = await moduleTask.Value;
        using var streamRef = new DotNetStreamReference(stream);
        await module.InvokeVoidAsync("KRazor.downloadFileByStream", fileName, streamRef);
    }

    public async void ShowImage(string id, Stream stream)
    {
        if (stream == null)
            return;

        var module = await moduleTask.Value;
        using var streamRef = new DotNetStreamReference(stream);
        await module.InvokeVoidAsync("KRazor.showImage", id, streamRef);
    }

    public async void ShowPdf(string id, Stream stream)
    {
        if (stream == null)
            return;

        var module = await moduleTask.Value;
        using var streamRef = new DotNetStreamReference(stream);
        await module.InvokeVoidAsync("KRazor.showPdf", id, streamRef);
    }

    public void ShowPdf(string title, int width, int height, Stream stream)
    {
        if (stream == null)
            return;

        var id = "pdfViewer";
        Show(new DialogOption
        {
            Title = title,
            Size = new Size(width, height),
            Content = builder => builder.Div(attr => attr.Id(id).Class("fit")),
            OnShow = firstRender =>
            {
                if (firstRender)
                {
                    ShowPdf(id, stream);
                }
            }
        });
    }
}