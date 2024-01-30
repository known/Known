using Known.Designers;
using Known.Extensions;
using Known.Helpers;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace Known.Blazor;

class AdvancedSearch : BaseComponent
{
    private EntityInfo entity;
    private string SettingKey => $"UserSearch_{Context.Module.Id}";
    private List<QueryInfo> Query { get; } = [];

    internal async Task<List<QueryInfo>> SaveQueryAsync()
    {
        await Platform.Setting.SaveUserSettingAsync(SettingKey, Query);
        return Query;
    }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        entity = DataHelper.GetEntity(Context.Module.EntityData);
        Query.Clear();
        var items = await Platform.Setting.GetUserSettingAsync<List<QueryInfo>>(SettingKey);
        if (items != null && items.Count > 0)
            Query.AddRange(items);
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Div("kui-advanced-search", () =>
        {
            UI.Button(builder, new ActionInfo("New"), this.Callback((Action<MouseEventArgs>)(e => OnAdd())));
            foreach (var item in Query)
            {
                builder.Div("item", () =>
                {
                    builder.Component<AdvancedSearchItem>()
                           .Set(c => c.Entity, entity)
                           .Set(c => c.Item, item)
                           .Build();
                    UI.Button(builder, new ActionInfo("Delete"), this.Callback<MouseEventArgs>(e => OnDelete(item)));
                });
            }
        });
    }

    private void OnAdd() => Query.Add(new QueryInfo("", ""));
    private void OnDelete(QueryInfo item) => Query.Remove(item);
}

class AdvancedSearchItem : BaseComponent
{
    private readonly List<CodeInfo> QueryTypes = TypeHelper.GetEnumCodes(typeof(QueryType));
    private List<FieldInfo> fields;
    private FieldInfo field;

    [Parameter] public EntityInfo Entity { get; set; }
    [Parameter] public QueryInfo Item { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        fields = Entity.GetFields(Language);
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Div(() => BuildQueryField(builder, Item));
        builder.Div(() => BuildQueryType(builder, Item));
        builder.Div(() => BuildQueryValue(builder, Item));
    }

    private void BuildQueryField(RenderTreeBuilder builder, QueryInfo item)
    {
        UI.BuildSelect(builder, new InputModel<string>
        {
            Placeholder = Language["PleaseSelect"],
            Codes = fields.Select(f => new CodeInfo(f.Id, f.Name)).ToList(),
            Value = item.Id,
            ValueChanged = this.Callback<string>(v =>
            {
                item.Id = v;
                field = fields.FirstOrDefault(f => f.Id == item.Id);
            })
        });
    }

    private void BuildQueryType(RenderTreeBuilder builder, QueryInfo item)
    {
        var types = GetQueryTypes();
        UI.BuildSelect(builder, new InputModel<string>
        {
            Placeholder = Language["PleaseSelect"],
            Codes = types,
            Value = $"{(int)item.Type}",
            ValueChanged = this.Callback<string>(v =>
            {
                Enum.TryParse<QueryType>(v, true, out QueryType type);
                item.Type = type;
            })
        });
    }

    private void BuildQueryValue(RenderTreeBuilder builder, QueryInfo item)
    {
        switch (field?.Type)
        {
            case FieldType.Switch:
            case FieldType.CheckBox:
                UI.BuildSwitch(builder, new InputModel<bool>
                {
                    Value = Utils.ConvertTo<bool>(item.Value),
                    ValueChanged = this.Callback<bool>(v => item.Value = v.ToString())
                });
                break;
            case FieldType.Number:
                UI.BuildNumber(builder, new InputModel<decimal>
                {
                    Value = Utils.ConvertTo<decimal>(item.Value),
                    ValueChanged = this.Callback<decimal>(v => item.Value = v.ToString())
                });
                break;
            case FieldType.Date:
                UI.BuildDatePicker(builder, new InputModel<string>
                {
                    Value = item.Value,
                    ValueChanged = this.Callback<string>(v => item.Value = v)
                });
                break;
            default:
                UI.BuildText(builder, new InputModel<string>
                {
                    Value = item.Value,
                    ValueChanged = this.Callback<string>(v => item.Value = v)
                });
                break;
        }
    }

    private List<CodeInfo> GetQueryTypes()
    {
        var types = new List<CodeInfo>();
        switch (field?.Type)
        {
            case FieldType.Switch:
            case FieldType.CheckBox:
                AddQueryType(types, QueryType.Equal);
                AddQueryType(types, QueryType.NotEqual);
                break;
            case FieldType.Number:
                AddQueryType(types, QueryType.Equal);
                AddQueryType(types, QueryType.NotEqual);
                AddQueryType(types, QueryType.LessThan);
                AddQueryType(types, QueryType.LessEqual);
                AddQueryType(types, QueryType.GreatThan);
                AddQueryType(types, QueryType.GreatEqual);
                AddQueryType(types, QueryType.Batch);
                break;
            case FieldType.Date:
                AddQueryType(types, QueryType.Between);
                AddQueryType(types, QueryType.BetweenNotEqual);
                AddQueryType(types, QueryType.BetweenLessEqual);
                AddQueryType(types, QueryType.BetweenGreatEqual);
                break;
            default:
                AddQueryType(types, QueryType.Equal);
                AddQueryType(types, QueryType.NotEqual);
                AddQueryType(types, QueryType.Contain);
                AddQueryType(types, QueryType.StartWith);
                AddQueryType(types, QueryType.EndWith);
                AddQueryType(types, QueryType.Batch);
                break;
        }
        return types;
    }

    private void AddQueryType(List<CodeInfo> types, QueryType type)
    {
        var queryType = QueryTypes.FirstOrDefault(t => t.Code == $"{(int)type}");
        queryType.Name = Language[$"QueryType.{type}"];
        types.Add(queryType);
    }
}