﻿namespace Known.Blazor;

public class TreeModel
{
    public bool IsView { get; set; }
    public bool Checkable { get; set; }
    public bool ExpandRoot { get; set; }
    public string[] SelectedKeys { get; set; }
    public string[] CheckedKeys { get; set; }
    public List<MenuInfo> Data { get; set; }
    public Action<MenuInfo> OnNodeClick { get; set; }
    public Action<MenuInfo> OnNodeCheck { get; set; }
    public Func<Task<TreeModel>> OnModelChanged { get; set; }
    public Func<Task> OnRefresh { get; set; }

    public Task RefreshAsync()
    {
        //TODO：CUD时根节点未选中问题
        if (OnRefresh == null)
            return Task.CompletedTask;

        return OnRefresh.Invoke();
    }
}