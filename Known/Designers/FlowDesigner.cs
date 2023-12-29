using Known.Blazor;
using Known.Extensions;
using Known.WorkFlows;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Designers;

class FlowDesigner : BaseDesigner<string>
{
    private readonly List<CodeInfo> addTypes =
    [
        new CodeInfo("新建"),
        new CodeInfo("从流程库中选择")
    ];
    private List<CodeInfo> flowModels;
    private string addType;
    private FlowInfo flow = new();
    private FlowView view;

    private bool IsNew => addType == addTypes[0].Code;
    private bool IsReadOnly => ReadOnly || !Entity.IsFlow;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        flowModels = DataHelper.Flows.Select(m => new CodeInfo(m.Id, $"{m.Name}({m.Id})", m)).ToList();
        addType = string.IsNullOrWhiteSpace(Model) || Model.Contains('|')
                ? addTypes[0].Code : addTypes[1].Code;
        flow = DataHelper.GetFlow(Model);
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Div("kui-designer entity", () =>
        {
            builder.Div("panel-model", () =>
            {
                builder.Div("caption", () =>
                {
                    builder.Div("title", "流程模型");
                    BuildModelType(builder);
                });
                BuildNewModel(builder);
            });
            builder.Div("panel-view", () =>
            {
                builder.Component<FlowView>().Set(c => c.Model, flow).Build(value => view = value);
            });
        });
    }

    private void BuildModelType(RenderTreeBuilder builder)
    {
        builder.Div(() =>
        {
            UI.BuildRadioList(builder, new InputModel<string>
            {
                Disabled = IsReadOnly,
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
                    Disabled = IsReadOnly,
                    Codes = flowModels,
                    Value = Model,
                    ValueChanged = this.Callback<string>(OnModelChanged)
                });
            });
        }
    }

    private void BuildNewModel(RenderTreeBuilder builder)
    {
        builder.Markup($@"<pre><b>说明：</b>
流程：名称|代码
步骤：名称|代码|类型|默认操作用户|默认操作角色|通过状态|退回状态
类型：开始,审核,结束
<b>示例：</b>
测试|TestFlow
申请|Apply|开始|||待审核
审核|Verify|审核||审核人|审核通过|审核退回
结束|End|结束</pre>");
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
        flow = DataHelper.GetFlow(model);
        view?.SetModel(flow);
        OnChanged?.Invoke(model);
    }
}