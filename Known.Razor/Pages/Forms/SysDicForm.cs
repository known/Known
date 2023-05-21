namespace Known.Razor.Pages.Forms;

[Dialog(500, 380)]
class SysDicForm : BaseForm<SysDictionary>
{
    private CodeNameGrid grid;
    private List<CodeName> children;
    private SysDictionary model;

    protected override void OnInitialized()
    {
        model = TModel;
        if (model.HasChild)
        {
            children = model.Children;
            children ??= new List<CodeName>();
        }
    }

    protected override void BuildFields(FieldBuilder<SysDictionary> builder)
    {
        builder.Hidden(f => f.Id);
        builder.Hidden(f => f.Category);
        builder.Hidden(f => f.CategoryName);
        builder.Table(table =>
        {
            if (model.HasChild)
                table.ColGroup(100, 200, null);
            else
                table.ColGroup(100, null);
            table.Tr(attr =>
            {
                builder.Field<Text>(f => f.Code).Build();
                if (model.HasChild)
                {
                    table.Th("dic-extend", attr =>
                    {
                        table.Label("bold", "子字典信息");
                        table.Button(ToolButton.Import, Callback(OnImport), !ReadOnly, "right");
                    });
                }
            });
            table.Tr(attr =>
            {
                builder.Field<Text>(f => f.Name).Build();
                if (model.HasChild)
                {
                    table.Td(attr =>
                    {
                        attr.RowSpan(4).Style("position:relative;height:200px;");
                        table.Component<CodeNameGrid>()
                             .Set(c => c.ReadOnly, ReadOnly)
                             .Set(c => c.Data, children)
                             .Build(value => grid = value);
                    });
                }
            });
            table.Tr(attr => builder.Field<CheckBox>(f => f.Enabled).Build());
            table.Tr(attr => builder.Field<Number>(f => f.Sort).Build());
            table.Tr(attr => builder.Field<TextArea>(f => f.Note).Build());
        });
    }

    protected override void BuildButtons(RenderTreeBuilder builder)
    {
        builder.Button(FormButton.Save, Callback(OnSave), !ReadOnly);
        base.BuildButtons(builder);
    }

    private void OnImport()
    {
        grid.ShowImport(new ImportOption
        {
            Name = "子字典信息",
            Template = "代码,名称",
            Model = new ImportFormInfo(),
            Action = lines =>
            {
                foreach (var item in lines)
                {
                    children?.Add(new CodeName(item["代码"], item["名称"]));
                }
                StateChanged();
                return Task.FromResult(Result.Success("导入成功"));
            }
        });
    }

    private void OnSave()
    {
        SubmitAsync(data =>
        {
            if (grid != null)
                data.Child = Utils.ToJson(grid?.Data);
            return Platform.Dictionary.SaveDictionaryAsync(data);
        });
    }
}