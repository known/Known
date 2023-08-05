using Sample.Razor.Samples.Models;

namespace Sample.Razor.Samples.Forms;

class BaseForm : BaseForm<DmBill>
{
    private readonly List<ButtonInfo> tools = new()
    {
        new ButtonInfo("Load", "加载", "fa fa-refresh", StyleType.Default),
        new ButtonInfo("View", "只读", "fa fa-file-text-o", StyleType.Warning),
        new ButtonInfo("Edit", "编辑", "fa fa-file-o", StyleType.Success),
        new ButtonInfo("Check", "验证", "fa fa-check", StyleType.Info),
        new ButtonInfo("Save", "保存", "fa fa-save", StyleType.Primary),
        new ButtonInfo("Clear", "清空", "fa fa-trash-o", StyleType.Danger),
        new ButtonInfo("Clear", "禁用", "fa fa-trash-o", StyleType.Primary) { Enabled = false }
    };
    protected Toolbar toolbar;
    protected string formData = "{}";

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        base.BuildRenderTree(builder);
        builder.Div("demo-tips", formData);
    }

    protected override void BuildButtons(RenderTreeBuilder builder)
    {
        builder.Div("demo-tool", attr =>
        {
            builder.Field<Known.Razor.Components.Fields.CheckBox>("chkEnabled").Value("True")
               .ValueChanged(OnFormEnabledChanged)
               .Set(f => f.Text, "是否启用")
               .Build();
            builder.Component<Toolbar>()
                   .Set(c => c.Style, "demo")
                   .Set(c => c.Tools, tools)
                   .Set(c => c.OnAction, OnAction)
                   .Build(value => toolbar = value);
        });
    }

    public virtual void Load() { }
    public void View() => SetReadOnly(true);
    public void Edit() => SetReadOnly(false);
    public void Check() => Validate();
    public virtual void Save() => Submit(data => formData = Utils.ToJson(data));

    private void OnAction(ButtonInfo info)
    {
        var method = GetType().GetMethod(info.Id);
        if (method == null)
            UI.Toast($"{info.Name}方法不存在！");
        else
            method.Invoke(this, null);
        StateChanged();
    }

    private void OnFormEnabledChanged(string value)
    {
        var enabled = Utils.ConvertTo<bool>(value);
        SetEnabled(enabled);
    }
}