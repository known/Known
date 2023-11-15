using AntDesign;
using Known;
using Known.Razor;
using Microsoft.Extensions.DependencyInjection;

namespace KnownAntDesign;

public static class Extension
{
    public static void AddKAntDesign(this IServiceCollection services)
    {
        //添加模块
        Config.AddModule(typeof(Extension).Assembly);
        //添加UI服务
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