namespace Known.Renders;

public abstract class BaseRender<T> where T : BaseComponent, new()
{
    public T Component { get; private set; }
    public UIService UI => Component.UI;

    public void BuildTree(T component, RenderTreeBuilder builder)
    {
        Component = component;
        BuildRenderTree(builder);
    }

    protected abstract void BuildRenderTree(RenderTreeBuilder builder);
}