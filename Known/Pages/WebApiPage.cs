using System.Web;

namespace Known.Pages;

/// <summary>
/// WebApi文档组件类。
/// </summary>
[Route("/dev/webapi")]
[DevPlugin("WebApi", "pull-request", Sort = 99)]
public class WebApiPage : BaseTablePage<ApiMethodInfo>
{
    private IWebApiService Service;

    /// <inheritdoc />
    protected override async Task OnInitPageAsync()
    {
        if (!CurrentUser.IsSystemAdmin())
        {
            Navigation.GoErrorPage("403");
            return;
        }

        await base.OnInitPageAsync();
        Service = await CreateServiceAsync<IWebApiService>();

        Table.SetDevTable();
        Table.OnQuery = Service.QueryWebApisAsync;
        Table.AddColumn(c => c.HttpMethod).Width(120).Template(BuildMethod);
        Table.AddColumn(c => c.Route, true).Width(250).Tag().FilterType(false);
        Table.AddColumn(c => c.Description).Filter(false);
        Table.AddAction(nameof(Test));
    }

    /// <summary>
    /// 测试WebApi功能。
    /// </summary>
    public void Test(ApiMethodInfo row)
    {
        var model = new DialogModel
        {
            Title = $"WebApi{Language[Language.Test]}",
            Width = 600,
            Content = b => b.Component<WebApiForm>().Set(c => c.Model, row).Build()
        };
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
}

class WebApiForm : BaseComponent
{
    private readonly Dictionary<string, string> request = [];
    private string postData = "";
    private string result = "";

    [Parameter] public ApiMethodInfo Model { get; set; }

    protected override void BuildRender(RenderTreeBuilder builder)
    {
        builder.Div().Style("padding:10px").Child(() =>
        {
            builder.Div("kui-api-name", Model.Description);
            builder.Div("kui-api-route", () =>
            {
                builder.Div(() =>
                {
                    WebApiPage.BuildMethod(builder, Model);
                    builder.Text($"/api{Model.Route}");
                });
                builder.Button(Language.Execute, this.Callback<MouseEventArgs>(OnExexuteAsync));
            });
            builder.Div("kui-api-row", () =>
            {
                builder.Div("kui-api-title", Language[Language.RequestHeaders]);
                BuildHeaders(builder);
            });
            if (Model.Parameters != null && Model.Parameters.Length > 0)
            {
                builder.Div("kui-api-row", () =>
                {
                    builder.Div("kui-api-title", Language[Language.RequestParameters]);
                    BuildParamters(builder);
                });
            }
            builder.Div("kui-api-row", () =>
            {
                builder.Div("kui-api-title", Language[Language.ResponseResults]);
                BuildResult(builder);
            });
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
                if (string.IsNullOrWhiteSpace(postData))
                {
                    var value = Activator.CreateInstance(param.ParameterType);
                    postData = FormatJson(value);
                }
                builder.TextArea(new InputModel<string>
                {
                    Rows = 6,
                    Value = postData,
                    ValueChanged = this.Callback<string>(value =>
                    {
                        postData = value;
                        request[param.Name] = value;
                    })
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
                        Value = request.GetValue<string>(param.Name),
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
        if (Utils.IsJson(result))
        {
            var value = Utils.FromJson<object>(result);
            result = FormatJson(value);
        }
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

    private HttpContent GetPostContent()
    {
        if (Model.Parameters == null || Model.Parameters.Length == 0)
            return null;

        if (Model.Parameters.Length > 1)
            return new FormUrlEncodedContent(request);

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