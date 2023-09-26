using Sample.Razor.Samples.Models;

namespace Sample.Razor.Samples.DataList;

class FullList : DmTestList
{
    public FullList()
    {
        Style = "fullList";

        var export = ToolButton.Export;
        export.Children.Add(ToolButton.ExportQuery);
        export.Children.Add(ToolButton.ExportPage);
        export.Children.Add(ToolButton.ExportAll);

        Tools = new List<ButtonInfo> { ToolButton.New, ToolButton.DeleteM, ToolButton.Import, export, ToolButton.Enable };
        Actions = new List<ButtonInfo> { GridAction.Edit, GridAction.Delete, GridAction.Print, GridAction.Return };

        var builder = new ColumnBuilder<DmTest>();
        //builder.Field(r => r.No);
        builder.Field(r => r.Title, true).IsVisible(false);
        builder.Field(r => r.Name, true).IsVisible(false);
        builder.Field(r => r.Picture).Width(100).Template((b, r) => b.Img(r.Picture));
        builder.Field("信息", "Info").Width(120).Template(BuildTestInfo);
        builder.Field(r => r.Status).Center(80).Template((b, r) => b.StatusTag(r.Status));
        builder.Field(r => r.Time).Width(100).Type(ColumnType.DateTime);
        builder.Field(r => r.Icon).Center(60).Template((b, r) => b.Icon(r.Icon));
        builder.Field(r => r.Color).Center(80).Template(BuildColorInfo);
        builder.Field(r => r.Progress).Width(100).Template(BuildProgressInfo);
        builder.Field(r => r.Sort).Center(60);
        builder.Field(r => r.Enabled).Center(80);
        builder.Field(r => r.Note).Width(200);
        Columns = builder.ToColumns();
    }

    public void New() => ShowForm(null, false);
    public void DeleteM() => DeleteRows(null);

    public void Enable()
    {
        var item = Tools?.FirstOrDefault(t => t.Id == "Enable");
        if (item != null)
        {
            var name = item.Name == "启用" ? "禁用" : "启用";
            var icon = item.Name == "启用" ? "fa fa-times-circle-o" : "fa fa-check-circle-o";
            toolbar?.SetItemName(item.Id, name, icon);
        }
    }

    public void Edit(DmTest row) => ShowForm(row, false);
    public void Delete(DmTest row) => DeleteRow(row, null);

    private void BuildTestInfo(RenderTreeBuilder builder, DmTest row)
    {
        builder.Link(row.Title, Callback(e => View(row, false)));
        builder.Span("small", row.Name);
        builder.Span("small", $"{row.Time:yyyy-MM-dd HH:mm:ss}");
    }

    private void BuildColorInfo(RenderTreeBuilder builder, DmTest row)
    {
        builder.Span("color", attr =>
        {
            attr.Style($"background-color:{row.Color};");
            builder.Text(row.Color);
        });
    }

    private void BuildProgressInfo(RenderTreeBuilder builder, DmTest row)
    {
        builder.Progress(StyleType.Primary, row.Progress, 100);
    }
}