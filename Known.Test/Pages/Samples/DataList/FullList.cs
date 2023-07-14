using Known.Test.Pages.Samples.Models;

namespace Known.Test.Pages.Samples.DataList;

class FullList : DmTestList
{
    public FullList()
    {
        var export = ToolButton.Export;
        export.Children.Add(ToolButton.ExportQuery);
        export.Children.Add(ToolButton.ExportPage);
        export.Children.Add(ToolButton.ExportAll);

        Tools = new List<ButtonInfo> { ToolButton.New, ToolButton.DeleteM, ToolButton.Import, export };
        Actions = new List<ButtonInfo> { GridAction.Edit, GridAction.Delete, GridAction.Print, GridAction.Return };

        var builder = new ColumnBuilder<DmTest>();
        //builder.Field(r => r.No);
        builder.Field(r => r.Title, true).IsVisible(false);
        builder.Field(r => r.Name, true).IsVisible(false);
        builder.Field(r => r.Picture).Template((b, r) => b.Img(r.Picture));
        builder.Field("信息", "Info").Width(200).Template(BuildTestInfo);
        builder.Field(r => r.Status).Center().Template((b, r) => b.BillStatus(r.Status));
        builder.Field(r => r.Time).Type(ColumnType.DateTime).IsVisible(false);
        builder.Field(r => r.Icon).Center().Template((b, r) => b.Icon(r.Icon));
        builder.Field(r => r.Color).Template(BuildColorInfo);
        builder.Field(r => r.Progress).Template(BuildProgressInfo);
        builder.Field(r => r.Sort).Center();
        builder.Field(r => r.Enabled);
        builder.Field(r => r.Note);
        Columns = builder.ToColumns();
    }

    public void New() => ShowForm();
    public void DeleteM() => DeleteRows(null);
    public void Edit(DmTest row) => ShowForm(row);
    public void Delete(DmTest row) => DeleteRow(row, null);

    private void BuildTestInfo(RenderTreeBuilder builder, DmTest row)
    {
        builder.Link(row.Title, Callback(e => { }));
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
        builder.Progress(StyleType.Default, 100, row.Progress);
    }
}