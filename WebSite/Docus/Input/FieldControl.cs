namespace WebSite.Docus.Input;

class FieldControl : BaseComponent
{
    private string? value;

    [Parameter] public Action<string>? OnVisibleChanged { get; set; }
    [Parameter] public Action<string>? OnEnabledChanged { get; set; }
    [Parameter] public Action? SetValue { get; set; }
    [Parameter] public Func<string?>? GetValue { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Div("control", attr =>
        {
            builder.Field<CheckBox>("chkVisible")
               .Set(f => f.Text, "可见")
               .Set(f => f.Value, "True")
               .Set(f => f.ValueChanged, OnVisibleChanged)
               .Build();
            builder.Field<CheckBox>("chkEnabled")
                   .Set(f => f.Text, "可用")
                   .Set(f => f.Value, "True")
                   .Set(f => f.ValueChanged, OnEnabledChanged)
                   .Build();
            builder.Button("赋值", Callback(SetValue), StyleType.Primary);
            builder.Button("取值", Callback(OnGetValue), StyleType.Primary);
            builder.Div("tips", value);
        });
    }

    private void OnGetValue() => value = GetValue?.Invoke();
}