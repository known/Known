namespace WebSite.Docus.Inputs;

class FieldControl : BaseComponent
{
    private string? visible = "True";
    private string? enabled = "True";
    private string? readOnly = "False";
    private string? value;

    [Parameter] public Action<bool>? OnVisibleChanged { get; set; }
    [Parameter] public Action<bool>? OnEnabledChanged { get; set; }
    [Parameter] public Action<bool>? OnReadOnlyChanged { get; set; }
    [Parameter] public Action? SetValue { get; set; }
    [Parameter] public Func<string?>? GetValue { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Div("control", attr =>
        {
            builder.Field<CheckBox>("chkVisible")
                   .Set(f => f.Text, "可见")
                   .Set(f => f.Value, visible)
                   .Set(f => f.ValueChanged, OnVisibleValueChanged)
                   .Build();
            builder.Field<CheckBox>("chkEnabled")
                   .Set(f => f.Text, "可用")
                   .Set(f => f.Value, enabled)
                   .Set(f => f.ValueChanged, OnEnabledValueChanged)
                   .Build();
            builder.Field<CheckBox>("chkReadOnly")
                   .Set(f => f.Text, "只读")
                   .Set(f => f.Value, readOnly)
                   .Set(f => f.ValueChanged, OnReadOnlyValueChanged)
                   .Build();
            builder.Button("赋值", Callback(SetValue), StyleType.Primary);
            builder.Button("取值", Callback(OnGetValue), StyleType.Primary);
            builder.Div("tips", value);
        });
    }

    private void OnVisibleValueChanged(string value)
    {
        visible = value;
        var check = Utils.ConvertTo<bool>(value);
        OnVisibleChanged?.Invoke(check);
    }

    private void OnEnabledValueChanged(string value)
    {
        enabled = value;
        var check = Utils.ConvertTo<bool>(value);
        OnEnabledChanged?.Invoke(check);
    }

    private void OnReadOnlyValueChanged(string value)
    {
        readOnly = value;
        var check = Utils.ConvertTo<bool>(value);
        OnReadOnlyChanged?.Invoke(check);
    }

    private void OnGetValue() => value = GetValue?.Invoke();
}