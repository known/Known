﻿namespace WebSite.Docus.Inputs.Inputs;

class Input6 : BaseComponent
{
    private KInput? input;

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Component<FieldControl>()
               .Set(c => c.OnVisibleChanged, OnVisibleChanged)
               .Set(c => c.OnEnabledChanged, OnEnabledChanged)
               .Set(c => c.OnReadOnlyChanged, OnReadOnlyChanged)
               .Set(c => c.SetValue, SetValue)
               .Set(c => c.GetValue, GetValue)
               .Build();

        builder.Field<KInput>("网址：", "Url")
               .Set(f => f.Type, InputType.Url)
               .Build(value => input = value);
    }

    private void OnVisibleChanged(bool value) => input?.SetVisible(value);
    private void OnEnabledChanged(bool value) => input?.SetEnabled(value);
    private void OnReadOnlyChanged(bool value) => input?.SetReadOnly(value);
    private void SetValue() => input?.SetValue("http://www.test.com");
    private string? GetValue() => input?.Value;
}