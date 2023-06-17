namespace Known.Razor.Components;

class AdvQuery<TItem> : BaseComponent
{
    private List<QueryInfo> data;

    [Parameter] public string PageId { get; set; }
    [Parameter] public List<Column<TItem>> Columns { get; set; }
    [Parameter] public Action<List<QueryInfo>> OnSetting { get; set; }

    protected override void OnInitialized()
    {
        data = Setting.GetUserQuerys(PageId);
        data ??= new List<QueryInfo>();
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Div("title", "高级查询");
        builder.Div("form", attr =>
        {
            builder.Div("form-body", attr =>
            {
                foreach (var column in Columns)
                {
                    if (column.IsAdvQuery)
                    {
                        var info = data?.FirstOrDefault(d => d.Id == column.Id);
                        if (info == null)
                        {
                            info = new QueryInfo(column.Id, "");
                            data.Add(info);
                        }
                        column.BuildAdvQuery(builder, info);
                    }
                }
            });
            builder.Div("form-button", attr =>
            {
                builder.Button(FormButton.Query, Callback(OnQuery));
                builder.Button(FormButton.Reset, Callback(OnReset));
            });
        });
    }

    private async void OnQuery()
    {
        var info = new SettingFormInfo
        {
            Type = UserSetting.KeyQuery,
            Name = PageId,
            Data = Utils.ToJson(data)
        };
        await Platform.User.SaveSettingAsync(info);
        Setting.UserSetting.Querys[PageId] = data;
        OnSetting?.Invoke(data);
    }

    private void OnReset()
    {
        Setting.UserSetting.Querys.Remove(PageId);
        data = new List<QueryInfo>();
        StateChanged();
    }
}