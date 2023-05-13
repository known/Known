using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Known.Razor.Templates;

class Templater
{
    private readonly ComServiceCollection services = new();
    private readonly Lazy<IServiceProvider> provider;
    private readonly Lazy<HtmlRenderer> renderer;

    public Templater()
    {
        // define a lazy service provider
        provider = new Lazy<IServiceProvider>(() =>
        {
            // creates a service provider from all providers
            var factory = new ComServiceProviderFactory();
            return factory.CreateServiceProvider(factory.CreateBuilder(services));
        });

        // define lazy renderer
        renderer = new Lazy<HtmlRenderer>(() =>
        {
            var loggerFactory = Services.GetService<ILoggerFactory>() ?? new NullLoggerFactory();
            return new HtmlRenderer(Services, loggerFactory);
        });
    }

    public IServiceProvider Services => provider.Value;

    private HtmlRenderer Renderer => renderer.Value;
    private readonly Type layout;

    public void AddServiceProvider(IServiceProvider serviceProvider)
    {
        // add service provider if not present
        if (!services.Contains(serviceProvider))
            services.Add(serviceProvider);
    }

    public string RenderComponent<TComponent>(IDictionary<string, object> parameters = null) where TComponent : IComponent
    {
        var componentType = typeof(TComponent);
        return RenderComponent(componentType, parameters);
    }

    public string RenderComponent(Type componentType, IDictionary<string, object> parameters = null)
    {
        ValidateComponentType(componentType);
        var layout = GetLayout(componentType);

        // create a RenderFragment from the component
        var childContent = (RenderFragment)(builder =>
        {
            builder.OpenComponent(0, componentType);

            // add parameters if any
            if (parameters != null && parameters.Any())
                builder.AddMultipleAttributes(1, parameters);
            builder.CloseComponent();
        });

        // render a LayoutView and use the TComponent as the child content
        var layoutView = new RenderedComponent<LayoutView>(Renderer);
        var layoutParams = new Dictionary<string, object>()
        {
            { nameof(LayoutView.Layout), layout },
            { nameof(LayoutView.ChildContent), childContent }
        };
        layoutView.SetParametersAndRender(GetParameterView(layoutParams));

        return layoutView.GetMarkup();
    }

    private static void ValidateComponentType(Type componentType)
    {
        if (!_iComponentType.IsAssignableFrom(componentType))
            throw new ArgumentException("Type must implement IComponent", nameof(componentType));
    }

    private static readonly Type _iComponentType = typeof(IComponent);

    private static ParameterView GetParameterView(IDictionary<string, object> parameters)
    {
        if (parameters == null) return ParameterView.Empty;
        return ParameterView.FromDictionary(parameters);
    }

    private Type GetLayout(Type componentType)
    {
        // Use layout override if set
        if (layout != null)
            return layout;

        // check top-level component for a layout attribute
        return GetLayoutFromAttribute(componentType);
    }

    public static Type GetLayoutFromAttribute(Type componentType)
    {
        var layoutAttrs = (LayoutAttribute[])componentType.GetCustomAttributes(typeof(LayoutAttribute), true);
        if (layoutAttrs != null && layoutAttrs.Length > 0)
            return layoutAttrs[0].LayoutType;
        else
            return null;
    }
}