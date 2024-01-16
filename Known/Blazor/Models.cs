using Microsoft.AspNetCore.Components;

namespace Known.Blazor;

public class DialogModel
{
    public string Title { get; set; }
    public Func<Task> OnOk { get; set; }
    public Func<Task> OnClose { get; set; }
    public RenderFragment Content { get; set; }
    public RenderFragment Footer { get; set; }

    public Task CloseAsync()
    {
        if (OnClose == null)
            return Task.CompletedTask;

        return OnClose.Invoke();
    }
}

public class InputModel<TValue>
{
    public bool Disabled { get; set; }
    public TValue Value { get; set; }
    public EventCallback<TValue> ValueChanged { get; set; }
    public List<CodeInfo> Codes { get; set; }
    public uint Rows { get; set; }
}