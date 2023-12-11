namespace Known.Blazor;

public class TreeModel
{
    public bool IsView { get; set; }
    public bool Checkable { get; set; }
    public bool ExpandRoot { get; set; }
    public string[] SelectedKeys { get; set; }
    public string[] DefaultCheckedKeys { get; set; }
    public List<MenuItem> Data { get; set; }
    public Action<MenuItem> OnNodeClick { get; set; }
    public Action<MenuItem> OnNodeCheck { get; set; }
    public Func<Task<List<MenuItem>>> OnQuery { get; set; }
    public Func<Task> OnRefresh { get; set; }

    public Task RefreshAsync()
    {
        if (OnRefresh == null)
            return Task.CompletedTask;

        return OnRefresh.Invoke();
    }
}