using Known.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Designers;

class BaseProperty : BaseComponent
{
    [Parameter] public ColumnInfo Column { get; set; }

    protected FormModel<ColumnInfo> Model { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        Model = new FormModel<ColumnInfo>(UI, false);
        Model.Data = Column;
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder) => UI.BuildForm(builder, Model);
}