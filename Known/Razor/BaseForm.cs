using Microsoft.AspNetCore.Components;

namespace Known.Razor;

public class BaseForm : BaseComponent { }

public class BaseForm<TItem> : BaseForm where TItem : class, new()
{
    [Parameter] public FormModel<TItem> Model { get; set; }
}