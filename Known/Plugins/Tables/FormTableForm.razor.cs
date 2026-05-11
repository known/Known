namespace Known.Plugins.Tables;

/// <summary>
/// 表单配置弹窗表格组件类。
/// </summary>
public partial class FormTableForm
{
    private FormModel<Dictionary<string, object>> Form;
    //private bool isPreview = false;

    private static string WrapClass => "kui-plugin-table";
    //private string WrapClass => $"kui-plugin-split{(isPreview ? " preview" : "")}";

    /// <summary>
    /// 取得或设置无代码表格插件配置信息。
    /// </summary>
    [Parameter] public AutoPageInfo Model { get; set; }

    /// <inheritdoc />
    protected override async Task OnInitFormAsync()
    {
        await base.OnInitFormAsync();
        Items = Model.GetFormFields();

        Form = new FormModel<Dictionary<string, object>>(this);
        PreviewForm();
    }

    private void OnTypeSetting(FormFieldInfo row)
    {
        var form = new FormModel<FormFieldInfo>(this)
        {
            Title = "更多属性设置",
            Data = row,
            Type = typeof(MoreFormSetting),
            OnSave = data => Result.SuccessAsync("")
        };
        form.Parameters[nameof(MoreFormSetting.IsDefaultValue)] = true;
        UI.ShowForm(form);
    }

    private void OnPreview()
    {
        Form.SetFormInfo(Model.Form);
        var model = new DialogModel
        {
            Title = Language.Preview,
            Width = Model.Form.Width,
            Maximizable = true,
            Content = b =>
            {
                b.Div("kui-plugin-page", () => b.Component<DemoTableForm>().Set(c => c.Model, Form).Build());
            }
        };
        UI.ShowDialog(model);
    }

    private static void PreviewForm()
    {
        //Form.Initialize(Model.Form);
    }

    private static void RefreshForm()
    {
        //PreviewForm();
        //StateChanged();
    }
}