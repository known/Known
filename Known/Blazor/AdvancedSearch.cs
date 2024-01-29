using Known.Extensions;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace Known.Blazor;

class AdvancedSearch : BaseComponent
{
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

        Query.Clear();
        var items = await Platform.Setting.GetUserSettingAsync<List<QueryInfo>>(SettingKey);
        if (items != null && items.Count > 0)
            Query.AddRange(items);
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Div("kui-advanced-search", () =>
        {
            UI.Button(builder, new ActionInfo("New"), this.Callback<MouseEventArgs>(e => OnAdd()));
            foreach (var item in Query)
            {
                builder.Div("item", () =>
                {
                    UI.BuildSelect(builder, new InputModel<string>
                    {
                    });
                    UI.BuildSelect(builder, new InputModel<string>
                    {
                    });
                    BuildQueryValue(builder, item);
                    UI.Button(builder, new ActionInfo("Delete"), this.Callback<MouseEventArgs>(e => OnDelete(item)));
                });
            }
        });
    }

    private void BuildQueryValue(RenderTreeBuilder builder, QueryInfo item)
    {
        UI.BuildText(builder, new InputModel<string>
        {

        });
    }

    private void OnAdd() => Query.Add(new QueryInfo("", ""));
    private void OnDelete(QueryInfo item) => Query.Remove(item);
}