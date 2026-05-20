using Microsoft.AspNetCore.Components.Routing;

namespace Known.Internals;

/// <summary>
/// 顶部面包屑组件类。
/// </summary>
public partial class TopBreadcrumb
{
    /// <summary>
    /// 取得或设置首页点击委托。
    /// </summary>
    [Parameter] public EventCallback OnHome { get; set; }

    /// <inheritdoc />
    protected override Task OnInitAsync()
    {
        Navigation.LocationChanged += OnLocationChanged;
        return base.OnInitAsync();
    }

    /// <inheritdoc />
    protected override Task OnDisposeAsync()
    {
        Navigation.LocationChanged -= OnLocationChanged;
        return base.OnDisposeAsync();
    }

    private void OnHomeClick()
    {
        if (OnHome.HasDelegate)
            OnHome.InvokeAsync();
        else
            Context.GoHomePage();
    }

    private void OnLocationChanged(object sender, LocationChangedEventArgs e)
    {
        InvokeAsync(StateHasChanged);
    }
}