namespace Known.Razor.Pages.Forms;

class SysMyMessage : PageComponent
{
    private bool isList = true;
    private SysMessage model;

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Div("user-message", attr =>
        {
            if (isList)
                BuildList(builder);
            else
                BuildDetail(builder);
        });
    }

    private void BuildList(RenderTreeBuilder builder)
    {
        builder.Component<SysMessageList>()
               .Set(c => c.OnDetail, OnDetail)
               .Build();
    }

    private void BuildDetail(RenderTreeBuilder builder)
    {
        builder.Component<SysMessageForm>()
               .Set(c => c.Model, model)
               .Set(c => c.OnBack, () => isList = true)
               .Build();
    }

    private void OnDetail(SysMessage row)
    {
        model = row;
        isList = false;
        StateChanged();
    }
}

class SysMessageList : DataGrid<SysMessage>
{
    public SysMessageList()
    {
        ShowCheckBox = true;
        OrderBy = $"{nameof(SysMessage.CreateTime)} desc";
        Tools = new List<ButtonInfo> { ToolButton.DeleteM };

        var builder = new ColumnBuilder<SysMessage>();
        builder.Field(r => r.MsgLevel).Center(100).Template(BuildMsgLevel);
        builder.Field(r => r.Subject, true).Template(BuildSubject);
        builder.Field(r => r.MsgBy).Center(100);
        builder.Field(r => r.CreateTime).Name("收件时间").Center(120).Type(ColumnType.DateTime);
        Columns = builder.ToColumns();
    }

    [Parameter] public Action<SysMessage> OnDetail { get; set; }

    protected override Task<PagingResult<SysMessage>> OnQueryData(PagingCriteria criteria)
    {
        return Platform.User.QueryMessagesAsync(criteria);
    }

    public void DeleteM() => DeleteRows(Platform.User.DeleteMessagesAsync);

    internal static void BuildMsgLevel(RenderTreeBuilder builder, string level)
    {
        var style = level == Constants.UMLUrgent ? StyleType.Danger : StyleType.Primary;
        builder.Tag(style, level);
    }

    private void BuildMsgLevel(RenderTreeBuilder builder, SysMessage row)
    {
        BuildMsgLevel(builder, row.MsgLevel);
    }

    private void BuildSubject(RenderTreeBuilder builder, SysMessage row)
    {
        var style = row.Status == Constants.UMStatusUnread ? "noread" : "";
        builder.Link(row.Subject, Callback(() => OnDetail?.Invoke(row)), style);
    }
}

class SysMessageForm : BaseForm<SysMessage>
{
    private SysMessage model;

    [Parameter] public Action OnBack { get; set; }

    protected override async Task InitFormAsync()
    {
        model = TModel;
        if (model.Status == Constants.UMStatusUnread)
        {
            dynamic data = new ExpandoObject();
            data.Id = model.Id;
            data.Status = Constants.UMStatusRead;
            Result result = await Platform.User.SaveMessageAsync(data);
            PageAction.RefreshMessageCount?.Invoke(result.DataAs<int>());
        }
    }

    protected override void BuildFields(FieldBuilder<SysMessage> builder)
    {
        builder.Hidden(f => f.Id);
        builder.Table(table =>
        {
            table.ColGroup(10, 23, 10, 23, 10, 24);
            table.Tr(attr => builder.Field<Text>(f => f.Subject).ColSpan(5).ReadOnly(true).Build());
            table.Tr(attr =>
            {
                table.Th("", "级别");
                table.Td(attr => SysMessageList.BuildMsgLevel(table.Builder, model.MsgLevel));
                table.Field<Text>(f => f.MsgBy).ReadOnly(true).Build();
                table.Field<Date>(f => f.CreateTime)
                     .Label("收件时间").ReadOnly(true)
                     .Set(f => f.DateType, DateType.DateTime)
                     .Build();
            });
            table.Tr(attr => table.Field<Text>(f => f.Content).ColSpan(5).ReadOnly(true).Build());
        });
    }

    protected override void BuildButtons(RenderTreeBuilder builder)
    {
        builder.Button(FormButton.Back, Callback(OnBack));
    }
}