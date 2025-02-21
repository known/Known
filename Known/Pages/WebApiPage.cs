using System.Web;

namespace Known.Pages;

/// <summary>
/// WebApi文档组件类。
/// </summary>
[StreamRendering]
[Route("/dev/webapi")]
[DevPlugin("WebApi", "pull-request", Sort = 2)]
public class WebApiPage : BaseTablePage<ApiMethodInfo>
{
    /// <summary>
    /// 异步初始化组件。
    /// </summary>
    /// <returns></returns>
    protected override async Task OnInitPageAsync()
    {
        if (!CurrentUser.IsSystemAdmin())
        {
            Navigation.GoErrorPage("403");
            return;
        }

        await base.OnInitPageAsync();

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
        var model = new DialogModel();
        model.Title = $"WebApi{Language["Test"]}";
        model.Width = 600;
        model.Content = b => b.Component<WebApiForm>().Set(c => c.Model, row).Build();
        UI.ShowDialog(model);
    }

    internal static void BuildMethod(RenderTreeBuilder builder, ApiMethodInfo row)
    {
        var text = row.HttpMethod.Method;
        var color = "blue";
        if (row.HttpMethod == HttpMethod.Get)
            color = "cyan";
        else if (row.HttpMethod == HttpMethod.Delete)
            color = "red";
        else if (row.HttpMethod == HttpMethod.Put)
            color = "purple";
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

class WebApiForm : BaseComponent
{
    private readonly Dictionary<string, string> request = [];
    private string postData = "";
    private string result = "";

    [Parameter] public ApiMethodInfo Model { get; set; }

    protected override void BuildRender(RenderTreeBuilder builder)
    {
        builder.Div("kui-api-name", Model.Description);
        builder.Div("kui-api-route", () =>
        {
            builder.Div(() =>
            {
                WebApiPage.BuildMethod(builder, Model);
                builder.Text($"/api{Model.Route}");
            });
            builder.Button(Language["Execute"], this.Callback<MouseEventArgs>(OnExexuteAsync));
        });
        builder.Div("kui-api-row", () =>
        {
            builder.Div("kui-api-title", Language["RequestHeaders"]);
            BuildHeaders(builder);
        });
        if (Model.Parameters != null && Model.Parameters.Length > 0)
        {
            builder.Div("kui-api-row", () =>
            {
                builder.Div("kui-api-title", Language["RequestParameters"]);
                BuildParamters(builder);
            });
        }
        builder.Div("kui-api-row", () =>
        {
            builder.Div("kui-api-title", Language["ResponseResults"]);
            BuildResult(builder);
        });
    }

    private void BuildHeaders(RenderTreeBuilder builder)
    {
        builder.Ul(() =>
        {
            builder.Li(() =>
            {
                var token = CurrentUser.Token;
                var chars = "********";
                var oldValue1 = token.Substring(4, 8);
                var oldValue2 = token.Substring(16, 8);
                var value = token.Replace(oldValue1, chars).Replace(oldValue2, chars);
                BuildLabel(builder, "String", Constants.KeyToken);
                builder.TextBox(new InputModel<string> { Value = value, Disabled = true });
            });
        });
    }

    private void BuildParamters(RenderTreeBuilder builder)
    {
        if (Model.HttpMethod == HttpMethod.Post)
        {
            var param = Model.Parameters[0];
            if (param.ParameterType != typeof(string))
            {
                var value = Activator.CreateInstance(param.ParameterType);
                postData = FormatJson(value);
                builder.TextArea(new InputModel<string>
                {
                    Rows = 6,
                    Value = postData,
                    ValueChanged = this.Callback<string>(value => request[param.Name] = value)
                });
            }
            else
            {
                BuildGetParameters(builder);
            }
        }
        else
        {
            BuildGetParameters(builder);
        }
    }

    private void BuildGetParameters(RenderTreeBuilder builder)
    {
        builder.Ul(() =>
        {
            foreach (var param in Model.Parameters)
            {
                builder.Li(() =>
                {
                    BuildLabel(builder, param.ParameterType.Name, param.Name);
                    builder.TextBox(new InputModel<string>
                    {
                        Value = "",
                        ValueChanged = this.Callback<string>(value => request[param.Name] = value)
                    });
                });
            }
        });
    }

    private static void BuildLabel(RenderTreeBuilder builder, string type, string name)
    {
        builder.Div("kui-api-label", () =>
        {
            builder.Tag(type, "geekblue");
            builder.Label(name);
        });
    }

    private void BuildResult(RenderTreeBuilder builder)
    {
        var value = Utils.FromJson<object>(result);
        result = FormatJson(value);
        builder.Pre().Class("kui-api-result").Child(result);
    }

    private async Task OnExexuteAsync(MouseEventArgs args)
    {
        var client = new HttpClient();
        client.DefaultRequestHeaders.Add(Constants.KeyToken, CurrentUser.Token);
        if (Model.HttpMethod == HttpMethod.Post)
        {
            var content = GetPostContent();
            var url = GetRequestUrl(Model.Route);
            var res = await client.PostAsync(url, content);
            result = await res.Content.ReadAsStringAsync();
        }
        else
        {
            var url = GetRequestUrl();
            result = await client.GetStringAsync(url);
        }
        await StateChangedAsync();
    }

    private string GetRequestUrl()
    {
        var url = Model.Route;
        if (Model.Parameters != null && Model.Parameters.Length > 0)
        {
            var parameters = new List<string>();
            foreach (var param in Model.Parameters)
            {
                var value = HttpUtility.UrlEncode(request.GetValue<string>(param.Name));
                parameters.Add($"{param.Name}={value}");
            }
            url += "?" + string.Join("&", parameters);
        }
        return GetRequestUrl(url);
    }

    private static string GetRequestUrl(string url)
    {
        return $"{Config.HostUrl}/api{url}";
    }

    private StringContent GetPostContent()
    {
        if (Model.Parameters == null || Model.Parameters.Length == 0)
            return null;

        var param = Model.Parameters[0];
        var json = request.GetValue<string>(param.Name);
        if (string.IsNullOrWhiteSpace(json))
            json = postData;
        return new StringContent(json, Encoding.UTF8, "application/json");
    }

    private static string FormatJson(object value)
    {
        return JsonSerializer.Serialize(value, new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        });
    }
}