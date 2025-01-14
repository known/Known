namespace Known.Plugins;

[PagePlugin("表格页面", "table", Category = nameof(PagePluginType.Module), Sort = 1)]
class TablePagePlugin : PluginBase<TablePageInfo>
{
    protected override Task OnInitAsync()
    {
        return base.OnInitAsync();
    }

    protected override void BuildRender(RenderTreeBuilder builder)
    {
        Parameter = Utils.FromJson<TablePageInfo>(Plugin?.Setting);
        var table = new TableModel<Dictionary<string, object>>(this);
        table.SetPage(Parameter);
        builder.Component<TablePage<Dictionary<string, object>>>()
               .Set(c => c.Model, table)
               .Build();
    }
}