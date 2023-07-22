using Known.Test.Pages.Samples.Models;

namespace Known.Test.Pages.Samples.Forms;

class BaseForm : BaseForm<DmBill>
{
    protected Toolbar toolbar;
    protected string formData = "{}";

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        base.BuildRenderTree(builder);
        builder.Div("demo-tips", formData);
    }

    protected override void BuildButtons(RenderTreeBuilder builder)
    {
        var tools = new List<ButtonInfo>
        {
            new ButtonInfo("Load", "加载", "fa fa-refresh", StyleType.Default),
            new ButtonInfo("View", "只读", "fa fa-file-text-o", StyleType.Warning),
            new ButtonInfo("Edit", "编辑", "fa fa-file-o", StyleType.Success),
            new ButtonInfo("Check", "验证", "fa fa-check", StyleType.Info),
            new ButtonInfo("Save", "保存", "fa fa-save", StyleType.Primary),
            new ButtonInfo("Clear", "清空", "fa fa-trash-o", StyleType.Danger),
            new ButtonInfo("Clear", "禁用", "fa fa-trash-o", StyleType.Primary) { Enabled = false }
        };
        builder.Component<Toolbar>()
               .Set(c => c.Tools, tools)
               .Set(c => c.OnAction, OnAction)
               .Build(value => toolbar = value);
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
}