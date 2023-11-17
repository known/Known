using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Known.Razor;

public class ModalOption
{
    public string Title { get; set; }
    public Func<Task> OnClose { get; set; }
    public RenderFragment Content { get; set; }
    public RenderFragment Footer { get; set; }
}

public class ButtonOption
{
    public string Text { get; set; }
    public string Icon { get; set; }
    public string Type { get; set; }
    public EventCallback<MouseEventArgs> OnClick { get; set; }
}