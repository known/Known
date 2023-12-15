using Known.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Designers;

class UIDesigner : ComponentBase
{
    private ColumnInfo current;

    [Parameter] public string Type { get; set; }
    [Parameter] public Type EntityType { get; set; }
    [Parameter] public List<ColumnInfo> Columns { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        await OnColumnChanged(Columns?.FirstOrDefault());
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Cascading(this, this.BuildTree(b =>
        {
            b.Div("kui-designer", () =>
            {
                b.Div("kui-model-panel", () =>
                {
                    b.Div("title", "字段列表");
                    b.Component<ColumnPanel>().Set(c => c.Columns, Columns).Set(c => c.ColumnChanged, OnColumnChanged).Build();
                });
                b.Div("kui-view-panel", () =>
                {
                    if (Type == "Page")
                        b.Component<PageView>().Build();
                    else if (Type == "Form")
                        b.Component<FormView>().Build();
                });
                b.Div("kui-property-panel", () =>
                {
                    b.Div("title", "字段属性");
                    if (Type == "Page")
                        b.Component<PageProperty>().Set(c => c.Column, current).Build();
                    else if (Type == "Form")
                        b.Component<FormProperty>().Set(c => c.Column, current).Build();
                });
            });
        }));
    }

    private Task OnColumnChanged(ColumnInfo column)
    {
        current = column;
        return Task.CompletedTask;
    }
}