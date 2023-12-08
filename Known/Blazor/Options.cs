using Microsoft.AspNetCore.Components;

namespace Known.Blazor;

public class ModalOption
{
    public string Title { get; set; }
    public Func<Task> OnOk { get; set; }
    public Func<Task> OnClose { get; set; }
    public RenderFragment Content { get; set; }
    public RenderFragment Footer { get; set; }
}

public class InputOption<TValue>
{
    public TValue Value { get; set; }
    public EventCallback<TValue> ValueChanged { get; set; }
}

public class ListOption<TValue>
{
    public bool Disabled { get; set; }
    public List<CodeInfo> Codes { get; set; }
    public TValue Value { get; set; }
    public EventCallback<TValue> ValueChanged { get; set; }
}