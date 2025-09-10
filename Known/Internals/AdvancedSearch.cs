namespace Known.Internals;

class AdvancedSearch : BaseComponent
{
    private string SettingKey => $"UserSearch_{Context.Current?.Id}_{TableId}";
    private List<QueryInfo> Query { get; } = [];

    [Parameter] public string TableId { get; set; }
    [Parameter] public List<ColumnInfo> Columns { get; set; }

    internal async Task<List<QueryInfo>> SaveQueryAsync()
    {
        var query = Query.Where(q => Columns.Exists(c => c.Id == q.Id)).ToList();
        await Admin.SaveUserSettingAsync(new SettingFormInfo
        {
            BizType = SettingKey,
            BizData = query
        });
        return query;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
        {
            Query.Clear();
            var json = await Admin.GetUserSettingAsync(SettingKey);
            var items = Utils.FromJson<List<QueryInfo>>(json);
            if (items != null && items.Count > 0)
                Query.AddRange(items);
        }
    }

    protected override void BuildRender(RenderTreeBuilder builder)
    {
        builder.Div("kui-advanced-search", () =>
        {
            builder.Button(new ActionInfo(Language.New), this.Callback<MouseEventArgs>(OnAdd));
            foreach (var item in Query)
            {
                if (!item.IsNew && !Columns.Exists(c => c.Id == item.Id))
                    continue;

                builder.Div("item", () =>
                {
                    builder.Component<AdvancedSearchItem>()
                           .Set(c => c.Columns, Columns)
                           .Set(c => c.Item, item)
                           .Build();
                    builder.Button(new ActionInfo(Language.Delete), this.Callback<MouseEventArgs>(e => OnDelete(item)));
                });
            }
        });
    }

    private void OnAdd(MouseEventArgs args) => Query.Add(new QueryInfo("", "") { IsNew = true });
    private void OnDelete(QueryInfo item) => Query.Remove(item);
}

class AdvancedSearchItem : BaseComponent
{
    private ColumnInfo column;

    [Parameter] public List<ColumnInfo> Columns { get; set; }
    [Parameter] public QueryInfo Item { get; set; }

    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        column = Columns?.FirstOrDefault(f => f.Id == Item.Id);
    }

    protected override void BuildRender(RenderTreeBuilder builder)
    {
        builder.Div(() => BuildQueryField(builder, Item));
        builder.Div(() => BuildQueryType(builder, Item));
        builder.Div(() => BuildQueryValue(builder, Item));
    }

    private void BuildQueryField(RenderTreeBuilder builder, QueryInfo item)
    {
        builder.Select(new InputModel<string>
        {
            Placeholder = Language.PleaseSelect,
            Codes = Columns?.Where(f => f.IsQueryField).Select(f => new CodeInfo(f.Id, Language.GetFieldName(f))).ToList(),
            Value = item.Id,
            ValueChanged = this.Callback<string>(v =>
            {
                item.Id = v;
                column = Columns?.FirstOrDefault(f => f.Id == item.Id);
            })
        });
    }

    private void BuildQueryType(RenderTreeBuilder builder, QueryInfo item)
    {
        var types = column?.Type.GetQueryTypes(Language);
        builder.Select(new InputModel<string>
        {
            Placeholder = Language.PleaseSelect,
            Codes = types,
            Value = $"{item.Type}",
            ValueChanged = this.Callback<string>(v =>
            {
                Enum.TryParse(v, true, out QueryType type);
                item.Type = type;
            })
        });
    }

    private void BuildQueryValue(RenderTreeBuilder builder, QueryInfo item)
    {
        switch (column?.Type)
        {
            case FieldType.Switch:
            case FieldType.CheckBox:
                builder.Switch(new InputModel<bool>
                {
                    Value = Utils.ConvertTo<bool>(item.Value),
                    ValueChanged = this.Callback<bool>(v => item.Value = v.ToString())
                });
                break;
            case FieldType.Integer:
                builder.Number(new InputModel<int>
                {
                    Value = Utils.ConvertTo<int>(item.Value),
                    ValueChanged = this.Callback<int>(v => item.Value = v.ToString())
                });
                break;
            case FieldType.Number:
                builder.Number(new InputModel<decimal>
                {
                    Value = Utils.ConvertTo<decimal>(item.Value),
                    ValueChanged = this.Callback<decimal>(v => item.Value = v.ToString())
                });
                break;
            case FieldType.Date:
            case FieldType.DateTime:
                builder.RangePicker(new InputModel<string>
                {
                    Value = item.Value,
                    ValueChanged = this.Callback<string>(v => item.Value = v)
                });
                break;
            default:
                builder.TextBox(new InputModel<string>
                {
                    Value = item.Value,
                    ValueChanged = this.Callback<string>(v => item.Value = v)
                });
                break;
        }
    }
}