namespace Known.Test.Pages.Samples;

class DemoGoodsGrid : DataGrid<DmGoods>
{
    public DemoGoodsGrid()
    {
        Name = "商品明细";
        ShowEmpty = false;

        var builder = new ColumnBuilder<DmGoods>();
        builder.Field(r => r.Code);
        builder.Field(r => r.Name);
        builder.Field(r => r.Model);
        builder.Field(r => r.Unit);
        builder.Field(r => r.Note);
        Columns = builder.ToColumns();
    }

    protected override Task OnInitializedAsync()
    {
        ShowCheckBox = !ReadOnly;
        if (!ReadOnly)
        {
            Tools = new List<ButtonInfo> { ToolButton.New, ToolButton.DeleteM, ToolButton.MoveUp, ToolButton.MoveDown, ToolButton.Import, ToolButton.Export };
            Actions = new List<ButtonInfo> { GridAction.View, GridAction.Edit, GridAction.Delete };
        }
        else
        {
            Actions = new List<ButtonInfo> { GridAction.View };
        }
        return base.OnInitializedAsync();
    }
}