namespace Known.AntBlazor.Components;

public class AntForm<TItem> : Form<TItem>, IAntForm where TItem : class, new()
{
    public bool IsView => Form.IsView;
    [Parameter] public bool ShowAction { get; set; }
    [Parameter] public FormModel<TItem> Form { get; set; }

    protected override void OnInitialized()
    {
        ValidateOnChange = true;
        ValidateMode = FormValidateMode.Rules;
        LabelColSpan = Form?.LabelSpan ?? 0;
        WrapperColSpan = Form?.WrapperSpan ?? 0;
        Model = Form?.Data;
        Form?.Initialize();
        base.OnInitialized();
    }

    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender)
            Form.OnValidate = Validate;
        base.OnAfterRender(firstRender);
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Cascading<IAntForm>(this, b =>
        {
            base.BuildRenderTree(b);
            if (ShowAction && !Form.IsView)
            {
                b.FormAction(() =>
                {
                    b.Button(new ActionInfo { Name = "确定", Style = ButtonType.Primary, OnClick = this.Callback<MouseEventArgs>(OnSaveAsync) });
                    b.Button(new ActionInfo { Name = "取消", OnClick = this.Callback<MouseEventArgs>(OnCloseAsync) });
                });
            }
        });
    }

    private async void OnSaveAsync(MouseEventArgs args) => await Form.SaveAsync();
    private async void OnCloseAsync(MouseEventArgs args) => await Form.CloseAsync();
}