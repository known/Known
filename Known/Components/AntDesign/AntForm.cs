using AntDesign;

namespace Known.Components;

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

    /// <inheritdoc />
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
            Form.Initialize();
        }
        base.OnInitialized();
    }

    /// <inheritdoc />
    protected override async Task OnParametersSetAsync()
    {
        if (Form != null)
            Model = Form.Data;
        await base.OnParametersSetAsync();
    }

    /// <inheritdoc />
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Cascading<IAntForm>(this, b =>
        {
            b.Div(Form?.ClassName, () =>
            {
                if (Form.Header != null)
                    b.Fragment(Form.Header);

                base.BuildRenderTree(b);

                if (ShowAction && !Form.IsView)
                {
                    b.FormAction(() =>
                    {
                        b.Button(Form?.Language?.OK, this.Callback<MouseEventArgs>(OnSaveAsync));
                        b.Button(Form?.Language?.Cancel, this.Callback<MouseEventArgs>(OnCloseAsync), "default");
                    });
                }
            });
        });
    }

    private Task OnSaveAsync(MouseEventArgs args) => Form?.SaveAsync();
    private Task OnCloseAsync(MouseEventArgs args) => Form?.CloseAsync();

    private async Task LoadDataAsync(TItem data)
    {
        if (Form == null)
            return;

        if (data == null)
            await Form.LoadDefaultDataAsync();
        else
            Form.Data = data;
        Model = Form.Data;
        StateHasChanged();
    }
}