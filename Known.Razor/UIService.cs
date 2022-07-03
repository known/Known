/* -------------------------------------------------------------------------------
 * Copyright (c) Suzhou Puman Technology Co., Ltd. All rights reserved.
 * 
 * WebSite: https://www.pumantech.com
 * Contact: knownchen@163.com
 * 
 * Change Logs:
 * Date           Author       Notes
 * 2022-04-01     KnownChen
 * ------------------------------------------------------------------------------- */

using System.Drawing;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.JSInterop;

namespace Known.Razor;

public class UIService : IAsyncDisposable
{
    private readonly IJSRuntime jsRuntime;
    private readonly Lazy<Task<IJSObjectReference>> moduleTask;
    private DialogContainer Dialog { get; set; }

    public UIService(IJSRuntime jsRuntime)
    {
        this.jsRuntime = jsRuntime;
        moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
           "import", "./_content/Known/script.js").AsTask());
    }

    public async void Back()
    {
        try
        {
            await jsRuntime.InvokeAsync<string>("history.go", new object[] { -1 });
        }
        catch
        {
        }
    }

    public async void DownloadFile(string fileName, Stream stream)
    {
        try
        {
            var module = await moduleTask.Value;
            using var streamRef = new DotNetStreamReference(stream);
            await module.InvokeVoidAsync("downloadFile", fileName, streamRef);
        }
        catch
        {
        }
    }

    public async void ShowPdf(string id, Stream stream)
    {
        try
        {
            var module = await moduleTask.Value;
            using var streamRef = new DotNetStreamReference(stream);
            await module.InvokeVoidAsync("showPdf", id, streamRef);
        }
        catch
        {
        }
    }

    public void ShowPdf(string title, int width, int height, Stream stream)
    {
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

    public async void Show(ChartOption option)
    {
        try
        {
            var module = await moduleTask.Value;
            _ = await module.InvokeAsync<ChartOption>("showChart", option);
        }
        catch
        {
        }
    }

    public void Show(DialogOption option)
    {
        Dialog.Show(option);
    }

    public void Show<TForm>(string title, int width, int height, object model, Action<Result> onSuccess = null) where TForm : FormComponent
    {
        Show(new DialogOption
        {
            Title = title,
            Size = new Size(width, height),
            Content = builder =>
            {
                builder.Component<TForm>(attr =>
                {
                    attr.Add(nameof(FormComponent.IsDialog), true)
                        .Add(nameof(FormComponent.ReadOnly), onSuccess == null)
                        .Add(nameof(FormComponent.Model), model)
                        .Add(nameof(FormComponent.OnSuccess), delegate (Result model)
                        {
                            CloseDialog();
                            onSuccess?.Invoke(model);
                        });
                });
            }
        });
    }

    public async void Tips(string message, Action action = null)
    {
        Dialog.ShowTips(message);
        action?.Invoke();
        await Task.Delay(2000);
        Dialog.CloseTips();
    }

    public void Alert(string message, Action action = null)
    {
        Show(new DialogOption
        {
            Title = Language.AlertTips,
            Size = new Size(300, 200),
            Body = builder => BuildMessage(builder, message),
            Foot = builder => BuildButton(builder, action)
        });
    }

    public void Confirm(string message, Action action)
    {
        Show(new DialogOption
        {
            Title = Language.AlertConfirm,
            Size = new Size(300, 200),
            Body = builder => BuildMessage(builder, message),
            Foot = builder => BuildButton(builder, action, true)
        });
    }

    public void Result(Result result, bool back)
    {
        if (back)
            Result(result, () => Back());
        else
            Result(result);
    }

    public void Result(Result result, Action action = null)
    {
        if (!result.IsValid)
        {
            Alert(result.Message);
            return;
        }

        Tips(result.Message, action);
    }

    public void CloseDialog()
    {
        Dialog.Close();
    }

    private static void BuildMessage(RenderTreeBuilder builder, string message)
    {
        builder.Div("message", attr =>
        {
            builder.Text(message);
        });
    }

    private void BuildButton(RenderTreeBuilder builder, Action action, bool cancel = false)
    {
        builder.ButtonOK(EventCallback.Factory.Create(this, () =>
        {
            CloseDialog();
            action?.Invoke();
        }));
        if (cancel)
        {
            builder.ButtonCancel(EventCallback.Factory.Create(this, () => CloseDialog()));
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (moduleTask.IsValueCreated)
        {
            var module = await moduleTask.Value;
            await module.DisposeAsync();
        }
    }

    [JSInvokable]
    public static Task<string> Hello(string name)
    {
        return Task.FromResult($"Hello {name}");
    }

    internal void Register(DialogContainer dialog)
    {
        Dialog = dialog;
    }
}
