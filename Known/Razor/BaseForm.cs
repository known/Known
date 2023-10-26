namespace Known.Razor;

public class BaseForm<T> : KForm
{
    protected T TModel => (T)Model;
    protected List<KMenuItem> TabItems { get; set; }

    protected Field Field(Expression<Func<T, object>> selector)
    {
        var property = TypeHelper.Property(selector);
        return Fields[property.Name];
    }

    protected TField Field<TField>(Expression<Func<T, object>> selector) where TField : Field
    {
        var property = TypeHelper.Property(selector);
        return FieldAs<TField>(property.Name);
    }

    internal override void BuildForm(RenderTreeBuilder builder)
    {
        //if (!Context.Check.IsCheckKey)
        //{
        //    BuildAuthorize(builder);
        //    return;
        //}

        if (TabItems != null && TabItems.Count > 0)
        {
            builder.Component<KTabs>()
                   .Set(c => c.Items, TabItems)
                   .Set(c => c.Body, BuildBody)
                   .Build();
        }
        else
        {
            base.BuildForm(builder);
        }
    }

    protected override void BuildFields(RenderTreeBuilder builder) => BuildFields(new FieldBuilder<T>(builder));
    protected virtual void BuildFields(FieldBuilder<T> builder) { }
    protected virtual void BuildTabBody(RenderTreeBuilder builder, KMenuItem item) { }

    protected void BuildBody(RenderTreeBuilder builder, KMenuItem item)
    {
        if (item.Name == TabItems[0].Name)
            base.BuildForm(builder);
        else
            BuildTabBody(builder, item);
    }
}