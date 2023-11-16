using AntDesign;
using Known;
using Known.Razor;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace KnownAntDesign;

public static class Extension
{
    public static void AddKAntDesign(this IServiceCollection services, Action<AntDesignOption> action = null)
    {
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