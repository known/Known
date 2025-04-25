namespace Known.Blazor;

/// <summary>
/// 泛型弹窗选择器组件基类。
/// </summary>
/// <typeparam name="TItem">选择对象类型。</typeparam>
public class BasePicker<TItem> : BaseComponent
{
    /// <summary>
    /// 构造函数，创建一个弹窗选择器组件实例。
    /// </summary>
    public BasePicker()
    {
        SelectedItems = [];
    }

    /// <summary>
    /// 取得或设置数据项查询表达式。
    /// </summary>
    protected Func<TItem, bool> ItemExpression { get; set; }

    /// <summary>
    /// 取得选择的对象实例列表。
    /// </summary>
    public virtual List<TItem> SelectedItems { get; }

    /// <summary>
    /// 取得或设置弹窗标题。
    /// </summary>
    [Parameter] public string Title { get; set; }

    /// <summary>
    /// 取得或设置弹窗宽度。
    /// </summary>
    [Parameter] public double? Width { get; set; }

    /// <summary>
    /// 取得或设置是否显示允许清空操作。
    /// </summary>
    [Parameter] public bool AllowClear { get; set; }

    /// <summary>
    /// 取得或设置选择器组件字段显示文本。
    /// </summary>
    [Parameter] public string Text { get; set; }

    /// <summary>
    /// 取得或设置选择器组件字段值。
    /// </summary>
    [Parameter] public object Value { get; set; }

    /// <summary>
    /// 取得或设置选择器组件字段值改变事件处理方法。
    /// </summary>
    [Parameter] public EventCallback<object> ValueChanged { get; set; }

    /// <summary>
    /// 取得字段关联的栏位配置信息。
    /// </summary>
    [Parameter] public ColumnInfo Column { get; set; }

    /// <summary>
    /// 取得或设置是否是弹窗，框架内使用。
    /// </summary>
    [Parameter] public bool IsPick { get; set; }

    internal Func<Task> OnClose { get; set; }

    /// <inheritdoc />
    protected override void BuildRender(RenderTreeBuilder builder)
    {
        if (IsPick)
        {
            BuildContent(builder);
            return;
        }

        builder.Div("kui-picker", () =>
        {
            BuildTextBox(builder);
            builder.TextBox(new InputModel<string> { Value = Text ?? Value?.ToString(), Disabled = true });

            if (!ReadOnly)
            {
                if (AllowClear)
                    builder.Icon("fa fa-close kui-pick-clear ant-btn-link", this.Callback<MouseEventArgs>(OnClear));
                builder.Icon("fa fa-ellipsis-h kui-pick ant-btn-link", this.Callback<MouseEventArgs>(ShowModal));
            }
        });
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
    /// <returns>选择器参数字典。</returns>
    protected virtual Dictionary<string, object> GetPickParameters() => new() {
        { nameof(IsPick), true },
        { nameof(Value), Value }
    };

    /// <summary>
    /// 选择器选择内容改变时触发的方法。
    /// </summary>
    /// <param name="items">选中的对象列表。</param>
    protected virtual void OnValueChanged(List<TItem> items) { }

    private async Task OnClear(MouseEventArgs args)
    {
        if (ReadOnly)
            return;

        Text = string.Empty;
        Value = string.Empty;
        if (ValueChanged.HasDelegate)
            await ValueChanged.InvokeAsync(Value);
    }

    private void ShowModal(MouseEventArgs args)
    {
        if (ReadOnly)
            return;

        BasePicker<TItem> picker = null;
        DialogModel model = null;
        model = new DialogModel
        {
            Title = Title,
            Width = Width,
            Content = b =>
            {
                var parameters = GetPickParameters();
                b.Component(GetType(), parameters, value =>
                {
                    picker = (BasePicker<TItem>)value;
                    picker.ValueChanged = ValueChanged;
                    picker.OnClose = () => model?.CloseAsync();
                });
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

            OnValueChanged(items);
            await model.CloseAsync();
        };
        UI.ShowDialog(model);
    }
}