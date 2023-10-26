using Known.Extensions;

namespace Known.Razor;

class TableFoot<TItem> : BaseComponent
{
    [CascadingParameter] private KDataGrid<TItem> Grid { get; set; }
    //[CascadingParameter] private Table<TItem> Table { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (!Grid.HasFoot())
            return;

        builder.TFoot(attr =>
        {
            builder.Tr(attr =>
            {
                var colSpan = 1;
                if (Grid.ShowCheckBox) colSpan++;
                if (Grid.GridColumns != null && Grid.GridColumns.Count > 0)
                {
                    builder.Td("index", "合计", colSpan);
                    foreach (var item in Grid.GridColumns)
                    {
                        if (!item.IsVisible)
                            continue;

                        builder.Td(attr =>
                        {
                            if (item.IsSum)
                            {
                                attr.Class("txt-right");
                                var value = GetSumValue(item.Id);
                                builder.Text($"{value}");
                            }
                        });
                    }
                }

                if (Grid.HasAction())
                    builder.Td();
            });
        });
    }

    private object GetSumValue(string id)
    {
        if (Grid.Sums != null && Grid.Sums.ContainsKey(id))
            return Grid.Sums[id];

        if (Grid.Data == null || Grid.Data.Count == 0)
            return 0;

        return Grid.Data.Sum(d =>
        {
            var data = Utils.MapTo<Dictionary<string, object>>(d);
            return Utils.ConvertTo<decimal>(data[id]);
        });
    }
}