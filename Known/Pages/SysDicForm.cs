namespace Known.Pages;

[Dialog(500, 380)]
class SysDicForm : BaseForm<SysDictionary>
{
    private CodeNameGrid? grid;
    private List<CodeName>? children;
    private SysDictionary? model;

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
            if (model != null && model.HasChild)
                table.ColGroup(100, 200, null);
            else
                table.ColGroup(100, null);
            table.Tr(attr =>
            {
                table.Field<KText>(f => f.Code).Build();
                if (model != null && model.HasChild)
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
                table.Field<KText>(f => f.Name).Build();
                if (model != null && model.HasChild)
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
            table.Tr(attr => table.Field<KCheckBox>(f => f.Enabled).Set(f => f.Switch, true).Build());
            table.Tr(attr => table.Field<KNumber>(f => f.Sort).Build());
            table.Tr(attr => table.Field<KTextArea>(f => f.Note).Build());
        });
    }

    protected override void BuildButtons(RenderTreeBuilder builder)
    {
        builder.Button(FormButton.Save, Callback(OnSaveAsync), !ReadOnly);
        base.BuildButtons(builder);
    }

    private void OnImport()
    {
        UI.ShowImport(new ImportOption
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
                return Result.SuccessAsync("导入成功");
            }
        });
    }

    private async Task OnSaveAsync()
    {
        await SubmitAsync(async data =>
        {
            if (grid != null)
                data.Child = Utils.ToJson(grid?.Data);
            return await Platform.Dictionary.SaveDictionaryAsync(data);
        });
    }
}