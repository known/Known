using Known.Blazor;
using Known.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Designers;

class EntityDesigner : BaseComponent
{
    private EntityInfo entity = new();
    private EntityView view;

    [Parameter] public string Model { get; set; }
    [Parameter] public Action<string> OnChanged { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        entity = GetEntity(Model);
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Cascading(this, this.BuildTree(b =>
        {
            b.Div("kui-designer entity", () =>
            {
                b.Div("panel-model", () =>
                {
                    b.Div("title", "实体模型");
                    b.Markup(@"<pre>说明：
实体：名称|代码
字段：名称|代码|类型|长度|必填
字段类型：CheckBox,CheckList,Date,Number,RadioList,Select,Text,TextArea
示例：
测试|KmTest
文本|Field1|Text|50|Y
数值|Field2|Number|18,5
日期|Field3|Date</pre>");
                    UI.BuildTextArea(b, new InputModel<string>
                    {
                        Disabled = ReadOnly,
                        Rows = 10,
                        Value = Model,
                        ValueChanged = this.Callback<string>(OnModelChanged)
                    });
                });
                b.Div("panel-view", () =>
                {
                    b.Component<EntityView>().Set(c => c.Model, entity).Build(value => view = value);
                });
            });
        }));
    }

    private async void OnModelChanged(string model)
    {
        Model = model;
        entity = GetEntity(model);
        await view?.SetModelAsync(entity);
        OnChanged?.Invoke(model);
    }

    private static EntityInfo GetEntity(string model)
    {
        var info = new EntityInfo();
        if (string.IsNullOrWhiteSpace(model))
            return info;

        var lines = model.Split(Environment.NewLine.ToArray())
                         .Where(s => !string.IsNullOrWhiteSpace(s))
                         .ToArray();

        if (lines.Length > 0)
        {
            var values = lines[0].Split('|');
            if (values.Length > 0)
                info.Name = values[0];
            if (values.Length > 1)
                info.Id = values[1];
        }

        if (lines.Length > 1)
        {
            for (int i = 1; i < lines.Length; i++)
            {
                var field = new FieldInfo();
                var values = lines[i].Split('|');
                if (values.Length > 0)
                    field.Name = values[0];
                if (values.Length > 1)
                    field.Id = values[1];
                if (values.Length > 2)
                    field.Type = values[2];
                if (values.Length > 3)
                    field.Length = values[3];
                if (values.Length > 4)
                    field.Required = values[4] == "Y";

                if (field.Type == "CheckBox")
                {
                    field.Length = "50";
                    field.Required = true;
                }

                info.Fields.Add(field);
            }
        }

        return info;
    }
}