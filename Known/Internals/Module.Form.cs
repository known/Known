namespace Known.Internals;

class ModuleForm : BaseTabForm
{
    [Parameter] public FormModel<ModuleInfo> Model { get; set; }

    protected override async Task OnInitFormAsync()
    {
        await base.OnInitFormAsync();
        Model.SmallLabel = true;
        Model.OnFieldChanged = OnFieldChanged;

        Model.AddRow().AddColumn(c => c.Name)
                      .AddColumn(c => c.Icon, c =>
                      {
                          c.Type = FieldType.Custom;
                          c.CustomField = nameof(IconPicker);
                      });
        Model.AddRow().AddColumn(c => c.Type, c =>
        {
            c.Category = nameof(MenuType);
            c.Type = FieldType.RadioList;
            c.Span = 12;
        }).AddColumn(c => c.Enabled, c => c.Span = 6)
          .AddColumn(c => c.Sort, c => c.Span = 6);
        Model.AddRow().AddColumn(c => c.Url)
             .AddColumn(c => c.Target, c =>
             {
                 c.Category = nameof(LinkTarget);
                 c.Type = FieldType.RadioList;
             });

        Tab.AddTab(Language.BasicInfo, b => b.Form(Model));
        foreach (var item in UIConfig.ModuleFormTabs.OrderBy(t => t.Value.Id))
        {
            if (item.Value.Parameters == null)
                item.Value.Parameters = [];
            item.Value.Parameters[nameof(ModuleFormTab.Module)] = Model.Data;
            Tab.AddTab(item.Key, b => b.DynamicComponent(item.Value));
        }
        SetTabVisible();
    }

    private void OnFieldChanged(string field)
    {
        if (field == nameof(ModuleInfo.Target))
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
            if (item.Id != Language.BasicInfo)
                item.IsVisible = !isMenu;
        }
    }
}