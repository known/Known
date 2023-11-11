namespace Known.Razor;

public class PageModel<TItem> where TItem : class, new()
{
    public List<ActionInfo> Tools { get; set; }
    public Action<ActionInfo> OnToolClick { get; set; }
    public TableModel<TItem> Table { get; set; }
    public TreeModel Tree { get; set; }
}