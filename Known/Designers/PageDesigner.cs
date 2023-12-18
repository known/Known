using Known.Extensions;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Designers;

class PageDesigner : BaseDesigner<PageInfo>
{
    protected override void BuildDesigner(RenderTreeBuilder builder)
    {
        builder.Div("panel-view", () =>
        {
            builder.Component<PageView>().Set(c => c.Model, Model).Build(value => view = value);
        });
        builder.Div("panel-property", () =>
        {
            builder.Component<PageProperty>().Build(value => property = value);
        });
    }

    protected override void OnFieldCheck()
    {
        foreach (var item in Fields)
        {
            if (!Model.Columns.Exists(c => c.Id == item.Id))
            {
                Model.Columns.Add(new ColumnInfo1
                {
                    Id = item.Id,
                    Name = item.Name
                });
            }
        }
        view?.SetModelAsync(Model);
        OnChanged?.Invoke(Model);
    }
}