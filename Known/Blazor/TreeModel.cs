namespace Known.Blazor;

public class TreeModel
{
    public bool IsView { get; set; }
    public bool Checkable { get; set; }
    public bool ExpandRoot { get; set; }
    public string[] SelectedKeys { get; set; }
    public string[] DefaultCheckedKeys { get; set; }
    public List<MenuInfo> Data { get; set; }
    public Action<MenuInfo> OnNodeClick { get; set; }
    public Action<MenuInfo> OnNodeCheck { get; set; }
    public Action<TreeModel> OnModelChanged { get; set; }
    public Func<Task> OnRefresh { get; set; }

    public void Load() => OnModelChanged?.Invoke(this);

    public Task RefreshAsync()
    {
        //TODO：CUD时根节点未选中问题
        if (OnRefresh == null)
            return Task.CompletedTask;

        return OnRefresh.Invoke();
    }
}