namespace Known.Internals;

class AdvancedSearch : BaseComponent
{
    private const string ModeNormal = "Normal";
    private const string ModeAdvanced = "Advanced";

    private string SettingKey => $"UserSearch_{Context.Current?.Id}_{TableId}";
    private string SchemeKey => $"UserSearchScheme_{Context.Current?.Id}_{TableId}";

    private SearchMode _mode = SearchMode.Normal;
    private List<QueryInfo> Query { get; } = [];
    private List<QueryGroup> Groups { get; } = [];
    private List<SearchScheme> Schemes { get; } = [];
    private string _schemeId;
    private string _schemeName;

    [Parameter] public string TableId { get; set; }
    [Parameter] public List<ColumnInfo> Columns { get; set; }

    internal async Task<List<QueryInfo>> SaveQueryAsync()
    {
        if (_mode == SearchMode.Normal)
        {
            var query = Query.Where(q => Columns.Exists(c => c.Id == q.Id)).ToList();
            await Admin.SaveUserSettingAsync(new SettingFormInfo
            {
                BizType = SettingKey,
                BizData = query
            });
            return query;
        }

        var groupQueries = new List<QueryInfo>();
        foreach (var group in Groups)
        {
            var groupId = group.Id ?? Utils.GetGuid();
            foreach (var condition in group.Conditions)
            {
                if (!Columns.Exists(c => c.Id == condition.Id))
                    continue;
                condition.GroupId = groupId;
                groupQueries.Add(condition);
            }
        }

        await Admin.SaveUserSettingAsync(new SettingFormInfo
        {
            BizType = SettingKey,
            BizData = new { Mode = ModeAdvanced, Groups }
        });

        return groupQueries;
    }

    protected override async Task OnRenderAsync(bool firstRender)
    {
        await base.OnRenderAsync(firstRender);
        if (firstRender)
        {
            await LoadDataAsync();
        }
    }

    private async Task LoadDataAsync()
    {
        Query.Clear();
        Groups.Clear();
        Schemes.Clear();

        var json = await Admin.GetUserSettingAsync(SettingKey);
        if (!string.IsNullOrWhiteSpace(json))
        {
            try
            {
                var obj = Utils.FromJson<Dictionary<string, object>>(json);
                if (obj != null && obj.ContainsKey("Mode"))
                {
                    _mode = SearchMode.Advanced;
                    var groupList = Utils.FromJson<List<QueryGroup>>(Utils.ToJson(obj["Groups"]));
                    if (groupList != null)
                        Groups.AddRange(groupList);
                }
                else
                {
                    _mode = SearchMode.Normal;
                    var items = Utils.FromJson<List<QueryInfo>>(json);
                    if (items != null && items.Count > 0)
                        Query.AddRange(items);
                }
            }
            catch
            {
                _mode = SearchMode.Normal;
                var items = Utils.FromJson<List<QueryInfo>>(json);
                if (items != null && items.Count > 0)
                    Query.AddRange(items);
            }
        }

        var schemeJson = await Admin.GetUserSettingAsync(SchemeKey);
        if (!string.IsNullOrWhiteSpace(schemeJson))
        {
            var items = Utils.FromJson<List<SearchScheme>>(schemeJson);
            if (items != null)
                Schemes.AddRange(items);
        }
    }

    protected override void BuildRender(RenderTreeBuilder builder)
    {
        builder.Div("kui-advanced-search", () =>
        {
            BuildToolbar(builder);
            if (_mode == SearchMode.Normal)
                BuildNormalContent(builder);
            else
                BuildAdvancedContent(builder);
        });
    }

    private void BuildToolbar(RenderTreeBuilder builder)
    {
        builder.Div("kui-adv-search-toolbar", () =>
        {
            if (_mode == SearchMode.Normal)
            {
                builder.Button(new ActionInfo(Language.New), this.Callback<MouseEventArgs>(OnAdd));
            }
            else
            {
                builder.Button(Language.AddConditionGroup, this.Callback<MouseEventArgs>(OnAddGroup));
            }

            builder.Component<AntRadioGroup>()
                   .Set(c => c.Codes,
                   [
                       new(ModeNormal, Language.Normal),
                       new(ModeAdvanced, Language.Advanced)
                   ])
                   .Set(c => c.Value, _mode == SearchMode.Normal ? ModeNormal : ModeAdvanced)
                   .Set(c => c.ValueChanged, this.Callback<string>(OnModeChanged))
                   .Set(c => c.Solid, true)
                   .Build();
        });
    }

    private void OnModeChanged(string value)
    {
        _mode = value == ModeAdvanced ? SearchMode.Advanced : SearchMode.Normal;
        StateChanged();
    }

    private void BuildNormalContent(RenderTreeBuilder builder)
    {
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
    }

    private void BuildAdvancedContent(RenderTreeBuilder builder)
    {
        if (Groups.Count == 0)
        {
            builder.Div("kui-adv-search-empty", () =>
            {
                builder.Span(Language[Language.TipConditionGroup]);
            });
        }

        for (int i = 0; i < Groups.Count; i++)
        {
            var group = Groups[i];
            builder.Div("kui-adv-search-group", () =>
        {
            BuildGroupHeader(builder, i, group);
            foreach (var condition in group.Conditions)
            {
                if (!condition.IsNew && !Columns.Exists(c => c.Id == condition.Id))
                    continue;

                builder.Div("item", () =>
                {
                    builder.Component<AdvancedSearchItem>()
                           .Set(c => c.Columns, Columns)
                           .Set(c => c.Item, condition)
                           .Build();
                    builder.Button(new ActionInfo(Language.Delete), this.Callback<MouseEventArgs>(e => OnDeleteCondition(group, condition)));
                });
            }
        });
        }

        BuildSchemeBar(builder);
    }

    private void BuildGroupHeader(RenderTreeBuilder builder, int index, QueryGroup group)
    {
        builder.Div("kui-adv-search-group-header", () =>
        {
            var tip = Language[Language.TipGroupHeader];
            builder.Span(tip.Replace("{index}", (index + 1).ToString()));
            builder.Div(() =>
            {
                builder.Button(Language.AddCondition, this.Callback<MouseEventArgs>(e => OnAddCondition(group)));
                builder.Button(new ActionInfo(Language.Delete), this.Callback<MouseEventArgs>(e => OnDeleteGroup(group)));
            });
        });
    }

    private void BuildSchemeBar(RenderTreeBuilder builder)
    {
        builder.Div("kui-adv-search-scheme", () =>
        {
            builder.Span(Language[Language.QueryScheme]);

            var schemeCodes = Schemes.Select(s => new CodeInfo(s.Id, s.Name)).ToList();
            if (schemeCodes.Count > 0)
            {
                builder.Div("kui-adv-search-select", () =>
                {
                    builder.Select(new InputModel<string>
                    {
                        Placeholder = Language.SelectScheme,
                        Codes = schemeCodes,
                        Value = _schemeId,
                        ValueChanged = this.Callback<string>(OnSchemeSelected)
                    });
                });
            }

            builder.TextBox(new InputModel<string>
            {
                Placeholder = Language.SchemeName,
                Value = _schemeName,
                ValueChanged = this.Callback<string>(v => _schemeName = v)
            });

            builder.Button(Language.SaveScheme, this.Callback<MouseEventArgs>(OnSaveScheme));
            if (Schemes.Count > 0)
            {
                builder.Button(Language.DeleteScheme, this.Callback<MouseEventArgs>(OnDeleteScheme));
            }
        });
    }

    private void OnAdd(MouseEventArgs args) => Query.Add(new QueryInfo("", "") { IsNew = true });
    private void OnDelete(QueryInfo item) => Query.Remove(item);

    private void OnAddGroup(MouseEventArgs args)
    {
        Groups.Add(new QueryGroup
        {
            Id = Utils.GetGuid(),
            Conditions = []
        });
        StateChanged();
    }

    private void OnDeleteGroup(QueryGroup group)
    {
        Groups.Remove(group);
        StateChanged();
    }

    private void OnAddCondition(QueryGroup group)
    {
        group.Conditions.Add(new QueryInfo("", "") { IsNew = true });
        StateChanged();
    }

    private void OnDeleteCondition(QueryGroup group, QueryInfo condition)
    {
        group.Conditions.Remove(condition);
        StateChanged();
    }

    private async void OnSaveScheme(MouseEventArgs args)
    {
        if (string.IsNullOrWhiteSpace(_schemeName))
        {
            UI.Alert(Language.TipSelectScheme);
            return;
        }

        var scheme = new SearchScheme
        {
            Id = Utils.GetGuid(),
            Name = _schemeName,
            Mode = _mode,
            Conditions = _mode == SearchMode.Normal ? [.. Query] : null,
            Groups = _mode == SearchMode.Advanced ? [.. Groups] : null
        };

        Schemes.Add(scheme);
        _schemeName = null;
        await SaveSchemesAsync();
        StateChanged();
    }

    private async void OnDeleteScheme(MouseEventArgs args)
    {
        if (Schemes.Count == 0) return;
        var last = Schemes[^1];
        Schemes.Remove(last);
        await SaveSchemesAsync();
        StateChanged();
    }

    private async void OnSchemeSelected(string schemeId)
    {
        _schemeId = schemeId;
        var scheme = Schemes.FirstOrDefault(s => s.Id == schemeId);
        if (scheme == null) return;

        _mode = scheme.Mode;
        Query.Clear();
        Groups.Clear();

        if (scheme.Mode == SearchMode.Normal && scheme.Conditions != null)
        {
            Query.AddRange(scheme.Conditions);
        }
        else if (scheme.Mode == SearchMode.Advanced && scheme.Groups != null)
        {
            foreach (var group in scheme.Groups)
            {
                Groups.Add(new QueryGroup
                {
                    Id = group.Id ?? Utils.GetGuid(),
                    Conditions = group.Conditions?.Select(c => new QueryInfo(c.Id, c.Type, c.Value) { IsNew = c.IsNew, GroupId = group.Id }).ToList() ?? []
                });
            }
        }
        StateChanged();
    }

    private async Task SaveSchemesAsync()
    {
        await Admin.SaveUserSettingAsync(new SettingFormInfo
        {
            BizType = SchemeKey,
            BizData = Schemes
        });
    }
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
