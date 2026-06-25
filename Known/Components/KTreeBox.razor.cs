namespace Known.Components;

/// <summary>
/// 树型框组件类。
/// </summary>
public partial class KTreeBox
{
    private string searchKey;
    private List<MenuInfo> dataSource = [];
    private List<MenuInfo> items = [];

    private string ClassName => CssBuilder.Default("kui-box").AddClass(Class).BuildClass();

    private void OnSearch(string key)
    {
        searchKey = key;
        //if (!string.IsNullOrWhiteSpace(searchKey))
        //    items = dataSource?.Where(c => c.Name.Contains(searchKey)).ToList();
        //else
        //    items = dataSource;
    }
}