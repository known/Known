namespace Known.Razor.Pages.Accounts;

class SysMessageList : DataGrid<SysMessage>
{
    public SysMessageList()
    {
        Style = "userMessage";
        ShowCheckBox = true;
        OrderBy = $"{nameof(SysMessage.CreateTime)} desc";
        Tools = new List<ButtonInfo> { ToolButton.DeleteM };

        var builder = new ColumnBuilder<SysMessage>();
        builder.Field(r => r.MsgLevel).Center(100);
        builder.Field(r => r.Category).Center(100);
        builder.Field(r => r.Subject, true);
        builder.Field(r => r.MsgBy).Center(100);
        builder.Field(r => r.CreateTime).Center(120).Type(ColumnType.DateTime);
        Columns = builder.ToColumns();
    }

    protected override Task<PagingResult<SysMessage>> OnQueryData(PagingCriteria criteria)
    {
        return Platform.User.QueryMessagesAsync(criteria);
    }
}