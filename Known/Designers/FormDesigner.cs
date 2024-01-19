using Known.Extensions;
using Known.WorkFlows;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Designers;

class FormDesigner : BaseViewDesigner<FormInfo>
{
    private FormProperty property;
    private FormFieldInfo current;

    [Parameter] public FlowInfo Flow { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        Fields = Model.Fields?.Select(c => new FieldInfo { Id = c.Id, Name = c.Name }).ToList();
        current = Model.Fields.FirstOrDefault();
    }

    protected override void BuildView(RenderTreeBuilder builder)
    {
        builder.Component<FormView>()
               .Set(c => c.ReadOnly, ReadOnly)
               .Set(c => c.Flow, Flow)
               .Set(c => c.Model, Model)
               .Set(c => c.OnChanged, OnChanged)
               .Build(value => view = value);
    }

    protected override void BuildProperty(RenderTreeBuilder builder)
    {
        builder.Component<FormProperty>()
               .Set(c => c.ReadOnly, ReadOnly)
               .Set(c => c.Model, current)
               .Set(c => c.OnChanged, OnPropertyChanged)
               .Build(value => property = value);
    }

    protected override void OnFieldCheck(FieldInfo field)
    {
        var fields = new List<FormFieldInfo>();
        foreach (var item in Fields)
        {
            var field1 = new FormFieldInfo { Id = item.Id, Name = item.Name, Type = item.Type, Required = item.Required };
            var info = Model.Fields.FirstOrDefault(c => c.Id == item.Id);
            SetFormField(field1, info);
            fields.Add(field1);
        }
        Model.Fields = fields;
        OnFieldClick(field);
        ChangeView();
    }

    protected override void OnFieldClick(FieldInfo field)
    {
        current = Model.Fields.FirstOrDefault(c => c.Id == field.Id);
        property?.SetModel(current);
    }

    private void OnPropertyChanged(FormFieldInfo info)
    {
        current = Model.Fields.FirstOrDefault(c => c.Id == info.Id);
        SetFormField(current, info);
        ChangeView();
    }

    private static void SetFormField(FormFieldInfo field, FormFieldInfo info)
    {
        if (field == null || info == null)
            return;

        field.Row = info.Row;
        field.Column = info.Column;
        field.Type = info.Type;
        field.Required = info.Required;
        field.Category = info.Category;
        field.Placeholder = info.Placeholder;
        field.ReadOnly = info.ReadOnly;
        field.MultiFile = info.MultiFile;
    }

    private void ChangeView()
    {
        view?.SetModel(Model);
        OnChanged?.Invoke(Model);
    }
}