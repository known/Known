namespace Known.Pages;

/// <summary>
/// WebApi文档组件类。
/// </summary>
public class WebApiList : BaseTable<ApiMethodInfo>
{
    /// <summary>
    /// 异步初始化组件。
    /// </summary>
    /// <returns></returns>
    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        Table.ShowPager = true;
        Table.OnQuery = OnQueryApisAsync;
        Table.AddColumn(c => c.HttpMethod).Width(90).Template(BuildMethod);
        Table.AddColumn(c => c.Route, true).Width(250).Template((b, r) => b.Tag(r.Route));
        Table.AddColumn(c => c.Description);
        Table.AddAction(nameof(Test));
    }

    /// <summary>
    /// 测试WebApi功能。
    /// </summary>
    public void Test(ApiMethodInfo row)
    {
        var model = new FormModel<ApiMethodInfo>(this);
        model.Title = $"WebApi{Language["Test"]}";
        model.SmallLabel = true;
        model.Data = row;
        model.AddRow().AddColumn("Route", b =>
        {
            b.Text(row.Route);
            BuildMethod(b, row);
        });
        model.AddRow().AddColumn("Description", row.Description);
        UI.ShowForm(model);
    }

    private static void BuildMethod(RenderTreeBuilder builder, ApiMethodInfo row)
    {
        var text = row.HttpMethod.Method;
        var color = text == "GET" ? "cyan" : "blue";
        builder.Tag(text, color);
    }

    private Task<PagingResult<ApiMethodInfo>> OnQueryApisAsync(PagingCriteria criteria)
    {
        var methods = Config.ApiMethods;
        var cq = criteria.Query?.FirstOrDefault(q => q.Id == nameof(ApiMethodInfo.Route));
        if (cq != null && !string.IsNullOrWhiteSpace(cq.Value))
            methods = methods.Where(m => m.Route.Contains(cq.Value, StringComparison.OrdinalIgnoreCase)).ToList();

        var pageData = methods.Skip((criteria.PageIndex - 1) * criteria.PageSize)
                              .Take(criteria.PageSize)
                              .ToList();
        var result = new PagingResult<ApiMethodInfo>(methods.Count, pageData);
        return Task.FromResult(result);
    }
}