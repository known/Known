namespace Known.Razor;

public class ComponentBuilder<T> : AttributeBuilder<T> where T : BaseComponent
{
    internal ComponentBuilder(RenderTreeBuilder builder) : base(builder) { }

    public ComponentBuilder<T> Id(string id)
    {
        Set(r => r.Id, id);
        return this;
    }

    public ComponentBuilder<T> ReadOnly(bool readOnly)
    {
        Set(r => r.ReadOnly, readOnly);
        return this;
    }

    public ComponentBuilder<T> Enabled(bool enabled)
    {
        Set(r => r.Enabled, enabled);
        return this;
    }
}