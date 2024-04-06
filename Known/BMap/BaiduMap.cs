using Known.Blazor;
using Known.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;

namespace Known.BMap;

public class BaiduMap : BaseComponent
{
    private readonly string id;
    private string key = string.Empty;
    private IJSObjectReference module;
    private DotNetObjectReference<BaiduMap> Instance { get; set; }

    public BaiduMap()
    {
        id = Utils.GetGuid();
        id = $"map-{id}";
    }

    [Inject] private IJSRuntime Js { get; set; }
    [Inject] private IConfiguration Config { get; set; }

    [Parameter] public string Key { get; set; }
    [Parameter] public Func<Location, Task> OnLocation { get; set; }
    [Parameter] public Func<List<SearchResult>, Task> OnSearch { get; set; }

    public string Style { get; set; } = "height:250px;width:100%;";

    protected override void BuildRender(RenderTreeBuilder builder)
    {
        builder.Div().Id(id).Style(Style).Close();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            //在 IConfiguration 服务获取 "BaiduKey" , 默认在 appsettings.json 文件配置
            key = Key ?? Config["BaiduMapKey"];
            module = await Js.InvokeAsync<IJSObjectReference>("import", "./_content/Known/libs/baidumap.js");
            Instance = DotNetObjectReference.Create(this);
            while (!await InitMapScript())
            {
                await Task.Delay(500);
            }
        }
    }

    private async Task<bool> InitMapScript() => await module!.InvokeAsync<bool>("addScript", [key, id, Instance]);

    public async Task ResetAsync() => await module!.InvokeVoidAsync("resetMaps");
    public async Task SearchAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name) || Instance == null)
            return;

        await module!.InvokeVoidAsync("searchLocation", [name, Instance]);
    }

    [JSInvokable]
    public async Task GetSearch(List<SearchResult> results)
    {
        if (OnSearch != null)
            await OnSearch.Invoke(results);
    }

    [JSInvokable]
    public async Task GetLocation(Location geolocations)
    {
        if (OnLocation != null)
            await OnLocation.Invoke(geolocations);
    }
}