namespace Known.Pages;

[Dialog(1000, 680)]
public class SysModuleForm : BaseForm<SysModule>
{
    private SysModule model;
    private ColumnGrid grid;
    private List<ColumnInfo> columns;

    public SysModuleForm()
    {
        Style = "module";
    }

    protected override void OnInitialized()
    {
        model = TModel;
        columns = model.Columns ?? new List<ColumnInfo>();
        base.OnInitialized();
    }

    protected override void BuildFields(FieldBuilder<SysModule> builder)
    {
        builder.Hidden(f => f.Id);
        builder.Hidden(f => f.ParentId);
        builder.Table(table =>
        {
            table.ColGroup(10, 15, 10, 15, 10, 15, 10, 15);
            BuildHead(table.Builder, table);
        });
        builder.FormList("列表栏位", "", () =>
        {
            builder.Component<ColumnGrid>()
                   .Set(c => c.ReadOnly, ReadOnly)
                   .Set(c => c.Data, columns)
                   .Build(value => grid = value);
        });
    }

    protected override void BuildButtons(RenderTreeBuilder builder)
    {
        builder.Button(FormButton.Save, Callback(OnSave), !ReadOnly);
        base.BuildButtons(builder);
    }

    private void OnSave()
    {
        SubmitAsync(data =>
        {
            data.ColumnData = Utils.ToJson(grid.Data);
            return Platform.Module.SaveModuleAsync(data);
        });
    }

    private void BuildHead(RenderTreeBuilder table, FieldBuilder<SysModule> builder)
    {
        table.Tr(attr =>
        {
            builder.Field<KText>(f => f.Code).Build();
            builder.Field<KText>(f => f.Name).Build();
            builder.Field<KText>(f => f.Icon).Build();
            builder.Field<KNumber>(f => f.Sort).Build();
        });
        table.Tr(attr =>
        {
            builder.Field<KText>(f => f.Description).ColSpan(3).Build();
            builder.Field<KText>(f => f.Target).ColSpan(3).Build();
        });
        table.Tr(attr =>
        {
            table.Th("", "选项");
            table.Td(attr =>
            {
                attr.ColSpan(3);
                table.Div("form-input", attr =>
                {
                    builder.Field<KCheckBox>(f => f.Enabled).IsInput(true).Set(f => f.Text, "启用").Set(f => f.Switch, true).Build();
                });
            });
            builder.Field<KText>(f => f.Note).ColSpan(3).Build();
        });
        table.Tr(attr =>
        {
            table.Th("", "按钮");
            table.Td("", attr =>
            {
                attr.ColSpan(7);
                builder.Div("inline", attr =>
                {
                    table.Div("form-input", attr => builder.Field<KText>(f => f.ButtonData).IsInput(true).ReadOnly(true).Build());
                    if (!ReadOnly)
                        table.Div(attr => table.Button(FormButton.Config, Callback(SetButton)));
                });
            });
        });
        table.Tr(attr =>
        {
            table.Th("", "操作");
            table.Td("", attr =>
            {
                attr.ColSpan(7);
                builder.Div("inline", attr =>
                {
                    table.Div("form-input", attr => builder.Field<KText>(f => f.ActionData).IsInput(true).ReadOnly(true).Build());
                    if (!ReadOnly)
                        table.Div(attr => table.Button(FormButton.Config, Callback(SetAction)));
                });
            });
        });
    }

    private void SetButton() => ConfigButton(ButtonGrid.KeyButton, model.ButtonData);
    private void SetAction() => ConfigButton(ButtonGrid.KeyAction, model.ActionData);

    private void ConfigButton(string name, string value)
    {
        UI.Show<ButtonGrid>(name, new(400, 400), action: attr =>
        {
            attr.Set(c => c.Key, name)
                .Set(c => c.Value, value)
                .Set(c => c.OnValueChanged, v =>
                {
                    if (name == ButtonGrid.KeyButton)
                    {
                        model.ButtonData = v;
                        Field(f => f.ButtonData).SetValue(v);
                    }
                    else if (name == ButtonGrid.KeyAction)
                    {
                        model.ActionData = v;
                        Field(f => f.ActionData).SetValue(v);
                    }
                });
        });
    }
}