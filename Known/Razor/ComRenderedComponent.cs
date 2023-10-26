namespace Known.Razor;

class ComRenderedComponent<T> where T : IComponent
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

    public string GetMarkup()
    {
        return ComHtmlizer.GetHtml(renderer, testId);
    }

    internal void SetParametersAndRender(ParameterView parameters)
    {
        container.RenderComponentUnderTest(typeof(T), parameters);
        var foundTestComponent = container.FindComponentUnderTest();
        testId = foundTestComponent.Item1;
        testInstance = (T)foundTestComponent.Item2;
    }
}