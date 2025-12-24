using AntDesign;

namespace Known.Components;

/// <summary>
/// 扩展Ant自动完成下拉框组件。
/// </summary>
public class AntAutoComplete : AutoComplete<CodeInfo>
{
    [CascadingParameter] private DataItem Item { get; set; }

    /// <summary>
    /// 取得或设置表单容器对象。
    /// </summary>
    [CascadingParameter] protected IComContainer AntForm { get; set; }

    /// <summary>
    /// 取得或设置UI上下文对象级联值实例。
    /// </summary>
    [CascadingParameter] public UIContext Context { get; set; }

    /// <summary>
    /// 取得或设置最大显示数量，默认10。
    /// </summary>
    [Parameter] public int MaxSize { get; set; } = 10;

    /// <summary>
    /// 取得或设置搜索方法。
    /// </summary>
    [Parameter] public Func<string, int, Task<List<CodeInfo>>> OnSearch { get; set; }

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        if (AntForm != null)
            Disabled = AntForm.IsView;
        if (Item != null)
        {
            Item.Type = typeof(string);
            Placeholder = Item.Language[Language.PleaseSelectInput];
        }
        AllowFilter = true;
        OptionFormat = item => item.Value.Name;

        if (OnSearch != null)
            OnInput = this.Callback<ChangeEventArgs>(OnInputAsync);

        base.OnInitialized();
    }

    private async Task OnInputAsync(ChangeEventArgs args)
    {
        Options = await OnSearch(args.Value.ToString(), MaxSize);
    }
}