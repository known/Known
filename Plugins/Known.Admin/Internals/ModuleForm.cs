using Known.Designers;

namespace Known.Internals;

class ModuleForm : BaseTabForm
{
    private ModuleInfo Module;

    [Parameter] public FormModel<SysModule> Model { get; set; }

    protected override async Task OnInitFormAsync()
    {
        await base.OnInitFormAsync();
        Module = Utils.MapTo<ModuleInfo>(Model.Data);
        Module.IsView = Model.IsView;
        Module.Entity = DataHelper.ToEntity(Model.Data.EntityData);

        Model.SmallLabel = true;
        Model.OnFieldChanged = OnFieldChanged;
        Model.OnSaving = OnModelSaving;

        Model.AddRow().AddColumn(c => c.Code)
                      .AddColumn(c => c.Name)
                      .AddColumn(c => c.Icon, c =>
                      {
                          c.Type = FieldType.Custom;
                          c.CustomField = "IconPicker";
                      });
        Model.AddRow().AddColumn(c => c.Target, c =>
        {
            c.Category = nameof(ModuleType);
            c.Type = FieldType.RadioList;
            c.Span = 16;
        }).AddColumn(c => c.Sort, c => c.Span = 4)
          .AddColumn(c => c.Enabled, c => c.Span = 4);
        Model.AddRow().AddColumn(c => c.Url, c => c.Span = 8)
                      .AddColumn(c => c.Description, c => c.Span = 16);
        Model.AddRow().AddColumn(c => c.Note, c => c.Type = FieldType.TextArea);

        Tab.AddTab("BasicInfo", BuildDataForm);
        Tab.AddTab("ModelSetting", b => ModuleForm.BuildModuleModel(b, Module));
        //Tab.AddTab("FlowSetting", b => BuildModuleFlow(b, Module));
        Tab.AddTab("PageSetting", b => BuildModulePage(b, Module));
        Tab.AddTab("FormSetting", b => BuildModuleForm(b, Module));
        SetTabVisible();
    }

    private void BuildDataForm(RenderTreeBuilder builder) => builder.Form(Model);

    private static void BuildModuleModel(RenderTreeBuilder builder, ModuleInfo model)
    {
        builder.Component<EntityDesigner>()
               .Set(c => c.ReadOnly, model.IsView)
               .Set(c => c.Module, model)
               .Set(c => c.Model, model.EntityData)
               .Set(c => c.OnChanged, data => model.EntityData = data)
               .Build();
    }

    private static void BuildModuleFlow(RenderTreeBuilder builder, ModuleInfo model)
    {
        builder.Component<FlowDesigner>()
               .Set(c => c.ReadOnly, model.IsView)
               .Set(c => c.Module, model)
               .Set(c => c.Model, model.FlowData)
               .Set(c => c.OnChanged, data => model.FlowData = data)
               .Build();
    }

    private static void BuildModulePage(RenderTreeBuilder builder, ModuleInfo model)
    {
        model.Entity.PageUrl = model.Url;
        builder.Component<PageDesigner>()
               .Set(c => c.ReadOnly, model.IsView)
               .Set(c => c.Module, model)
               .Set(c => c.Model, model.Page)
               .Set(c => c.OnChanged, data => model.Page = data)
               .Build();
    }

    private static void BuildModuleForm(RenderTreeBuilder builder, ModuleInfo model)
    {
        builder.Component<FormDesigner>()
               .Set(c => c.ReadOnly, model.IsView)
               .Set(c => c.Module, model)
               //.Set(c => c.Flow, Flow)
               .Set(c => c.Model, model.Form)
               .Set(c => c.OnChanged, data => model.Form = data)
               .Build();
    }

    private void OnFieldChanged(string field)
    {
        if (field == nameof(SysModule.Target))
        {
            SetTabVisible();
            Tab.StateChanged();
        }
    }

    private Task<bool> OnModelSaving(SysModule model)
    {
        if (Module != null)
        {
            model.EntityData = Module.EntityData;
            model.FlowData = Module.FlowData;
            model.PageData = Module.PageData;
            model.FormData = Module.FormData;
        }
        return Task.FromResult(true);
    }

    private void SetTabVisible()
    {
        var isMenu = Model.Data.Target == ModuleType.Menu.ToString() ||
                     Model.Data.Target == ModuleType.IFrame.ToString();
        foreach (var item in Tab.Items)
        {
            if (item.Id != "BasicInfo")
                item.IsVisible = !isMenu;
        }
    }
}