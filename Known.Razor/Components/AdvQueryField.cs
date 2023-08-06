namespace Known.Razor.Components;

class AdvQueryField<T> : BaseComponent
{
    private readonly List<CodeInfo> QueryTypes = TypeHelper.GetEnumCodes<QueryType>();

    [Parameter] public Column<T> Column { get; set; }
    [Parameter] public QueryInfo Info { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        var types = GetQueryTypes();
        builder.Div("item", attr =>
        {
            builder.Label("form-label", Column.Name);
            builder.Field<Select>("", "QueryType").IsInput(true).Value($"{(int)Info.Type}")
                   .Set(f => f.EmptyText, "")
                   .Set(f => f.Items, types)
                   .Set(f => f.ValueChanged, v => Info.Type = (QueryType)int.Parse(v))
                   .Build();
            Column.BuildQuery(builder, "", Info.Value, v => Info.Value = v);
        });
    }

    private CodeInfo[] GetQueryTypes()
    {
        var types = new List<CodeInfo>();
        switch (Column.Type)
        {
            case ColumnType.Text:
                AddQueryType(types, QueryType.Equal);
                AddQueryType(types, QueryType.NotEqual);
                AddQueryType(types, QueryType.Contain);
                AddQueryType(types, QueryType.StartWith);
                AddQueryType(types, QueryType.EndWith);
                AddQueryType(types, QueryType.Batch);
                break;
            case ColumnType.Number:
                AddQueryType(types, QueryType.Equal);
                AddQueryType(types, QueryType.NotEqual);
                AddQueryType(types, QueryType.LessThan);
                AddQueryType(types, QueryType.LessEqual);
                AddQueryType(types, QueryType.GreatThan);
                AddQueryType(types, QueryType.GreatEqual);
                AddQueryType(types, QueryType.Batch);
                break;
            case ColumnType.Boolean:
                AddQueryType(types, QueryType.Equal);
                AddQueryType(types, QueryType.NotEqual);
                break;
            case ColumnType.Date:
            case ColumnType.DateTime:
                AddQueryType(types, QueryType.Between);
                AddQueryType(types, QueryType.BetweenNotEqual);
                AddQueryType(types, QueryType.BetweenLessEqual);
                AddQueryType(types, QueryType.BetweenGreatEqual);
                break;
            default:
                break;
        }
        return types.ToArray();
    }

    private void AddQueryType(List<CodeInfo> types, QueryType type)
    {
        var queryType = QueryTypes.FirstOrDefault(t => t.Code == $"{(int)type}");
        types.Add(queryType);
    }
}