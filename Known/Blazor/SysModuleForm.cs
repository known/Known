using Known.Entities;
using Known.Extensions;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Blazor;

class SysModuleForm : BaseForm<SysModule>
{
    private readonly StepModel step = new();
    private StepForm stepForm;

    private bool IsPage => Model.Data.Target == "页面";
    private int StepCount => IsPage ? 4 : 1;

    protected override async Task OnInitFormAsync()
    {
        await base.OnInitFormAsync();
        step.Items.Add(new("基本信息") { Content = BuildDataForm });
        step.Items.Add(new("模型设置") { Content = BuildModuleModel });
        step.Items.Add(new("页面设置") { Content = BuildModulePage });
        step.Items.Add(new("表单设置") { Content = BuildModuleForm });
        Model.OnFieldChanged = OnFieldChanged;
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder) => builder.Cascading(this, BuildForm);

    private void BuildForm(RenderTreeBuilder builder)
    {
        builder.Component<StepForm>()
               .Set(c => c.Model, step)
               .Set(c => c.IsView, Model.IsView)
               .Set(c => c.StepCount, StepCount)
               .Set(c => c.OnSave, SaveAsync)
               .Build(value => stepForm = value);
    }

    private void BuildDataForm(RenderTreeBuilder builder) => UI.BuildForm(builder, Model);
    private void BuildModuleModel(RenderTreeBuilder builder) => builder.Component<SysModuleFormModel>().Build();
    private void BuildModulePage(RenderTreeBuilder builder) => builder.Component<SysModuleFormPage>().Build();
    private void BuildModuleForm(RenderTreeBuilder builder) => builder.Component<SysModuleFormForm>().Build();

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