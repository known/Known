using Known.Blazor;
using Known.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.AntBlazor.Components;

public class AntLanguage : BaseComponent
{
    private ActionInfo current;

    [Parameter] public Action OnChanged { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        current = Language.GetLanguage(Context.CurrentLanguage);
        current ??= Language.Items.FirstOrDefault();
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Component<AntDropdown>()
               .Set(c => c.TextIcon, current?.Icon)
               .Set(c => c.Items, Language.Items)
               .Set(c => c.OnItemClick, OnLanguageChanged)
               .Build();
    }

    private void OnLanguageChanged(ActionInfo info)
    {
        current = info;
        Context.SetCurrentLanguage(JS, current.Id);
        OnChanged?.Invoke();
    }
}