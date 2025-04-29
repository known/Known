using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Known.Internals;

/// <summary>
/// 打印组件呈现类。
/// </summary>
/// <typeparam name="T">组件类型。</typeparam>
class ComponentRenderer<T> : IPrintRenderer<T> where T : Microsoft.AspNetCore.Components.IComponent
{
    private const string ChildContent = nameof(ChildContent);
    private static readonly Type componentType = typeof(T);

    private readonly Dictionary<string, object> parameters = new(StringComparer.Ordinal);
    private readonly ComTemplater templater;

    /// <summary>
    /// 构造函数，创建一个打印组件呈现类的实例。
    /// </summary>
    public ComponentRenderer()
    {
        templater = new ComTemplater();
    }

    internal ComponentRenderer<T> AddServiceProvider(IServiceProvider serviceProvider)
    {
        templater.AddServiceProvider(serviceProvider);
        return this;
    }

    /// <summary>
    /// 设置打印组件参数。
    /// </summary>
    /// <typeparam name="TValue">组件参数对象类型。</typeparam>
    /// <param name="selector">组件参数属性选择表达式。</param>
    /// <param name="value">组件参数对象。</param>
    /// <returns>打印组件呈现对象。</returns>
    /// <exception cref="ArgumentNullException">参数值为空异常。</exception>
    public IPrintRenderer<T> Set<TValue>(Expression<Func<T, TValue>> selector, TValue value)
    {
        if (value is null)
            throw new ArgumentNullException(nameof(value));

        parameters.Add(GetParameterName(selector), value);
        return this;
    }

    private static string GetParameterName<TValue>(Expression<Func<T, TValue>> selector)
    {
        if (selector is null)
            throw new ArgumentNullException(nameof(selector));

        if (selector.Body is not MemberExpression memberExpression ||
            memberExpression.Member is not PropertyInfo propInfoCandidate)
            throw new ArgumentException($"The parameter selector '{selector}' does not resolve to a public property on the component '{typeof(T)}'.", nameof(selector));

        var propertyInfo = propInfoCandidate.DeclaringType != componentType
            ? componentType.GetProperty(propInfoCandidate.Name, propInfoCandidate.PropertyType)
            : propInfoCandidate;

        var paramAttr = propertyInfo?.GetCustomAttribute<ParameterAttribute>(inherit: true);

        if (propertyInfo is null || paramAttr is null)
            throw new ArgumentException($"The parameter selector '{selector}' does not resolve to a public property on the component '{typeof(T)}' with a [Parameter] or [CascadingParameter] attribute.", nameof(selector));

        return propertyInfo.Name;
    }

    internal string Render() => templater.RenderComponent<T>(parameters);
}

class ComTemplater
{
    private readonly ComServiceCollection services = [];
    private readonly Lazy<IServiceProvider> provider;
    private readonly Lazy<ComHtmlRenderer> renderer;

    public ComTemplater()
    {
        // define a lazy service provider
        provider = new Lazy<IServiceProvider>(() =>
        {
            // creates a service provider from all providers
            var factory = new ComServiceProviderFactory();
            return factory.CreateServiceProvider(factory.CreateBuilder(services));
        });

        // define lazy renderer
        renderer = new Lazy<ComHtmlRenderer>(() =>
        {
            var loggerFactory = Services.GetService<ILoggerFactory>() ?? new NullLoggerFactory();
            return new ComHtmlRenderer(Services, loggerFactory);
        });
    }

    public IServiceProvider Services => provider.Value;

    private ComHtmlRenderer Renderer => renderer.Value;
    //private readonly Type layout;

    public void AddServiceProvider(IServiceProvider serviceProvider)
    {
        // add service provider if not present
        if (!services.Contains(serviceProvider))
            services.Add(serviceProvider);
    }

    public string RenderComponent<TComponent>(IDictionary<string, object> parameters = null) where TComponent : Microsoft.AspNetCore.Components.IComponent
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
        var layoutView = new ComRenderedComponent<LayoutView>(Renderer);
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

    private static readonly Type _iComponentType = typeof(Microsoft.AspNetCore.Components.IComponent);

    private static ParameterView GetParameterView(IDictionary<string, object> parameters)
    {
        if (parameters == null) return ParameterView.Empty;
        return ParameterView.FromDictionary(parameters);
    }

    private Type GetLayout(Type componentType)
    {
        // Use layout override if set
        //if (layout != null)
        //    return layout;

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

class ComServiceProviderFactory : IServiceProviderFactory<ComServiceBuilder>
{
    public ComServiceBuilder CreateBuilder(IServiceCollection services)
    {
        var provider = services.BuildServiceProvider();
        if (services is IEnumerable<IServiceProvider> providers)
        {
            return new(provider, providers);
        }
        return new(provider);
    }

    public IServiceProvider CreateServiceProvider(ComServiceBuilder builder) => builder.Build();
}

class ComServiceBuilder(IServiceProvider provider, IEnumerable<IServiceProvider> providers = null)
{
    private readonly IServiceProvider provider = provider;
    private readonly IEnumerable<IServiceProvider> providers = providers;

    public IServiceProvider Build()
    {
        if (providers is IEnumerable<IServiceProvider> comProviders)
        {
            var scopes = comProviders.Select(sp => sp.CreateScope());
            return new ComServiceProvider(provider, scopes);
        }
        return provider;
    }
}

class ComServiceProvider(IServiceProvider provider, IEnumerable<IServiceScope> scopes) : IServiceProvider, IDisposable
{
    private readonly IServiceProvider provider = provider;
    private readonly IEnumerable<IServiceScope> scopes = [.. scopes];

    public object GetService(Type serviceType)
    {
        if (provider.GetService(serviceType) is object foundService)
            return foundService;

        foreach (var scope in scopes)
        {
            if (scope.ServiceProvider.GetService(serviceType) is object serviceFromScope)
                return serviceFromScope;
        }

        return null;
    }

    public void Dispose()
    {
        foreach (var scope in scopes) scope.Dispose();
    }
}

class ComServiceCollection : ServiceCollection, ICollection<IServiceProvider>, IEnumerable<IServiceProvider>, IList<IServiceProvider>
{
    private readonly IList<IServiceProvider> providers = [];

    IServiceProvider IList<IServiceProvider>.this[int index]
    {
        get => providers[index];
        set => providers[index] = value;
    }

    public void Add(IServiceProvider item)
    {
        if (!Contains(item))
            providers.Add(item);
    }

    public new void Clear()
    {
        base.Clear();
        providers.Clear();
    }

    public bool Contains(IServiceProvider item) => providers.Contains(item);
    public void CopyTo(IServiceProvider[] array, int arrayIndex) => providers.CopyTo(array, arrayIndex);
    public int IndexOf(IServiceProvider item) => providers.IndexOf(item);
    public void Insert(int index, IServiceProvider item) => providers.Insert(index, item);
    public bool Remove(IServiceProvider item) => providers.Remove(item);

    IEnumerator<IServiceProvider> IEnumerable<IServiceProvider>.GetEnumerator() => providers.GetEnumerator();
}

[SuppressMessage("Usage", "BL0006:Do not use RenderTree types", Justification = "<Pending>")]
class ComHtmlRenderer(IServiceProvider serviceProvider, ILoggerFactory loggerFactory) : Renderer(serviceProvider, loggerFactory)
{
    private Exception unhandledException;
    private TaskCompletionSource<object> nextRenderTcs = new();

    public new ArrayRange<RenderTreeFrame> GetCurrentRenderTreeFrames(int componentId) => base.GetCurrentRenderTreeFrames(componentId);
    public int AttachTestRootComponent(ComContainerComponent testRootComponent) => AssignRootComponentId(testRootComponent);

    public new Task DispatchEventAsync(ulong eventHandlerId, EventFieldInfo fieldInfo, EventArgs eventArgs)
    {
        var task = Dispatcher.InvokeAsync(() => base.DispatchEventAsync(eventHandlerId, fieldInfo, eventArgs));
        AssertNoSynchronousErrors();
        return task;
    }

    public Task NextRender => nextRenderTcs.Task;
    public override Dispatcher Dispatcher { get; } = Dispatcher.CreateDefault();
    protected override void HandleException(Exception exception) => unhandledException = exception;

    protected override Task UpdateDisplayAsync(in RenderBatch renderBatch)
    {
        // TODO: Capture batches (and the state of component output) for individual inspection
        var prevTcs = nextRenderTcs;
        nextRenderTcs = new TaskCompletionSource<object>();
        prevTcs.SetResult(null);
        return Task.CompletedTask;
    }

    public void DispatchAndAssertNoSynchronousErrors(Action callback)
    {
        Dispatcher.InvokeAsync(callback).Wait();
        AssertNoSynchronousErrors();
    }

    private void AssertNoSynchronousErrors()
    {
        if (unhandledException != null)
        {
            ExceptionDispatchInfo.Capture(unhandledException).Throw();
        }
    }
}

[SuppressMessage("Usage", "BL0006:Do not use RenderTree types", Justification = "<Pending>")]
class ComHtmlizer
{
    private static readonly HtmlEncoder htmlEncoder = HtmlEncoder.Default;

    private static readonly HashSet<string> selfClosingElements = new(StringComparer.OrdinalIgnoreCase)
    {
        "area", "base", "br", "col", "embed", "hr", "img", "input", "link", "meta", "param", "source", "track", "wbr"
    };

    public static string GetHtml(ComHtmlRenderer renderer, int componentId)
    {
        var frames = renderer.GetCurrentRenderTreeFrames(componentId);
        var context = new HtmlRenderingContext(renderer);
        var newPosition = RenderFrames(context, frames, 0, frames.Count);
        Debug.Assert(newPosition == frames.Count);
        return string.Join(string.Empty, context.Result);
    }

    private static int RenderFrames(HtmlRenderingContext context, ArrayRange<RenderTreeFrame> frames, int position, int maxElements)
    {
        var nextPosition = position;
        var endPosition = position + maxElements;
        while (position < endPosition)
        {
            nextPosition = RenderCore(context, frames, position);
            if (position == nextPosition)
            {
                throw new InvalidOperationException("We didn't consume any input.");
            }
            position = nextPosition;
        }

        return nextPosition;
    }

    private static int RenderCore(HtmlRenderingContext context, ArrayRange<RenderTreeFrame> frames, int position)
    {
        ref var frame = ref frames.Array[position];
        switch (frame.FrameType)
        {
            case RenderTreeFrameType.Element:
                return RenderElement(context, frames, position);
            case RenderTreeFrameType.Attribute:
                throw new InvalidOperationException($"Attributes should only be encountered within {nameof(RenderElement)}");
            case RenderTreeFrameType.Text:
                context.Result.Add(htmlEncoder.Encode(frame.TextContent));
                return ++position;
            case RenderTreeFrameType.Markup:
                context.Result.Add(frame.MarkupContent);
                return ++position;
            case RenderTreeFrameType.Component:
                return RenderChildComponent(context, frames, position);
            case RenderTreeFrameType.Region:
                return RenderFrames(context, frames, position + 1, frame.RegionSubtreeLength - 1);
            case RenderTreeFrameType.ElementReferenceCapture:
            case RenderTreeFrameType.ComponentReferenceCapture:
                return ++position;
            default:
                throw new InvalidOperationException($"Invalid element frame type '{frame.FrameType}'.");
        }
    }

    private static int RenderChildComponent(HtmlRenderingContext context, ArrayRange<RenderTreeFrame> frames, int position)
    {
        ref var frame = ref frames.Array[position];
        var childFrames = context.Renderer.GetCurrentRenderTreeFrames(frame.ComponentId);
        RenderFrames(context, childFrames, 0, childFrames.Count);
        return position + frame.ComponentSubtreeLength;
    }

    private static int RenderElement(HtmlRenderingContext context, ArrayRange<RenderTreeFrame> frames, int position)
    {
        ref var frame = ref frames.Array[position];
        var result = context.Result;
        result.Add("<");
        result.Add(frame.ElementName);
        var afterAttributes = RenderAttributes(context, frames, position + 1, frame.ElementSubtreeLength - 1, out var capturedValueAttribute);

        // When we see an <option> as a descendant of a <select>, and the option's "value" attribute matches the
        // "value" attribute on the <select>, then we auto-add the "selected" attribute to that option. This is
        // a way of converting Blazor's select binding feature to regular static HTML.
        if (context.ClosestSelectValueAsString != null
            && string.Equals(frame.ElementName, "option", StringComparison.OrdinalIgnoreCase)
            && string.Equals(capturedValueAttribute, context.ClosestSelectValueAsString, StringComparison.Ordinal))
        {
            result.Add(" selected");
        }

        var remainingElements = frame.ElementSubtreeLength + position - afterAttributes;
        if (remainingElements > 0)
        {
            result.Add(">");

            var isSelect = string.Equals(frame.ElementName, "select", StringComparison.OrdinalIgnoreCase);
            if (isSelect)
            {
                context.ClosestSelectValueAsString = capturedValueAttribute;
            }

            var afterElement = RenderChildren(context, frames, afterAttributes, remainingElements);

            if (isSelect)
            {
                // There's no concept of nested <select> elements, so as soon as we're exiting one of them,
                // we can safely say there is no longer any value for this
                context.ClosestSelectValueAsString = null;
            }

            result.Add("</");
            result.Add(frame.ElementName);
            result.Add(">");
            Debug.Assert(afterElement == position + frame.ElementSubtreeLength);
            return afterElement;
        }
        else
        {
            if (selfClosingElements.Contains(frame.ElementName))
            {
                result.Add(" />");
            }
            else
            {
                result.Add(">");
                result.Add("</");
                result.Add(frame.ElementName);
                result.Add(">");
            }
            Debug.Assert(afterAttributes == position + frame.ElementSubtreeLength);
            return afterAttributes;
        }
    }

    private static int RenderChildren(HtmlRenderingContext context, ArrayRange<RenderTreeFrame> frames, int position, int maxElements)
    {
        if (maxElements == 0)
            return position;

        return RenderFrames(context, frames, position, maxElements);
    }

    private static int RenderAttributes(HtmlRenderingContext context, ArrayRange<RenderTreeFrame> frames, int position, int maxElements, out string capturedValueAttribute)
    {
        capturedValueAttribute = null;

        if (maxElements == 0)
            return position;

        var result = context.Result;

        for (var i = 0; i < maxElements; i++)
        {
            var candidateIndex = position + i;
            ref var frame = ref frames.Array[candidateIndex];
            if (frame.FrameType != RenderTreeFrameType.Attribute)
                return candidateIndex;

            if (frame.AttributeName.Equals("value", StringComparison.OrdinalIgnoreCase))
                capturedValueAttribute = frame.AttributeValue as string;

            if (frame.AttributeEventHandlerId > 0)
            {
                result.Add($" {frame.AttributeName}=\"{frame.AttributeEventHandlerId}\"");
                continue;
            }

            switch (frame.AttributeValue)
            {
                case bool flag when flag:
                    result.Add(" ");
                    result.Add(frame.AttributeName);
                    break;
                case string value:
                    result.Add(" ");
                    result.Add(frame.AttributeName);
                    result.Add("=");
                    result.Add("\"");
                    result.Add(htmlEncoder.Encode(value));
                    result.Add("\"");
                    break;
                default:
                    break;
            }
        }

        return position + maxElements;
    }

    private class HtmlRenderingContext(ComHtmlRenderer renderer)
    {
        public ComHtmlRenderer Renderer { get; } = renderer;
        public List<string> Result { get; } = new List<string>();
        public string ClosestSelectValueAsString { get; set; }
    }
}

class ComRenderedComponent<T> where T : Microsoft.AspNetCore.Components.IComponent
{
    private readonly ComHtmlRenderer renderer;
    private readonly ComContainerComponent container;
    private int testId;
    private T testInstance;

    internal ComRenderedComponent(ComHtmlRenderer renderer)
    {
        this.renderer = renderer;
        container = new ComContainerComponent(this.renderer);
    }

    public T Instance => testInstance;

    public string GetMarkup() => ComHtmlizer.GetHtml(renderer, testId);

    internal void SetParametersAndRender(ParameterView parameters)
    {
        container.RenderComponentUnderTest(typeof(T), parameters);
        var foundTestComponent = container.FindComponentUnderTest();
        testId = foundTestComponent.Item1;
        testInstance = (T)foundTestComponent.Item2;
    }
}

[SuppressMessage("Usage", "BL0006:Do not use RenderTree types", Justification = "<Pending>")]
class ComContainerComponent : Microsoft.AspNetCore.Components.IComponent
{
    private readonly ComHtmlRenderer renderer;
    private readonly int componentId;
    private RenderHandle renderHandle;

    public ComContainerComponent(ComHtmlRenderer renderer)
    {
        this.renderer = renderer;
        componentId = renderer.AttachTestRootComponent(this);
    }

    public void Attach(RenderHandle renderHandle) => this.renderHandle = renderHandle;
    public Task SetParametersAsync(ParameterView parameters) => throw new NotImplementedException($"{nameof(ComContainerComponent)} shouldn't receive any parameters");

    public (int, object) FindComponentUnderTest()
    {
        var ownFrames = renderer.GetCurrentRenderTreeFrames(componentId);
        if (ownFrames.Count == 0)
            throw new InvalidOperationException($"{nameof(ComContainerComponent)} hasn't yet rendered");

        ref var childFrame = ref ownFrames.Array[0];
        Debug.Assert(childFrame.FrameType == RenderTreeFrameType.Component);
        Debug.Assert(childFrame.Component != null);
        return (childFrame.ComponentId, childFrame.Component);
    }

    public void RenderComponentUnderTest(Type componentType, ParameterView parameters)
    {
        renderer.DispatchAndAssertNoSynchronousErrors(() =>
        {
            renderHandle.Render(builder =>
            {
                builder.OpenComponent(0, componentType);

                foreach (var parameterValue in parameters)
                {
                    builder.AddAttribute(1, parameterValue.Name, parameterValue.Value);
                }

                builder.CloseComponent();
            });
        });
    }
}