using Known.Test.Pages.Samples.Models;

namespace Known.Test.Pages.Samples.DataList;

class FullList : DmTestList
{
    public FullList()
    {
        Tools = new List<ButtonInfo> { ToolButton.New, ToolButton.DeleteM, ToolButton.Import, ToolButton.Export };
        Actions = new List<ButtonInfo> { GridAction.Edit, GridAction.Delete, GridAction.Print, GridAction.Return };

        var builder = new ColumnBuilder<DmTest>();
        //builder.Field(r => r.No);
        builder.Field(r => r.Title, true).IsVisible(false);
        builder.Field(r => r.Name, true).IsVisible(false);
        builder.Field(r => r.Picture).Template((b, r) => b.Img(r.Picture));
        builder.Field("信息", "").Width(300).Template(BuildTestInfo);
        builder.Field(r => r.Status).Center();
        builder.Field(r => r.Time).Type(ColumnType.DateTime);
        builder.Field(r => r.Icon).Template((b, r) => b.Icon(r.Icon));
        builder.Field(r => r.Color).Template((b, r) => b.Span("", r.Color));
        builder.Field(r => r.Progress).Template((b, r) => { });
        builder.Field(r => r.Sort);
        builder.Field(r => r.Enabled);
        builder.Field(r => r.Note);
        Columns = builder.ToColumns();
    }

    private void BuildTestInfo(RenderTreeBuilder builder, DmTest row)
    {
        builder.Link(row.Title, Callback(e => { }));
        builder.Span("name", row.Name);
    }
}