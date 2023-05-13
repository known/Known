namespace Known.Razor.Templates;

class RenderedComponent<T> where T : IComponent
{
    private readonly HtmlRenderer renderer;
    private readonly ContainerComponent container;
    private int testId;
    private T testInstance;

    internal RenderedComponent(HtmlRenderer renderer)
    {
        this.renderer = renderer;
        container = new ContainerComponent(this.renderer);
    }

    public T Instance => testInstance;

    public string GetMarkup()
    {
        return Htmlizer.GetHtml(renderer, testId);
    }

    internal void SetParametersAndRender(ParameterView parameters)
    {
        container.RenderComponentUnderTest(typeof(T), parameters);
        var foundTestComponent = container.FindComponentUnderTest();
        testId = foundTestComponent.Item1;
        testInstance = (T)foundTestComponent.Item2;
    }
}