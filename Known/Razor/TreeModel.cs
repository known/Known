namespace Known.Razor;

public class TreeModel
{
    public bool ExpandParent { get; set; }
    public string[] SelectedKeys { get; set; }
	public List<MenuItem> Data { get; set; }
    public Action<MenuItem> OnNodeClick { get; set; }
    public Func<Task> OnRefresh { get; set; }

    public Task RefreshAsync()
    {
        if (OnRefresh == null)
            return Task.CompletedTask;

        return OnRefresh.Invoke();
    }
}