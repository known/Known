using Known.Blazor;
using Known.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Designers;

class EntityDesigner : BaseDesigner<string>
{
    private string dataTypes;
    private readonly List<CodeInfo> addTypes =
    [
        new CodeInfo("新建"),
        new CodeInfo("从实体库中选择")
    ];
    private List<CodeInfo> entityModels;

    private string addType;
    private EntityInfo entity = new();
    private EntityView view;

    private bool IsNew => addType == addTypes[0].Code;

    [CascadingParameter] private SysModuleForm Form { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        entityModels = EntityHelper.Models.Select(m => new CodeInfo(m.Id, $"{m.Name}({m.Id})", m)).ToList();
        dataTypes = string.Join(",", Cache.GetCodes(nameof(FieldType)).Select(c => c.Name));
        addType = string.IsNullOrWhiteSpace(Model) || Model.Contains('|')
                ? addTypes[0].Code : addTypes[1].Code;
        entity = EntityHelper.GetEntity(Model);
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
        builder.Markup($@"<pre><b>说明：</b>
实体：名称|代码|流程类
字段：名称|代码|类型|长度|必填
类型：{dataTypes}
<b>示例：</b>
测试|KmTest|Y
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
        entity = EntityHelper.GetEntity(model);
        Form.Entity = entity;
        view?.SetModel(entity);
        OnChanged?.Invoke(model);
    }
}