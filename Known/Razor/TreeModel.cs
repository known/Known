namespace Known.Razor;

public class TreeModel
{
    public bool ExpandParent { get; set; }
	public List<MenuItem> Data { get; set; }
    public Action<MenuItem> OnNodeClick { get; set; }
    public Func<Task> OnRefresh { get; set; }

    public Task RefreshAsync() => OnRefresh?.Invoke();
}