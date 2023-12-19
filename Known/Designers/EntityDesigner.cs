using Known.Blazor;
using Known.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Designers;

class EntityDesigner : BaseComponent
{
    private readonly List<CodeInfo> addTypes =
    [
        new CodeInfo("新建"),
        new CodeInfo("从实体库中选择")
    ];
    private readonly List<CodeInfo> entityModels = [];

    private string addType;
    private EntityInfo entity = new();
    private EntityView view;

    private bool IsNew => addType == addTypes[0].Code;

    [CascadingParameter] private SysModuleForm Form { get; set; }

    [Parameter] public string Model { get; set; }
    [Parameter] public List<string> Models { get; set; }
    [Parameter] public Action<string> OnChanged { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        LoadEntityModels(Models);
        addType = !string.IsNullOrWhiteSpace(Model) && Model.Contains('|')
                ? addTypes[0].Code : addTypes[1].Code;
        entity = GetEntity(Model);
        Form.Entity = entity;
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Div("kui-designer entity", () =>
        {
            builder.Div("panel-model", () =>
            {
                builder.Div("caption", () =>
                {
                    builder.Div("title", "实体模型");
                    BuildModelType(builder);
                });
                BuildNewModel(builder);
            });
            builder.Div("panel-view", () =>
            {
                builder.Component<EntityView>().Set(c => c.Model, entity).Build(value => view = value);
            });
        });
    }

    private void BuildModelType(RenderTreeBuilder builder)
    {
        builder.Div(() =>
        {
            UI.BuildRadioList(builder, new InputModel<string>
            {
                Disabled = ReadOnly,
                Codes = addTypes,
                Value = addType,
                ValueChanged = this.Callback<string>(OnTypeChanged)
            });
        });

        if (!IsNew)
        {
            builder.Div("select", () =>
            {
                UI.BuildSelect(builder, new InputModel<string>
                {
                    Disabled = ReadOnly,
                    Codes = entityModels,
                    Value = Model,
                    ValueChanged = this.Callback<string>(OnModelChanged)
                });
            });
        }
    }

    private void BuildNewModel(RenderTreeBuilder builder)
    {
        builder.Markup(@"<pre><b>说明：</b>
实体：名称|代码|流程类
字段：名称|代码|类型|长度|必填
字段类型：CheckBox,CheckList,Date,Number,RadioList,Select,Text,TextArea
<b>示例：</b>
测试|KmTest
文本|Field1|Text|50|Y
数值|Field2|Number|18,5
日期|Field3|Date</pre>");
        UI.BuildTextArea(builder, new InputModel<string>
        {
            Disabled = ReadOnly || !IsNew,
            Rows = 11,
            Value = Model,
            ValueChanged = this.Callback<string>(OnModelChanged)
        });
    }

    private void OnTypeChanged(string type) => addType = type;

    private void OnModelChanged(string model)
    {
        Model = model;
        entity = GetEntity(model);
        Form.Entity = entity;
        view?.SetModel(entity);
        OnChanged?.Invoke(model);
    }

    private void LoadEntityModels(List<string> models)
    {
        entityModels.Clear();
        foreach (var model in models)
        {
            var entity = GetEntityInfo(model);
            entityModels.Add(new CodeInfo(entity.Id, $"{entity.Name}({entity.Id})", entity));
        }
    }

    private EntityInfo GetEntity(string model)
    {
        var info = new EntityInfo();
        if (string.IsNullOrWhiteSpace(model))
            return info;

        if (!model.Contains('|'))
            return entityModels.FirstOrDefault(m => m.Code == model)?.DataAs<EntityInfo>();

        return GetEntityInfo(model);
    }

    private static EntityInfo GetEntityInfo(string model)
    {
        var info = new EntityInfo();
        var lines = model.Split([.. Environment.NewLine])
                         .Where(s => !string.IsNullOrWhiteSpace(s))
                         .ToArray();

        if (lines.Length > 0)
        {
            var values = lines[0].Split('|');
            if (values.Length > 0) info.Name = values[0];
            if (values.Length > 1) info.Id = values[1];
            if (values.Length > 2) info.IsFlow = values[2] == "Y";
        }

        if (lines.Length > 1)
        {
            for (int i = 1; i < lines.Length; i++)
            {
                var field = new FieldInfo();
                var values = lines[i].Split('|');
                if (values.Length > 0) field.Name = values[0];
                if (values.Length > 1) field.Id = values[1];
                if (values.Length > 2) field.Type = values[2];
                if (values.Length > 3) field.Length = values[3];
                if (values.Length > 4) field.Required = values[4] == "Y";

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