using System.ComponentModel.DataAnnotations;
using Known.Blazor;
using Known.Extensions;
using Known.WorkFlows;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Designers;

class FlowDesigner : BaseDesigner<string>
{
    private List<CodeInfo> addTypes;
    private List<CodeInfo> flowModels;
    private string addType;
    private FlowInfo flow = new();
    private FlowView view;

    private bool IsNew => addType == addTypes[0].Code;
    private bool IsReadOnly => ReadOnly || !Entity.IsFlow;

    protected override async Task OnInitializedAsync()
    {
        addTypes =
        [
            new CodeInfo("New", Language["Designer.New"]),
            new CodeInfo("Select", Language["Designer.SelectFlow"])
        ];
        await base.OnInitializedAsync();
        flowModels = DataHelper.Flows.Select(m => new CodeInfo(m.Id, $"{m.Name}({m.Id})", m)).ToList();
        addType = string.IsNullOrWhiteSpace(Model) || Model.Contains('|')
                ? addTypes[0].Code : addTypes[1].Code;
        flow = DataHelper.GetFlow(Model);
        Form.Flow = flow;
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Div("kui-designer entity", () =>
        {
            builder.Div("panel-model", () =>
            {
                builder.Div("caption", () =>
                {
                    builder.Div("title", Language["Designer.FlowModel"]);
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
        //TODO:示例语言切换
        builder.Markup($@"<pre><b>{Language["Designer.Explanation"]}</b>
{Language["Designer.Flow"]}{Language["Name"]}|{Language["Code"]}
{Language["Designer.Step"]}{Language["Name"]}|{Language["Code"]}|{Language["Type"]}|{Language["User"]}|{Language["Role"]}|{Language["Pass"]}|{Language["Fail"]}
{Language["Designer.Type"]}开始,提交,审核,结束
<b>{Language["Designer.Sample"]}</b>
{Language["Designer.Test"]}|TestFlow
申请|Apply|提交|||待审核
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
        Form.Flow = flow;
        view?.SetModel(flow);
        OnChanged?.Invoke(model);
    }
}