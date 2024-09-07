namespace Known.Blazor;

/// <summary>
/// 泛型弹窗选择器组件基类。
/// </summary>
/// <typeparam name="TItem">选择对象类型。</typeparam>
public class BasePicker<TItem> : BaseComponent where TItem : class, new()
{
    /// <summary>
    /// 构造函数，创建一个弹窗选择器组件实例。
    /// </summary>
    public BasePicker()
    {
        SelectedItems = [];
    }

    /// <summary>
    /// 取得选择的对象实例列表。
    /// </summary>
    public virtual List<TItem> SelectedItems { get; }

    /// <summary>
    /// 取得或设置弹窗标题。
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// 取得或设置弹窗宽度。
    /// </summary>
    [Parameter] public double? Width { get; set; }

    /// <summary>
    /// 取得或设置是否是弹窗，框架内使用。
    /// </summary>
    [Parameter] public bool IsPick { get; set; }

    /// <summary>
    /// 取得或设置选择数据是否多选。
    /// </summary>
    [Parameter] public bool IsMulti { get; set; }

    /// <summary>
    /// 取得或设置是否显示允许清空操作。
    /// </summary>
    [Parameter] public bool AllowClear { get; set; }

    /// <summary>
    /// 取得或设置选择器组件字段值。
    /// </summary>
    [Parameter] public string Value { get; set; }

    /// <summary>
    /// 取得或设置选择器组件字段值改变事件处理方法。
    /// </summary>
    [Parameter] public Action<List<TItem>> ValueChanged { get; set; }

    /// <summary>
    /// 构建选择器组件内容。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    protected override void BuildRender(RenderTreeBuilder builder)
    {
        if (IsPick)
        {
            BuildContent(builder);
            return;
        }

        BuildTextBox(builder);
        UI.BuildText(builder, new InputModel<string> { Value = Value, Disabled = true });

        if (!ReadOnly)
        {
            if (AllowClear)
                builder.Icon("fa fa-close kui-pick-clear", this.Callback<MouseEventArgs>(OnClear));
            builder.Icon("fa fa-ellipsis-h kui-pick", this.Callback<MouseEventArgs>(ShowModal));
        }
    }

    /// <summary>
    /// 构建弹窗选择器文本附加组件。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    protected virtual void BuildTextBox(RenderTreeBuilder builder) { }

    /// <summary>
    /// 构建弹窗选择器内容组件。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    protected virtual void BuildContent(RenderTreeBuilder builder) { }

    /// <summary>
    /// 获取弹窗选择器参数字典。
    /// </summary>
    /// <returns></returns>
    protected virtual Dictionary<string, object> GetPickParameters() => new() { { nameof(IsPick), true } };

    private void OnClear(MouseEventArgs args)
    {
        if (ReadOnly)
            return;

        Value = string.Empty;
        ValueChanged?.Invoke(null);
    }

    private void ShowModal(MouseEventArgs args)
    {
        if (ReadOnly)
            return;

        BasePicker<TItem> picker = null;
        var model = new DialogModel
        {
            Title = Title,
            Width = Width,
            Content = b =>
            {
                var parameters = GetPickParameters();
                b.Component(GetType(), parameters, value => picker = (BasePicker<TItem>)value);
            }
        };
        model.OnOk = async () =>
        {
            var items = picker?.SelectedItems;
            if (items == null || items.Count == 0)
            {
                UI.Error(Language.SelectOneAtLeast);
                return;
            }

            ValueChanged?.Invoke(items);
            await model.CloseAsync();
        };
        UI.ShowDialog(model);
    }
}