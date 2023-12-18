using Known.Designers;
using Known.Entities;
using Known.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Blazor;

class SysModuleForm : BaseForm<SysModule>
{
    private readonly StepModel step = new();
    private StepForm stepForm;

    private bool IsPage => Model.Data.Target == "页面";
    private int StepCount => IsPage ? 4 : 1;

    internal EntityInfo Entity {  get; set; }

    protected override async Task OnInitFormAsync()
    {
        await base.OnInitFormAsync();
        step.Items.Add(new("基本信息") { Content = BuildDataForm });
        step.Items.Add(new("模型设置") { Content = BuildModuleModel });
        step.Items.Add(new("页面设置") { Content = BuildModulePage });
        step.Items.Add(new("表单设置") { Content = BuildModuleForm });
        Model.OnFieldChanged = OnFieldChanged;
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Cascading(this, b =>
        {
            b.Component<StepForm>()
             .Set(c => c.Model, step)
             .Set(c => c.IsView, Model.IsView)
             .Set(c => c.StepCount, StepCount)
             .Set(c => c.OnSave, SaveAsync)
             .Build(value => stepForm = value);
        });
    }

    private void BuildDataForm(RenderTreeBuilder builder) => UI.BuildForm(builder, Model);
    
    private void BuildModuleModel(RenderTreeBuilder builder)
    {
        builder.Component<SysModuleFormEntity>()
               .Set(c => c.ReadOnly, Model.IsView)
               .Set(c => c.Model, Model.Data.EntityData)
               .Set(c => c.OnChanged, model => Model.Data.EntityData = model)
               .Build();
    }

    private void BuildModulePage(RenderTreeBuilder builder)
    {
        builder.Component<SysModuleFormPage>()
               .Set(c => c.ReadOnly, Model.IsView)
               .Set(c => c.Model, Model.Data.Page)
               .Set(c => c.OnChanged, model => Model.Data.Page = model)
               .Build();
    }

    private void BuildModuleForm(RenderTreeBuilder builder)
    {
        builder.Component<SysModuleFormForm>()
               .Set(c => c.ReadOnly, Model.IsView)
               .Set(c => c.Model, Model.Data.Form)
               .Set(c => c.OnChanged, model => Model.Data.Form = model)
               .Build();
    }

    private async Task<bool> SaveAsync(bool isClose = false)
    {
        if (!Model.Validate())
            return false;

        await Model.SaveAsync(isClose);
        return true;
    }

    private void OnFieldChanged(string columnId)
    {
        if (columnId == nameof(SysModule.Target))
        {
            stepForm.SetStepCount(StepCount);
        }
    }
}

class SysModuleFormEntity : BaseComponent
{
    private List<string> models = [];

    [Parameter] public string Model { get; set; }
    [Parameter] public Action<string> OnChanged { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        var modules = await Platform.Module.GetModulesAsync();
        models = modules.Where(m => !string.IsNullOrWhiteSpace(m.EntityData) && m.EntityData.Contains('|')).Select(m => m.EntityData).ToList();
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Component<EntityDesigner>()
               .Set(c => c.ReadOnly, ReadOnly)
               .Set(c => c.Model, Model)
               .Set(c => c.Models, models)
               .Set(c => c.OnChanged, OnChanged)
               .Build();
    }
}

class SysModuleFormPage : BaseComponent
{
    [CascadingParameter] private SysModuleForm Form { get; set; }

    [Parameter] public PageInfo Model { get; set; }
    [Parameter] public Action<PageInfo> OnChanged { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Component<PageDesigner>()
               .Set(c => c.ReadOnly, ReadOnly)
               .Set(c => c.Entity, Form.Entity)
               .Set(c => c.Model, Model)
               .Set(c => c.OnChanged, OnChanged)
               .Build();
    }
}

class SysModuleFormForm : BaseComponent
{
    [CascadingParameter] private SysModuleForm Form { get; set; }

    [Parameter] public FormInfo Model { get; set; }
    [Parameter] public Action<FormInfo> OnChanged { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Component<FormDesigner>()
               .Set(c => c.ReadOnly, ReadOnly)
               .Set(c => c.Entity, Form.Entity)
               .Set(c => c.Model, Model)
               .Set(c => c.OnChanged, OnChanged)
               .Build();
    }
}