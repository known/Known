namespace Known.Blazor;

public class DialogModel
{
    public string ClassName { get; set; }
    public string Title { get; set; }
    public bool Draggable { get; set; } = true;
    public bool Resizable { get; set; }
    public bool Maximizable { get; set; }
    public bool DefaultMaximized { get; set; }
    public double? Width { get; set; }
    public Func<Task> OnOk { get; set; }
    public Func<Task> OnClose { get; set; }
    public Action OnClosed { get; set; }
    public RenderFragment Content { get; set; }
    public RenderFragment Footer { get; set; }

    public async Task CloseAsync()
    {
        if (OnClose != null)
            await OnClose.Invoke();

        OnClosed?.Invoke();
    }
}

public class DropdownModel
{
    public string Icon { get; set; }
    public string Text { get; set; }
    public string TextIcon { get; set; }
    public string TextButton { get; set; }
    public List<ActionInfo> Items { get; set; }
    public Action<ActionInfo> OnItemClick { get; set; }
}

public class InputModel<TValue>
{
    public bool Disabled { get; set; }
    public string Placeholder { get; set; }
    public TValue Value { get; set; }
    public EventCallback<TValue> ValueChanged { get; set; }
    public List<CodeInfo> Codes { get; set; }
    public uint Rows { get; set; }
}