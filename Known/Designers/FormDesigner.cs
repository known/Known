using Known.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Designers;

class FormDesigner : ComponentBase
{
    private ColumnInfo current;
    private BaseView view;
    private BaseProperty property;

    [Parameter] public FormInfo Model { get; set; }
    [Parameter] public Action<FormInfo> OnChanged { get; set; }

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
                b.Div("panel-model", () =>
                {
                    b.Div("title", "字段列表");
                    b.Component<ColumnPanel>()
                     .Set(c => c.Columns, Columns)
                     .Set(c => c.ColumnChanged, OnColumnChanged)
                     .Build();
                });
                b.Div("panel-view", () =>
                {
                    b.Component<FormView>().Build(value => view = value);
                });
                b.Div("panel-property", () =>
                {
                    b.Div("title", $"字段属性 - {current?.Id}");
                    b.Component<FormProperty>().Build(value => property = value);
                });
            });
        }));
    }

    private Task OnColumnChanged(ColumnInfo column)
    {
        current = column;
        view?.StateChanged();
        property?.StateChanged();
        return Task.CompletedTask;
    }
}