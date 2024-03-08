namespace Known.AntBlazor.Components;

public class AntColumn<TData> : Column<TData>
{
    [Parameter] public ColumnInfo Info { get; set; }

    protected override void OnInitialized()
    {
        AddAttributes();
        base.OnInitialized();
    }

    private void AddAttributes()
    {
        DataIndex = Info.Id;
        Ellipsis = true;
        Hidden = !Info.IsVisible;
        Sortable = Info.IsSort;
        //TODO:固定列显示混乱问题
        //if (!string.IsNullOrWhiteSpace(Info.Fixed))
        //    Fixed = Info.Fixed;
        if (!string.IsNullOrWhiteSpace(Info.Width))
            Width = Info.Width;
        if (!string.IsNullOrWhiteSpace(Info.Align))
            Align = GetColumnAlign(Info.Align);
        if (!string.IsNullOrWhiteSpace(Info.DefaultSort))
        {
            var sortName = Info.DefaultSort == "Descend" ? "descend" : "ascend";
            DefaultSortOrder = SortDirection.Parse(sortName);
        }
        //Filterable = true;
        if (Info.Type == FieldType.Date)
            Format = "yyyy-MM-dd";
        if (Info.Type == FieldType.DateTime)
            Format = "yyyy-MM-dd HH:mm:ss";
    }

    private static ColumnAlign GetColumnAlign(string align)
    {
        if (align == "center")
            return ColumnAlign.Center;
        else if (align == "right")
            return ColumnAlign.Right;
        return ColumnAlign.Left;
    }
}