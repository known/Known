using Known.Test.Pages.Samples.Models;

namespace Known.Test.Pages.Samples.Forms;

class BaseForm : BaseForm<DmBill>
{
    protected string formData;

    protected override void BuildFields(RenderTreeBuilder builder)
    {
        base.BuildFields(builder);
        builder.Div("form-button", attr =>
        {
            builder.Button("加载", "fa fa-refresh", Callback(OnLoadData));
            builder.Button("只读", "fa fa-file-text-o", Callback(OnViewData));
            builder.Button("编辑", "fa fa-file-o", Callback(OnEditData));
            builder.Button("验证", "fa fa-check", Callback(OnCheckData));
            builder.Button("保存", "fa fa-save", Callback(OnSaveData));
            builder.Button("清空", "fa fa-trash-o", Callback(Clear));
        });
        builder.Div("demo-tips", formData);
    }

    protected override void BuildButtons(RenderTreeBuilder builder) { }

    protected virtual void OnLoadData() { }
    private void OnViewData() => SetReadOnly(true);
    private void OnEditData() => SetReadOnly(false);
    private void OnCheckData() => Validate();
    protected virtual void OnSaveData() => Submit(data => formData = Utils.ToJson(data));
}