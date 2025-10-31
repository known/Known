using AntDesign;

namespace Known.Components;

/// <summary>
/// 扩展Ant表单组件类。
/// </summary>
/// <typeparam name="TItem">表单数据对象类型。</typeparam>
public class AntForm<TItem> : Form<TItem>, IComContainer where TItem : class, new()
{
    [Inject] private IServiceScopeFactory Factory { get; set; }

    /// <summary>
    /// 取得是否是表格。
    /// </summary>
    public bool IsTable => false;

    /// <summary>
    /// 取得或设置是否查看模式。
    /// </summary>
    public bool IsView { get; set; }

    /// <summary>
    /// 取得或设置UI上下文对象级联值实例。
    /// </summary>
    [CascadingParameter] public UIContext Context { get; set; }

    /// <summary>
    /// 取得或设置是否显示【确定】和【取消】操作按钮。
    /// </summary>
    [Parameter] public bool ShowAction { get; set; }

    /// <summary>
    /// 取得或设置表单组件模型对象实例。
    /// </summary>
    [Parameter] public FormModel<TItem> Form { get; set; }

    /// <summary>
    /// 取得或设置表体内容模板。
    /// </summary>
    [Parameter] public RenderFragment ListContent { get; set; }

    /// <summary>
    /// 创建依赖注入的后端服务接口实例。
    /// </summary>
    /// <typeparam name="T">继承 IService 的服务接口。</typeparam>
    /// <returns></returns>
    public Task<T> CreateServiceAsync<T>() where T : IService
    {
        var context = Form?.Context ?? Context;
        return Factory.CreateAsync<T>(context);
    }

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        //为true时，AutoComplete无法选中
        //为false时，AntSelect无法选中
        //此问题解决，需要将DataItemValue设为IsFixed
        //ValidateOnChange = true;
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
    protected override void OnParametersSet()
    {
        if (Form != null)
        {
            IsView = Form.IsView;
            Model = Form.Data;
        }
        base.OnParametersSet();
    }

    /// <inheritdoc />
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (Form == null)
        {
            base.BuildRenderTree(builder);
            return;
        }

        builder.Cascading<IComContainer>(this, b =>
        {
            var className = CssBuilder.Default(Form.ClassName).AddClass(Class).BuildClass();
            b.Div(className, () =>
            {
                if (Form.Header != null)
                    b.Fragment(Form.Header);

                base.BuildRenderTree(b);

                if (ListContent != null)
                    b.Div("kui-form-list", () => b.Fragment(ListContent));

                if (ShowAction && !Form.IsView)
                {
                    b.FormAction(() =>
                    {
                        if (Form.FooterRight != null)
                            b.Fragment(Form.FooterRight);

                        if (Form.Actions != null && Form.Actions.Count > 0)
                        {
                            foreach (var action in Form.Actions)
                            {
                                builder.Button(action);
                            }
                        }

                        b.Button(Language.OK, this.Callback<MouseEventArgs>(OnSaveAsync));
                        b.Button(Language.Cancel, this.Callback<MouseEventArgs>(OnCloseAsync), "default");
                    }, Form.FooterLeft);
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