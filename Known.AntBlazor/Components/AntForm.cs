namespace Known.AntBlazor.Components;

public class AntForm<TItem> : Form<TItem>, IAntForm where TItem : class, new()
{
    public bool IsView => Form != null && Form.IsView;
    [Parameter] public bool ShowAction { get; set; }
    [Parameter] public FormModel<TItem> Form { get; set; }

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
            Class = Form.ClassName;
            LabelColSpan = Form.Info?.LabelSpan ?? 0;
            WrapperColSpan = Form.Info?.WrapperSpan ?? 0;
            Model = Form.Data;
            Form.Initialize();
        }
        base.OnInitialized();
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Cascading<IAntForm>(this, b =>
        {
            b.Div("kui-form", () => base.BuildRenderTree(b));
            if (ShowAction && !Form.IsView)
            {
                b.FormAction(() =>
                {
                    b.AntButton(Form?.Language?.OK, this.Callback<MouseEventArgs>(OnSaveAsync));
                    b.AntButton(Form?.Language?.Cancel, this.Callback<MouseEventArgs>(OnCloseAsync), "default");
                });
            }
        });
    }

    private async void OnSaveAsync(MouseEventArgs args) => await Form?.SaveAsync();
    private async void OnCloseAsync(MouseEventArgs args) => await Form?.CloseAsync();

    private async Task LoadDataAsync(TItem data)
    {
        if (data == null)
            await Form?.LoadDefaultDataAsync();
        Model = Form?.Data;
        StateHasChanged();
    }
}