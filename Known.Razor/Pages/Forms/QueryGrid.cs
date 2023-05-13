namespace Known.Razor.Pages.Forms;

class QueryGrid : EditGrid<QueryInfo>
{
    [Parameter] public List<ColumnInfo> Fields { get; set; }
    [Parameter] public Action<List<QueryInfo>> OnSetting { get; set; }

    protected override void OnInitialized()
    {
        Style = "form-grid";
        Actions = new List<ButtonInfo> { GridAction.Delete };

        var builder = new ColumnBuilder<QueryInfo>();
        builder.Field(r => r.Id).Name("栏位").Edit().Width(100).Select(new SelectOption
        {
            Items = Fields.Select(f => new CodeInfo(f.Id, f.Name)).ToArray()
        });
        builder.Field(r => r.Type).Name("条件").Edit().Width(100).Select(new SelectOption(typeof(QueryType)));
        builder.Field(r => r.Value).Name("值").Edit();
        Columns = builder.ToColumns();
    }

    protected override void BuildOther(RenderTreeBuilder builder)
    {
        builder.Div("form-button", attr =>
        {
            builder.Button(FormButton.Query, Callback(OnQuery));
            builder.Button(FormButton.Cancel, Callback(OnCancel));
        });
    }

    private void OnQuery()
    {
        OnSetting?.Invoke(Data);
        OnCancel();
    }

    private void OnCancel() => UI.CloseDialog();
}