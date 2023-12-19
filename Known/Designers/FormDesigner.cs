using Known.Extensions;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Designers;

class FormDesigner : BaseDesigner<FormInfo>
{
    private FormProperty property;

    protected override void BuildDesigner(RenderTreeBuilder builder)
    {
        builder.Div("panel-view", () =>
        {
            builder.Component<FormView>().Build(value => view = value);
        });
        builder.Div("panel-property", () =>
        {
            builder.Component<FormProperty>().Build(value => property = value);
        });
    }

    protected override void OnFieldCheck()
    {
        foreach (var item in Fields)
        {
            if (!Model.Fields.Exists(c => c.Id == item.Id))
            {
                Model.Fields.Add(new FieldInfo
                {
                    Id = item.Id,
                    Name = item.Name,
                    Type = item.Type,
                    Required = item.Required
                });
            }
        }
        view?.SetModelAsync(Model);
        OnChanged?.Invoke(Model);
    }

    protected override void OnFieldClick(FieldInfo field) => property?.SetField(field);
}