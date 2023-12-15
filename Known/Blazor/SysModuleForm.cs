using Known.Entities;
using Known.Extensions;
using Known.Helpers;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Blazor;

class SysModuleForm : BaseForm<SysModule>
{
    private readonly StepModel step = new();
    private StepForm stepForm;

    private bool IsMenu => Model.Data.Target == "菜单";
    private bool IsPage => Model.Data.Target == "页面";
    private int StepCount => IsMenu ? 1 : 3;

    internal Type EntityType => Config.ModelTypes.FirstOrDefault(t => t.Name == Model.Data.EntityType);
    internal List<ColumnInfo> Columns => TypeHelper.GetColumnAttributes(EntityType).Select(a => new ColumnInfo(a)).ToList();

    protected override async Task OnInitFormAsync()
    {
        await base.OnInitFormAsync();
        step.Items.Add(new("基本信息") { Content = BuildDataForm });
        step.Items.Add(new("页面设置") { Content = BuildModulePage });
        step.Items.Add(new("表单设置") { Content = BuildModuleForm });
        Model.OnFieldChanged = OnFieldChanged;
        Model.Codes["EntityTypes"] = Config.ModelTypes.Select(m => new CodeInfo(m.Name, m.Name)).ToList();
        //类型是菜单，则实体类型为只读
        Model.Column(c => c.EntityType).ReadOnly(IsMenu);
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
            SetEntityTypeStatus();
            stepForm.SetStepCount(StepCount);
        }
    }

    private void SetEntityTypeStatus()
    {
        var fdEntityType = Model.Fields[nameof(SysModule.EntityType)];
        fdEntityType.Column.IsReadOnly = IsMenu;
        fdEntityType.Column.IsRequired = IsPage;
        fdEntityType.StateChanged();
    }
}