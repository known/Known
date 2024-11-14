namespace Known.Admin.Pages;

class ModuleForm : BaseTabForm
{
    [Parameter] public FormModel<ModuleInfo> Model { get; set; }

    protected override async Task OnInitFormAsync()
    {
        await base.OnInitFormAsync();
        Model.SmallLabel = true;
        Model.Data.Entity = DataHelper.ToEntity(Model.Data.EntityData);
        Model.OnFieldChanged = OnFieldChanged;
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
        UIConfig.ModuleForm?.Invoke(Tab, Model);
        SetTabVisible();
    }

    private void BuildDataForm(RenderTreeBuilder builder) => UI.BuildForm(builder, Model);

    private void OnFieldChanged(string field)
    {
        if (field == nameof(SysModule.Target))
        {
            SetTabVisible();
            Tab.StateChanged();
        }
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