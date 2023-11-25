using AntDesign;
using Known.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace Known.AntBlazor;

public static class Extension
{
    public static void AddKnownAntDesign(this IServiceCollection services, Action<AntDesignOption> action = null)
    {
        //添加AntDesign
        services.AddAntDesign();

        KaConfig.Option = new AntDesignOption();
        action?.Invoke(KaConfig.Option);

        Config.AddModule(typeof(Extension).Assembly);
        services.AddScoped<IUIService, UIService>();
    }

    internal static CheckboxOption[] ToOptions(this List<CodeInfo> codes, Action<CheckboxOption> action = null)
    {
        return codes.Select(a =>
        {
            var option = new CheckboxOption
            {
                Label = a.Name,
                Value = a.Code
            };
            action?.Invoke(option);
            return option;
        }).ToArray();
    }
}

public class AntDesignOption
{
    public RenderFragment Footer { get; set; }
}

class KaConfig
{
    public static AntDesignOption Option { get; set; }
}