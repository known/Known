﻿namespace Known.Pages;

class ModuleForm : BaseTabForm
{
    [Parameter] public FormModel<SysModule> Model { get; set; }

    protected override async Task OnInitFormAsync()
    {
        await base.OnInitFormAsync();
        Model.SmallLabel = true;
        Model.Data.Entity = DataHelper.ToEntity(Model.Data.EntityData);
        Model.AddRow().AddColumn(c => c.Code)
                      .AddColumn(c => c.Name)
                      .AddColumn(c => c.Icon, c =>
                      {
                          c.Type = FieldType.Custom;
                          c.CustomField = nameof(IconPicker);
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
    }

    private void BuildDataForm(RenderTreeBuilder builder) => UI.BuildForm(builder, Model);
}