using AntDesign;

namespace Known.Components;

/// <summary>
/// 扩展Ant表单接口。
/// </summary>
public interface IAntForm
{
    /// <summary>
    /// 取得表单是否查看模式。
    /// </summary>
    bool IsView { get; }
}

/// <summary>
/// 扩展Ant表单组件类。
/// </summary>
/// <typeparam name="TItem">表单数据对象类型。</typeparam>
public class AntForm<TItem> : Form<TItem>, IAntForm where TItem : class, new()
{
    /// <summary>
    /// 取得表单是否查看模式。
    /// </summary>
    public bool IsView => Form != null && Form.IsView;

    /// <summary>
    /// 取得或设置是否显示【确定】和【取消】操作按钮。
    /// </summary>
    [Parameter] public bool ShowAction { get; set; }

    /// <summary>
    /// 取得或设置表单组件模型对象实例。
    /// </summary>
    [Parameter] public FormModel<TItem> Form { get; set; }

    /// <summary>
    /// 初始化组件。
    /// </summary>
    protected override void OnInitialized()
    {
        //为true时，AutoComplete无法选中
        //为false时，AntSelect无法选中
        //此问题解决，需要将DataItemValue设为IsFixed
        ValidateOnChange = true;
        ValidateMode = FormValidateMode.Rules;
        if (Form != null)
        {
            Form.OnValidate = Validate;
            Form.OnLoadData = LoadDataAsync;
            Model = Form.Data;
            Form.Initialize();
        }
        base.OnInitialized();
    }

    /// <summary>
    /// 呈现表单组件内容。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Cascading<IAntForm>(this, b =>
        {
            b.Div(Form?.ClassName, () =>
            {
                base.BuildRenderTree(b);
                if (ShowAction && !Form.IsView)
                {
                    b.FormAction(() =>
                    {
                        b.AntButton(Form?.Language?.OK, this.Callback<MouseEventArgs>(OnSaveAsync));
                        b.AntButton(Form?.Language?.Cancel, this.Callback<MouseEventArgs>(OnCloseAsync), "default");
                    });
                }
            });
        });
    }

    private Task OnSaveAsync(MouseEventArgs args) => Form?.SaveAsync();
    private Task OnCloseAsync(MouseEventArgs args) => Form?.CloseAsync();

    private async Task LoadDataAsync(TItem data)
    {
        if (data == null)
            await Form?.LoadDefaultDataAsync();
        Model = Form?.Data;
        StateHasChanged();
    }
}